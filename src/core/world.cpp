#include "kwa/world.hpp"
#include <algorithm>
#include <cmath>
#include <utility>
namespace kwa {
namespace {
void decay(float& value, float dt) {
    value = std::max(0.0f, value - dt);
}
} // namespace
AttackSpec attackSpec(Action action, int combo) {
    if (action == Action::Heavy)
        return {0.38f, 0.8f, 30, 48, 96, -0.05f};
    if (combo == 3)
        return {0.13f, 0.43f, 16, 36, 86, 0.05f};
    return {0.10f, 0.29f, 12, 22, 78, 0.1f};
}
EnemySpec enemySpec(EnemyKind kind) {
    switch (kind) {
    case EnemyKind::Scout:
        return {42, 100, 0.48f, 0.8f, 10, 52};
    case EnemyKind::Brute:
        return {130, 48, 0.95f, 1.2f, 25, 85};
    default:
        return {65, 68, 0.67f, 1.0f, 16, 64};
    }
}
World::World(std::uint32_t seed) : random_(seed) {}
void World::start(Level level, Mode mode, int stage, bool preservePlayer) {
    if (!preservePlayer) {
        player_ = Player{};
        score_ = 0;
        kills_ = 0;
        wave_ = 1;
        nextId_ = 1;
    }
    level_ = std::move(level);
    mode_ = mode;
    stage_ = stage;
    outcome_ = Outcome::Playing;
    player_.position = level_.spawn;
    player_.action = Action::Idle;
    player_.actionTime = 0;
    player_.invulnerable = 0;
    player_.stamina = 100;
    player_.combo = 0;
    player_.comboWindow = 0;
    player_.bufferedLight = false;
    player_.bufferTime = 0;
    wolf_ = Wolf{};
    wolf_.position = level_.wolfSpawn;
    wolf_.rescued = (stage != 1 || mode == Mode::Survival);
    enemies_.clear();
    events_.clear();
    flowTimer_ = 0;
    waveDelay_ = 0;
    flow_ = level_.flow(player_.position);
    if (mode == Mode::Survival)
        spawnWave();
    else
        for (std::size_t i = 0; i < level_.enemySpawns.size(); ++i)
            spawn(level_.enemySpawns[i],
                  (i % 5 == 4 || (stage == 2 && i == 0))
                      ? EnemyKind::Brute
                      : (i % 3 == 0 ? EnemyKind::Scout : EnemyKind::Soldier));
}
void World::spawn(Vec2 position, EnemyKind kind) {
    Enemy e;
    e.id = nextId_++;
    e.kind = kind;
    e.position = level_.nearestFloor(position);
    e.alerted = mode_ == Mode::Survival;
    e.health = e.maxHealth =
        enemySpec(kind).health +
        (mode_ == Mode::Survival ? static_cast<float>(std::min(wave_ - 1, 20)) * 4 : 0);
    enemies_.push_back(e);
}
void World::spawnWave() {
    const int count = std::min(4 + wave_ * 2, 32);
    std::vector<Vec2> candidates;
    flow_ = level_.flow(player_.position);
    for (int y = 0; y < level_.height; ++y)
        for (int x = 0; x < level_.width; ++x) {
            Vec2 p{(static_cast<float>(x) + 0.5f) * Level::tileSize,
                   (static_cast<float>(y) + 0.5f) * Level::tileSize};
            if (flow_[static_cast<std::size_t>(y * level_.width + x)] >= 0 &&
                distance(p, player_.position) > 320)
                candidates.push_back(p);
        }
    std::shuffle(candidates.begin(), candidates.end(), random_);
    for (int i = 0; i < count && !candidates.empty(); ++i)
        spawn(candidates[static_cast<std::size_t>(i) % candidates.size()],
              (wave_ >= 2 && i % 5 == 0) ? EnemyKind::Brute
                                         : (i % 3 == 0 ? EnemyKind::Scout : EnemyKind::Soldier));
    emit(EventKind::Wave, player_.position, wave_);
}
int World::livingEnemies() const {
    return static_cast<int>(std::count_if(enemies_.begin(), enemies_.end(), [](const Enemy& e) {
        return e.state != EnemyState::Dead;
    }));
}
void World::emit(EventKind kind, Vec2 position, int amount) {
    events_.push_back({kind, position, amount, 0.7f, ++eventSerial_});
}
void World::beginAttack(Action action) {
    const int nextCombo =
        action == Action::Light ? (player_.comboWindow > 0 ? player_.combo % 3 + 1 : 1) : 0;
    const auto spec = attackSpec(action, nextCombo);
    if (player_.stamina < spec.cost)
        return;
    player_.combo = nextCombo;
    player_.comboWindow = 0;
    player_.stamina -= spec.cost;
    player_.regenDelay = 0.55f;
    player_.action = action;
    player_.actionTime = 0;
    player_.strikeDone = false;
    ++player_.swing;
    player_.bufferedLight = false;
    player_.bufferTime = 0;
}
void World::updatePlayer(const Input& input, float dt) {
    decay(player_.invulnerable, dt);
    decay(player_.comboWindow, dt);
    decay(player_.regenDelay, dt);
    decay(player_.bufferTime, dt);
    if (player_.bufferTime == 0)
        player_.bufferedLight = false;
    if (player_.regenDelay == 0 && player_.action == Action::Idle)
        player_.stamina = std::min(100.0f, player_.stamina + 32 * dt);
    const Vec2 movement = normalized(input.move);
    player_.moving = length(movement) > 0 && player_.action == Action::Idle;
    if (player_.action == Action::Idle && length(movement) > 0)
        player_.facing = movement;
    if (input.light && (player_.action == Action::Light || player_.action == Action::Heavy)) {
        player_.bufferedLight = true;
        player_.bufferTime = 0.22f;
    }
    // Dodges cancel recovery, but never the windup before a strike.
    const bool canDodge = player_.action == Action::Idle ||
                          ((player_.action == Action::Light || player_.action == Action::Heavy) &&
                           player_.strikeDone);
    if (input.dodge && canDodge && player_.stamina >= 25) {
        player_.stamina -= 25;
        player_.regenDelay = 0.7f;
        player_.action = Action::Dodge;
        player_.actionTime = 0;
        player_.invulnerable = 0.24f;
        player_.dodgeDirection = length(movement) > 0 ? movement : player_.facing;
        emit(EventKind::Dodge, player_.position);
    }
    if (player_.action == Action::Idle) {
        if (input.parry && player_.stamina >= 18) {
            player_.stamina -= 18;
            player_.regenDelay = 0.6f;
            player_.action = Action::Parry;
            player_.actionTime = 0;
        } else if (input.heavy)
            beginAttack(Action::Heavy);
        else if (input.light || player_.bufferedLight)
            beginAttack(Action::Light);
        else if (input.heal && player_.potions > 0 && player_.health < player_.maxHealth) {
            --player_.potions;
            player_.health = std::min(player_.maxHealth, player_.health + 45);
            emit(EventKind::Heal, player_.position, 45);
        }
    }
    player_.actionTime += dt;
    switch (player_.action) {
    case Action::Light:
    case Action::Heavy: {
        const auto spec = attackSpec(player_.action, player_.combo);
        if (!player_.strikeDone && player_.actionTime >= spec.windup) {
            strike();
            player_.strikeDone = true;
        }
        if (player_.actionTime >= spec.duration) {
            player_.action = Action::Idle;
            player_.comboWindow = 0.45f;
        }
        break;
    }
    case Action::Dodge:
        player_.position = level_.move(player_.position, player_.dodgeDirection * (460 * dt));
        if (player_.actionTime >= 0.30f)
            player_.action = Action::Idle;
        break;
    case Action::Parry:
        if (player_.actionTime >= 0.48f)
            player_.action = Action::Idle;
        break;
    case Action::Hurt:
        if (player_.actionTime >= 0.2f)
            player_.action = Action::Idle;
        break;
    case Action::Idle:
        player_.position = level_.move(player_.position, movement * (180 * dt));
        break;
    }
}
void World::strike() {
    const auto spec = attackSpec(player_.action, player_.combo);
    for (auto& e : enemies_) {
        const Vec2 delta = e.position - player_.position;
        if (e.state == EnemyState::Dead || e.lastSwing == player_.swing ||
            length(delta) > spec.range ||
            (length(delta) > 18 && dot(normalized(delta), player_.facing) < spec.arcDot) ||
            !level_.clearLine(player_.position, e.position))
            continue;
        e.lastSwing = player_.swing;
        hitEnemy(e, spec.damage + player_.damageBonus, player_.facing,
                 player_.action == Action::Heavy ? 0.8f : 0.28f);
    }
}
void World::hitEnemy(Enemy& e, float damage, Vec2 direction, float stun) {
    if (e.state == EnemyState::Dead)
        return;
    e.alerted = true;
    e.health = std::max(0.0f, e.health - damage);
    e.flash = 0.15f;
    e.position = level_.move(e.position, direction * (stun > 0.5f ? 30.0f : 12.0f));
    emit(EventKind::Hit, e.position, static_cast<int>(damage));
    if (e.health <= 0) {
        e.state = EnemyState::Dead;
        e.timer = 0.8f;
        ++kills_;
        score_ += e.kind == EnemyKind::Brute ? 150 : 100;
        player_.stamina = std::min(100.0f, player_.stamina + 12);
        emit(EventKind::Kill, e.position);
    } else {
        e.state = EnemyState::Stunned;
        e.timer = stun;
    }
}
void World::hitPlayer(Enemy& e) {
    const auto spec = enemySpec(e.kind);
    const Vec2 delta = player_.position - e.position;
    if (length(delta) > spec.range + 12 ||
        (length(delta) > 12 && dot(normalized(delta), e.attackDirection) < 0.2f) ||
        !level_.clearLine(e.position, player_.position))
        return;
    if (player_.action == Action::Parry && player_.actionTime <= 0.22f &&
        dot(player_.facing, normalized(e.position - player_.position)) > 0) {
        hitEnemy(e, 18, normalized(e.position - player_.position), 1.2f);
        player_.stamina = std::min(100.0f, player_.stamina + 28);
        score_ += 25;
        emit(EventKind::Parry, player_.position);
        return;
    }
    if (player_.invulnerable > 0)
        return;
    player_.health = std::max(0.0f, player_.health - spec.damage);
    player_.invulnerable = 0.65f;
    player_.action = Action::Hurt;
    player_.actionTime = 0;
    player_.comboWindow = 0;
    player_.position = level_.move(player_.position, normalized(delta) * 24);
    emit(EventKind::Hurt, player_.position, static_cast<int>(spec.damage));
}
void World::updateEnemies(float dt) {
    for (auto& e : enemies_) {
        decay(e.flash, dt);
        e.age += dt;
        if (e.state != EnemyState::Chase) {
            decay(e.timer, dt);
            if (e.timer > 0 || e.state == EnemyState::Dead)
                continue;
            if (e.state == EnemyState::Windup) {
                e.state = EnemyState::Recover;
                e.timer = enemySpec(e.kind).recovery;
                hitPlayer(e);
            } else
                e.state = EnemyState::Chase;
            continue;
        }
        const auto spec = enemySpec(e.kind);
        const Vec2 delta = player_.position - e.position;
        if (!e.alerted && length(delta) > 360)
            continue;
        e.alerted = true;
        e.facing = normalized(delta);
        if (length(delta) < spec.range - 5 && level_.clearLine(e.position, player_.position)) {
            e.state = EnemyState::Windup;
            e.timer = spec.windup;
            e.attackDirection = e.facing;
        } else {
            Vec2 direction = level_.steer(e.position, player_.position, flow_);
            Vec2 separation{};
            for (const auto& other : enemies_)
                if (other.id != e.id && other.state != EnemyState::Dead) {
                    const Vec2 d = e.position - other.position;
                    const float n = length(d);
                    if (n > 0.01f && n < 28)
                        separation += normalized(d) * ((28 - n) / 28);
                }
            e.position = level_.move(e.position, normalized(direction + separation * 0.65f) *
                                                     (spec.speed * dt));
        }
    }
    std::erase_if(enemies_,
                  [](const Enemy& e) { return e.state == EnemyState::Dead && e.timer <= 0; });
}
void World::updateWolf(const Input& input, float dt) {
    decay(wolf_.cooldown, dt);
    decay(wolf_.burst, dt);
    if (!wolf_.rescued) {
        if (distance(player_.position, wolf_.position) < 65 &&
            level_.clearLine(player_.position, wolf_.position)) {
            wolf_.rescued = true;
            emit(EventKind::Rescue, wolf_.position);
            score_ += 250;
        }
        return;
    }
    if (input.wolf && wolf_.cooldown == 0) {
        Enemy* target = nullptr;
        float best = 250;
        for (auto& e : enemies_)
            if (e.state != EnemyState::Dead && distance(e.position, player_.position) < best &&
                level_.clearLine(wolf_.position, e.position)) {
                best = distance(e.position, player_.position);
                target = &e;
            }
        if (target) {
            wolf_.facing = normalized(target->position - wolf_.position);
            wolf_.position = level_.move(target->position, wolf_.facing * (-20), 9);
            wolf_.cooldown = 5;
            wolf_.burst = 0.4f;
            hitEnemy(*target, 32, wolf_.facing, 1.0f);
            emit(EventKind::Wolf, wolf_.position);
        }
    }
    if (wolf_.burst == 0 && distance(wolf_.position, player_.position) > 48) {
        wolf_.facing = level_.steer(wolf_.position, player_.position, flow_);
        wolf_.position = level_.move(wolf_.position, wolf_.facing * (225 * dt), 9);
    }
}
void World::tick(const Input& input, float dt) {
    if (outcome_ != Outcome::Playing || dt <= 0 || dt > 0.05f)
        return;
    for (auto& event : events_)
        event.remaining -= dt;
    std::erase_if(events_, [](const Event& event) { return event.remaining <= 0; });
    updatePlayer(input, dt);
    decay(flowTimer_, dt);
    if (flowTimer_ == 0) {
        flow_ = level_.flow(player_.position);
        flowTimer_ = 0.2f;
    }
    updateWolf(input, dt);
    updateEnemies(dt);
    if (player_.health <= 0) {
        outcome_ = Outcome::Defeat;
        return;
    }
    if (mode_ == Mode::Survival) {
        if (livingEnemies() == 0) {
            if (waveDelay_ == 0) {
                waveDelay_ = 4;
                player_.health = std::min(player_.maxHealth, player_.health + 20);
                player_.potions = std::min(5, player_.potions + 1);
            }
            waveDelay_ -= dt;
            if (waveDelay_ <= 0) {
                waveDelay_ = 0;
                ++wave_;
                spawnWave();
            }
        }
    } else if (stage_ == 2 && livingEnemies() == 0)
        outcome_ = Outcome::Victory;
    else if (input.interact && distance(player_.position, level_.exit) < 75 &&
             ((stage_ == 0 && livingEnemies() == 0) || (stage_ == 1 && wolf_.rescued)))
        outcome_ = Outcome::StageClear;
}
void World::upgrade(int choice) {
    if (outcome_ != Outcome::StageClear)
        return;
    if (choice == 1)
        player_.damageBonus += 6;
    else if (choice == 2)
        player_.maxHealth += 25;
    else
        player_.potions = std::min(6, player_.potions + 2);
    player_.health = player_.maxHealth;
}
} // namespace kwa

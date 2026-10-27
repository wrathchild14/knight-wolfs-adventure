#include "kwa/world.hpp"
#include <cmath>
#include <iostream>
#include <stdexcept>
#include <string>
using namespace kwa;
namespace {
int checks = 0;
void check(bool condition, const std::string& message) {
    ++checks;
    if (!condition)
        throw std::runtime_error(message);
}
constexpr float dt = 1.0f / 120;
void advance(World& w, int ticks, Input input = {}) {
    for (int i = 0; i < ticks; ++i) {
        w.tick(input, dt);
        input.clearEdges();
    }
}
Level arena(std::vector<Vec2> enemies = {}) {
    Level l;
    l.theme = "Forest";
    l.width = 20;
    l.height = 14;
    l.tiles.assign(280, 1);
    l.spawn = {320, 320};
    l.wolfSpawn = {280, 320};
    l.exit = {350, 320};
    l.enemySpawns = std::move(enemies);
    return l;
}
void movement() {
    World a, b;
    a.start(arena(), Mode::Campaign);
    b.start(arena(), Mode::Campaign);
    Input right;
    right.move = {1, 0};
    Input diagonal;
    diagonal.move = {1, 1};
    advance(a, 120, right);
    advance(b, 120, diagonal);
    check(std::abs(distance(a.player().position, {320, 320}) - 180) < 0.1f,
          "movement is time based");
    check(std::abs(distance(b.player().position, {320, 320}) - 180) < 0.1f,
          "diagonal movement normalized");
    auto l = arena();
    for (int y = 0; y < l.height; ++y)
        l.tiles[static_cast<std::size_t>(y * l.width + 6)] = 10;
    a.start(l, Mode::Campaign);
    right.dodge = true;
    advance(a, 120, right);
    check(a.player().position.x < 373, "movement and dodge cannot tunnel through walls");
    check(l.move({15, 15}, {-100, -100}).x >= 12, "world boundary collision");
}
void combat() {
    World w;
    w.start(arena({{365, 320}}), Mode::Campaign);
    Input attack;
    attack.light = true;
    advance(w, 16, attack);
    check(w.enemies().front().health == 20, "one light swing deals 22 damage");
    advance(w, 12);
    check(w.enemies().front().health == 20, "a swing hits an enemy once");
    advance(w, 8);
    advance(w, 16, attack);
    check(w.kills() == 1, "combo can kill enemy");
    check(w.score() == 100, "kill counted once");
    advance(w, 120);
    check(w.kills() == 1, "corpse cleanup does not recount kills");
    w.start(arena({{280, 320}}), Mode::Campaign);
    advance(w, 16, attack);
    check(w.enemies().front().health == 42, "attacks do not hit behind the player");
    auto l = arena({{460, 350}});
    l.spawn = {365, 350};
    for (int y = 0; y < l.height; ++y)
        l.tiles[static_cast<std::size_t>(y * 20 + 6)] = 10;
    w.start(l, Mode::Campaign);
    Input heavy;
    heavy.heavy = true;
    advance(w, 50, heavy);
    check(w.enemies().front().health == 42, "sword cannot hit through a wall");
    w.start(arena({{370, 320}}), Mode::Campaign);
    advance(w, 50, heavy);
    check(w.kills() == 1, "heavy attack deals lethal damage to scout");
    check(w.player().stamina == 82, "heavy costs 30 stamina and kill restores 12");
}
void defense() {
    World w;
    w.start(arena({{360, 320}}), Mode::Campaign);
    advance(w, 1);
    check(w.enemies().front().state == EnemyState::Windup, "enemy announces attack");
    advance(w, 38);
    Input parry;
    parry.parry = true;
    advance(w, 22, parry);
    check(w.player().health == 100, "timed frontal parry prevents damage");
    check(w.score() == 25, "successful parry rewarded");
    w.start(arena({{360, 320}}), Mode::Campaign);
    advance(w, 90);
    check(w.player().health == 90, "unblocked scout attack damages player");
    w.start(arena({{360, 320}}), Mode::Campaign);
    advance(w, 40);
    Input dodge;
    dodge.dodge = true;
    dodge.move = {0, 1};
    advance(w, 25, dodge);
    check(w.player().health == 100, "dodge evades incoming strike");
    check(w.player().stamina == 75, "dodge consumes stamina");
    w.start(arena({{360, 320}}), Mode::Campaign);
    advance(w, 90);
    Input heal;
    heal.heal = true;
    advance(w, 1, heal);
    check(w.player().health == 100 && w.player().potions == 2,
          "healing uses one potion and caps health");
    advance(w, 1, heal);
    check(w.player().potions == 2, "healing at full health does not waste potion");
}
void companion() {
    World w;
    w.start(arena({{400, 320}}), Mode::Campaign);
    Input command;
    command.wolf = true;
    advance(w, 1, command);
    check(w.enemies().front().health == 10, "wolf attack deals damage");
    check(w.enemies().front().state == EnemyState::Stunned, "wolf staggers target");
    advance(w, 1, command);
    check(w.enemies().front().health == 10, "wolf cooldown prevents repeated damage");
    auto l = arena();
    l.wolfSpawn = {700, 320};
    w.start(l, Mode::Campaign, 1);
    check(!w.wolf().rescued, "dungeon begins with missing wolf");
    Input interact;
    interact.interact = true;
    advance(w, 1, interact);
    check(w.outcome() == Outcome::Playing, "cannot leave without wolf");
    Input right;
    right.move = {1, 0};
    advance(w, 215, right);
    check(w.wolf().rescued, "approaching wolf rescues it");
}
void progression() {
    World w;
    w.start(arena(), Mode::Campaign);
    Input use;
    use.interact = true;
    advance(w, 1, use);
    check(w.outcome() == Outcome::StageClear, "forest gate advances after clear");
    w.upgrade(2);
    w.start(arena(), Mode::Campaign, 1, true);
    check(w.player().maxHealth == 125, "upgrade persists across chapters");
    advance(w, 1, use);
    check(w.outcome() == Outcome::StageClear, "rescued wolf enables dungeon exit");
    w.start(arena(), Mode::Campaign, 2, true);
    advance(w, 1);
    check(w.outcome() == Outcome::Victory, "village clear wins campaign");
    const auto p = w.player().position;
    Input move;
    move.move = {1, 0};
    advance(w, 60, move);
    check(distance(w.player().position, p) == 0, "finished world is frozen");
    w.start(arena(), Mode::Survival);
    check(w.livingEnemies() == 6, "survival starts with bounded wave");
    for (const auto& e : w.enemies())
        check(distance(e.position, w.player().position) > 320, "survival spawns away from player");
}
void maps() {
    for (const auto* name : {"forest", "dungeon", "village", "survival"}) {
        const auto l = Level::load(std::string(KWA_LEVEL_DIR) + "/" + name + ".level");
        for (const auto target : l.enemySpawns) {
            Vec2 position = l.spawn;
            const auto route = l.flow(target);
            for (int step = 0; step < 5000 && distance(position, target) > 5; ++step)
                position = l.move(position, l.steer(position, target, route) * 1.5f);
            check(distance(position, target) <= 5,
                  std::string(name) + ": navigation reaches encounter around obstacles");
        }
        check(l.fits(l.spawn), "valid player spawn");
        check(l.fits(l.wolfSpawn), "valid wolf spawn");
        const auto flow = l.flow(l.spawn);
        for (const auto& e : l.enemySpawns)
            check(flow[static_cast<std::size_t>(static_cast<int>(e.y / 64) * l.width +
                                                static_cast<int>(e.x / 64))] >= 0,
                  std::string(name) + ": enemy reachable");
        if (std::string(name) == "dungeon")
            check(flow[static_cast<std::size_t>(static_cast<int>(l.wolfSpawn.y / 64) * l.width +
                                                static_cast<int>(l.wolfSpawn.x / 64))] >= 0,
                  "wolf reachable");
        if (std::string(name) == "forest" || std::string(name) == "dungeon")
            check(flow[static_cast<std::size_t>(static_cast<int>(l.exit.y / 64) * l.width +
                                                static_cast<int>(l.exit.x / 64))] >= 0,
                  "exit reachable");
    }
    bool threw = false;
    try {
        Level::load("missing.level");
    } catch (const std::runtime_error&) {
        threw = true;
    }
    check(threw, "missing level fails clearly");
}
} // namespace
int main() {
    try {
        movement();
        combat();
        defense();
        companion();
        progression();
        maps();
        std::cout << checks << " checks passed\n";
        return 0;
    } catch (const std::exception& e) {
        std::cerr << "FAILED after " << checks << " checks: " << e.what() << '\n';
        return 1;
    }
}

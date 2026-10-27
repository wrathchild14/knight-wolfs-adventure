#pragma once
#include "kwa/level.hpp"
#include <cstdint>
#include <random>
#include <vector>
namespace kwa {
enum class Mode { Campaign, Survival };
enum class Outcome { Playing, StageClear, Defeat, Victory };
enum class Action { Idle, Light, Heavy, Dodge, Parry, Hurt };
enum class EnemyKind { Scout, Soldier, Brute };
enum class EnemyState { Chase, Windup, Recover, Stunned, Dead };
enum class EventKind { Hit, Kill, Hurt, Parry, Dodge, Wolf, Heal, Rescue, Wave };
struct Input {
    Vec2 move{};
    bool light{}, heavy{}, dodge{}, parry{}, wolf{}, heal{}, interact{};
    void clearEdges() { light = heavy = dodge = parry = wolf = heal = interact = false; }
};
struct Player {
    Vec2 position{}, facing{1, 0}, dodgeDirection{1, 0};
    float health{100}, maxHealth{100}, stamina{100};
    Action action{Action::Idle};
    float actionTime{}, invulnerable{}, comboWindow{}, regenDelay{}, bufferTime{};
    int combo{}, potions{3};
    float damageBonus{};
    bool strikeDone{}, bufferedLight{}, moving{};
    std::uint32_t swing{};
};
struct Enemy {
    std::uint32_t id{};
    Vec2 position{}, facing{-1, 0}, attackDirection{};
    EnemyKind kind{EnemyKind::Soldier};
    EnemyState state{EnemyState::Chase};
    float health{60}, maxHealth{60}, timer{}, flash{}, age{};
    std::uint32_t lastSwing{};
    bool alerted{};
};
struct Wolf {
    Vec2 position{}, facing{1, 0};
    bool rescued{true};
    float cooldown{}, burst{};
};
struct Event {
    EventKind kind;
    Vec2 position;
    int amount{};
    float remaining{0.7f};
    std::uint64_t serial{};
};
struct AttackSpec {
    float windup, duration, cost, damage, range, arcDot;
};
AttackSpec attackSpec(Action action, int combo);
struct EnemySpec {
    float health, speed, windup, recovery, damage, range;
};
EnemySpec enemySpec(EnemyKind kind);
class World {
  public:
    explicit World(std::uint32_t seed = 42);
    void start(Level level, Mode mode, int stage = 0, bool preservePlayer = false);
    // Caller supplies fixed 1/120-second ticks. No graphics or wall-clock dependencies.
    void tick(const Input& input, float dt);
    void upgrade(int choice);
    const Level& level() const { return level_; }
    const Player& player() const { return player_; }
    const Wolf& wolf() const { return wolf_; }
    const std::vector<Enemy>& enemies() const { return enemies_; }
    const std::vector<Event>& events() const { return events_; }
    Outcome outcome() const { return outcome_; }
    Mode mode() const { return mode_; }
    int stage() const { return stage_; }
    int score() const { return score_; }
    int kills() const { return kills_; }
    int wave() const { return wave_; }
    float waveDelay() const { return waveDelay_; }
    int livingEnemies() const;

  private:
    Level level_;
    Player player_;
    Wolf wolf_;
    std::vector<Enemy> enemies_;
    std::vector<Event> events_;
    std::vector<int> flow_;
    std::mt19937 random_;
    Mode mode_{Mode::Campaign};
    Outcome outcome_{Outcome::Playing};
    int stage_{}, score_{}, kills_{}, wave_{1};
    float flowTimer_{}, waveDelay_{};
    std::uint32_t nextId_{1};
    std::uint64_t eventSerial_{};
    void spawn(Vec2 position, EnemyKind kind);
    void spawnWave();
    void updatePlayer(const Input& input, float dt);
    void updateEnemies(float dt);
    void updateWolf(const Input& input, float dt);
    void beginAttack(Action action);
    void strike();
    void hitEnemy(Enemy& enemy, float damage, Vec2 direction, float stun);
    void hitPlayer(Enemy& enemy);
    void emit(EventKind kind, Vec2 position, int amount = 0);
};
} // namespace kwa

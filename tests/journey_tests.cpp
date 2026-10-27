#include "kwa/world.hpp"
#include <iostream>
#include <stdexcept>
#include <string>
using namespace kwa;
namespace {
// A deterministic player uses the same public inputs as the application. No
// health overrides, direct enemy mutations, or skipped progression rules.
Input chooseInput(const World& world) {
    const auto& player = world.player();
    const Enemy* nearest = nullptr;
    float closest = 1e9f;
    for (const auto& enemy : world.enemies()) {
        const float range = distance(enemy.position, player.position);
        if (enemy.health > 0 && range < closest) {
            closest = range;
            nearest = &enemy;
        }
    }
    const Vec2 target = nearest                                       ? nearest->position
                        : world.stage() == 1 && !world.wolf().rescued ? world.wolf().position
                                                                      : world.level().exit;
    Input input;
    input.move = world.level().steer(player.position, target, world.level().flow(target));
    input.interact = true;
    input.wolf = true;
    if (nearest && closest < 80) {
        input.move = normalized(target - player.position);
        if (nearest->state == EnemyState::Windup && nearest->timer < 0.17f)
            input.parry = true;
        else if (player.health < player.maxHealth - 30 && player.potions > 0)
            input.heal = true;
        else if (player.stamina > 35)
            input.heavy = true;
        else if (player.stamina >= 12)
            input.light = true;
    }
    if (player.health < player.maxHealth - 40 && player.potions > 0) {
        input.light = input.heavy = input.parry = false;
        input.heal = true;
    }
    return input;
}
} // namespace
int main() {
    try {
        World world;
        const std::string root = KWA_LEVEL_DIR;
        const char* names[]{"forest", "dungeon", "village"};
        for (int stage = 0; stage < 3; ++stage) {
            world.start(Level::load(root + "/" + names[stage] + ".level"), Mode::Campaign, stage,
                        stage > 0);
            for (int frame = 0; frame < 120 * 360 && world.outcome() == Outcome::Playing; ++frame)
                world.tick(chooseInput(world), 1.0f / 120);
            const auto expected = stage == 2 ? Outcome::Victory : Outcome::StageClear;
            if (world.outcome() != expected)
                throw std::runtime_error(std::string("Could not finish ") + names[stage]);
            world.upgrade(1);
        }
        std::cout << "Complete campaign passed: " << world.kills() << " kills, " << world.score()
                  << " score\n";
        world.start(Level::load(root + "/survival.level"), Mode::Survival);
        for (int frame = 0;
             frame < 120 * 360 && world.outcome() == Outcome::Playing && world.wave() < 4; ++frame)
            world.tick(chooseInput(world), 1.0f / 120);
        if (world.wave() < 4 || world.outcome() != Outcome::Playing)
            throw std::runtime_error("Could not complete three survival waves");
        std::cout << "Three survival waves passed, wave four spawned\n";
    } catch (const std::exception& error) {
        std::cerr << error.what() << '\n';
        return 1;
    }
}

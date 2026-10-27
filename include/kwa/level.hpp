#pragma once
#include "kwa/math.hpp"
#include <filesystem>
#include <string>
#include <vector>
namespace kwa {
struct Level {
    static constexpr float tileSize = 64;
    std::string theme;
    int width{}, height{};
    Vec2 spawn{}, wolfSpawn{}, exit{};
    std::vector<int> tiles;
    std::vector<Vec2> enemySpawns;
    static Level load(const std::filesystem::path& path);
    int tile(int x, int y) const;
    bool solid(int x, int y) const;
    bool fits(Vec2 p, float radius = 12) const;
    bool clearLine(Vec2 a, Vec2 b, float radius = 3) const;
    Vec2 move(Vec2 from, Vec2 delta, float radius = 12) const;
    Vec2 nearestFloor(Vec2 p) const;
    std::vector<int> flow(Vec2 target) const;
    Vec2 steer(Vec2 from, Vec2 target, const std::vector<int>& field) const;
};
} // namespace kwa

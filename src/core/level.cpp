#include "kwa/level.hpp"
#include <algorithm>
#include <fstream>
#include <limits>
#include <queue>
#include <stdexcept>
namespace kwa {
Level Level::load(const std::filesystem::path& path) {
    std::ifstream in(path);
    Level l;
    std::string magic;
    int version{};
    if (!(in >> magic >> version >> l.theme >> l.width >> l.height >> l.spawn.x >> l.spawn.y >>
          l.wolfSpawn.x >> l.wolfSpawn.y) ||
        magic != "KWA_LEVEL" || version != 1 || l.width < 2 || l.height < 2 || l.width > 128 ||
        l.height > 128 || (l.theme != "Forest" && l.theme != "Dungeon" && l.theme != "Town"))
        throw std::runtime_error("Invalid level: " + path.string());
    l.tiles.resize(static_cast<std::size_t>(l.width * l.height));
    for (int y = 0; y < l.height; ++y)
        for (int x = 0; x < l.width; ++x) {
            int t{};
            if (!(in >> t) || t < 1 || (t > (l.theme == "Town" ? 26 : 15) && t != 99))
                throw std::runtime_error("Invalid tile in " + path.string());
            const Vec2 center{(static_cast<float>(x) + 0.5f) * tileSize,
                              (static_cast<float>(y) + 0.5f) * tileSize};
            if (t == 99) {
                l.enemySpawns.push_back(center);
                t = 1;
            }
            if (t == 15 && l.theme != "Town")
                l.exit = center;
            l.tiles[static_cast<std::size_t>(y * l.width + x)] = t;
        }
    l.spawn = l.nearestFloor(l.spawn);
    l.wolfSpawn = l.nearestFloor(l.wolfSpawn);
    return l;
}
int Level::tile(int x, int y) const {
    if (x < 0 || y < 0 || x >= width || y >= height)
        return -1;
    return tiles[static_cast<std::size_t>(y * width + x)];
}
bool Level::solid(int x, int y) const {
    const int t = tile(x, y);
    if (t < 0)
        return true;
    if (theme == "Town")
        return t >= 14;
    if (t == 15)
        return false;
    return t >= (theme == "Dungeon" ? 7 : 10);
}
bool Level::fits(Vec2 p, float r) const {
    for (int y = static_cast<int>(std::floor((p.y - r) / tileSize));
         y <= static_cast<int>(std::floor((p.y + r) / tileSize)); ++y)
        for (int x = static_cast<int>(std::floor((p.x - r) / tileSize));
             x <= static_cast<int>(std::floor((p.x + r) / tileSize)); ++x)
            if (solid(x, y))
                return false;
    return true;
}
bool Level::clearLine(Vec2 a, Vec2 b, float radius) const {
    const int steps = std::max(1, static_cast<int>(distance(a, b) / 8));
    for (int i = 0; i <= steps; ++i)
        if (!fits(a + (b - a) * (static_cast<float>(i) / static_cast<float>(steps)), radius))
            return false;
    return true;
}
Vec2 Level::move(Vec2 from, Vec2 delta, float r) const {
    const int steps = std::max(1, static_cast<int>(length(delta) / 5) + 1);
    delta = delta * (1.0f / static_cast<float>(steps));
    for (int i = 0; i < steps; ++i) {
        if (fits({from.x + delta.x, from.y}, r))
            from.x += delta.x;
        if (fits({from.x, from.y + delta.y}, r))
            from.y += delta.y;
    }
    return from;
}
Vec2 Level::nearestFloor(Vec2 p) const {
    float best = std::numeric_limits<float>::max();
    Vec2 result{};
    for (int y = 0; y < height; ++y)
        for (int x = 0; x < width; ++x)
            if (!solid(x, y)) {
                Vec2 c{(static_cast<float>(x) + 0.5f) * tileSize,
                       (static_cast<float>(y) + 0.5f) * tileSize};
                if (distance(p, c) < best) {
                    best = distance(p, c);
                    result = c;
                }
            }
    if (best == std::numeric_limits<float>::max())
        throw std::runtime_error("Level has no walkable floor");
    return fits(p) ? p : result;
}
std::vector<int> Level::flow(Vec2 target) const {
    std::vector<int> field(static_cast<std::size_t>(width * height), -1);
    const int tx = static_cast<int>(target.x / tileSize),
              ty = static_cast<int>(target.y / tileSize);
    if (solid(tx, ty))
        return field;
    std::queue<int> q;
    q.push(ty * width + tx);
    field[static_cast<std::size_t>(q.front())] = 0;
    constexpr int dx[]{1, -1, 0, 0}, dy[]{0, 0, 1, -1};
    while (!q.empty()) {
        const int index = q.front();
        q.pop();
        for (int d = 0; d < 4; ++d) {
            const int x = index % width + dx[d], y = index / width + dy[d], n = y * width + x;
            if (!solid(x, y) && field[static_cast<std::size_t>(n)] < 0) {
                field[static_cast<std::size_t>(n)] = field[static_cast<std::size_t>(index)] + 1;
                q.push(n);
            }
        }
    }
    return field;
}
Vec2 Level::steer(Vec2 from, Vec2 target, const std::vector<int>& field) const {
    if (clearLine(from, target, 12))
        return normalized(target - from);
    const int x = static_cast<int>(from.x / tileSize), y = static_cast<int>(from.y / tileSize);
    if (solid(x, y) || field.size() != tiles.size())
        return {};
    int best = field[static_cast<std::size_t>(y * width + x)];
    Vec2 next{(static_cast<float>(x) + 0.5f) * tileSize, (static_cast<float>(y) + 0.5f) * tileSize};
    constexpr int dx[]{1, -1, 0, 0}, dy[]{0, 0, 1, -1};
    for (int d = 0; d < 4; ++d) {
        int nx = x + dx[d], ny = y + dy[d];
        if (solid(nx, ny))
            continue;
        int cost = field[static_cast<std::size_t>(ny * width + nx)];
        if (cost >= 0 && (best < 0 || cost < best)) {
            best = cost;
            next = {(static_cast<float>(nx) + 0.5f) * tileSize,
                    (static_cast<float>(ny) + 0.5f) * tileSize};
        }
    }
    // Align to the corridor center before rounding a corner.
    if (!clearLine(from, next, 12))
        next = {(static_cast<float>(x) + 0.5f) * tileSize,
                (static_cast<float>(y) + 0.5f) * tileSize};
    return normalized(next - from);
}
} // namespace kwa

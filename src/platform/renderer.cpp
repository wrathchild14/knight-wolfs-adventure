#include "renderer.hpp"
#include <algorithm>
#include <cmath>
#include <string>
#include <vector>
namespace kwa {
namespace {
constexpr Color ink{12, 19, 26, 255}, panelColor{19, 30, 39, 242}, gold{231, 186, 103, 255},
    muted{150, 169, 175, 255}, pale{236, 239, 223, 255}, mint{104, 210, 168, 255};
Vector2 rv(Vec2 v) {
    return {v.x, v.y};
}
void bar(int x, int y, int w, int h, float value, float max, Color color) {
    DrawRectangle(x, y, w, h, {40, 52, 60, 255});
    DrawRectangle(x, y,
                  static_cast<int>(static_cast<float>(w) * std::clamp(value / max, 0.0f, 1.0f)), h,
                  color);
}
const char* stageName(int stage) {
    return stage == 0 ? "THE WILDWOOD" : stage == 1 ? "THE HOLLOW KEEP" : "THE HOMECOMING";
}
} // namespace
void Renderer::text(const char* value, int x, int y, int size, Color color) const {
    DrawText(value, x, y, size, color);
}
void Renderer::sprite(const std::string& family, const std::string& animation, int frames, Vec2 p,
                      Vec2 facing, float time, Color tint, float scale, bool loop) {
    const auto t = assets_.texture("Sprites/" + family + "/" + family + animation + ".png");
    const float fw = static_cast<float>(t.width) / static_cast<float>(frames);
    const int frame = loop ? static_cast<int>(time * 10) % frames
                           : std::min(frames - 1, static_cast<int>(time * 10));
    DrawEllipse(static_cast<int>(p.x), static_cast<int>(p.y), 20 * scale / 1.5f, 7,
                Fade(BLACK, 0.3f));
    DrawTexturePro(
        t,
        {fw * static_cast<float>(frame), 0, facing.x < 0 ? -fw : fw, static_cast<float>(t.height)},
        {p.x - fw * scale / 2, p.y - static_cast<float>(t.height) * scale + 8, fw * scale,
         static_cast<float>(t.height) * scale},
        {0, 0}, 0, tint);
}
Camera2D Renderer::camera(const World& w) {
    const auto& p = w.player();
    const auto& l = w.level();
    const float worldWidth = static_cast<float>(l.width) * Level::tileSize,
                worldHeight = static_cast<float>(l.height) * Level::tileSize;
    const float cx =
        worldWidth > width ? std::clamp(p.position.x, 640.0f, worldWidth - 640) : worldWidth / 2;
    const float cy = worldHeight > height ? std::clamp(p.position.y, 360.0f, worldHeight - 360)
                                          : worldHeight / 2;
    return {{640, 360}, {cx, cy}, 0, 1};
}
void Renderer::world(const World& w, float clock) {
    ClearBackground(ink);
    const auto& p = w.player();
    const auto& l = w.level();
    const auto view = camera(w);
    const float cx = view.target.x, cy = view.target.y;
    BeginMode2D(view);
    for (int y = 0; y < l.height; ++y)
        for (int x = 0; x < l.width; ++x) {
            const float px = static_cast<float>(x) * 64, py = static_cast<float>(y) * 64;
            if (px < cx - 704 || px > cx + 704 || py < cy - 424 || py > cy + 424)
                continue;
            const auto tile = assets_.texture("Tiles/" + l.theme + "/Tile" +
                                              std::to_string(l.tile(x, y)) + ".png");
            DrawTexturePro(tile,
                           {0, 0, static_cast<float>(tile.width), static_cast<float>(tile.height)},
                           {px, py, 64, 64}, {0, 0}, 0, WHITE);
        }
    const bool exitReady = w.stage() == 0 ? w.livingEnemies() == 0 : w.wolf().rescued;
    if (w.mode() == Mode::Campaign && w.stage() < 2) {
        DrawCircleLines(static_cast<int>(l.exit.x), static_cast<int>(l.exit.y),
                        30 + std::sin(clock * 3) * 3, exitReady ? mint : gold);
        DrawCircleLines(static_cast<int>(l.exit.x), static_cast<int>(l.exit.y), 36,
                        Fade(exitReady ? mint : gold, 0.45f));
    }
    for (const auto& e : w.enemies())
        if (e.state == EnemyState::Windup) {
            const auto spec = enemySpec(e.kind);
            const float progress = 1 - e.timer / spec.windup;
            DrawCircleV(rv(e.position), spec.range, Fade(RED, 0.05f + progress * 0.12f));
            DrawCircleLines(static_cast<int>(e.position.x), static_cast<int>(e.position.y),
                            spec.range, {250, 100, 80, 220});
            DrawLineEx(rv(e.position), rv(e.position + e.attackDirection * spec.range), 3, gold);
        }
    struct DrawItem {
        float y;
        int kind;
        std::size_t index;
    };
    std::vector<DrawItem> order{{p.position.y, 0, 0}, {w.wolf().position.y, 1, 0}};
    for (std::size_t i = 0; i < w.enemies().size(); ++i)
        order.push_back({w.enemies()[i].position.y, 2, i});
    std::stable_sort(order.begin(), order.end(),
                     [](const auto& a, const auto& b) { return a.y < b.y; });
    for (const auto& item : order) {
        if (item.kind == 0) {
            const bool attack = p.action == Action::Light || p.action == Action::Heavy;
            const bool dead = w.outcome() == Outcome::Defeat;
            const auto anim = dead                        ? "Death"
                              : attack                    ? "Attack"
                              : p.action == Action::Parry ? "Pray"
                              : p.moving                  ? "Running"
                                                          : "Idle";
            const int frames = dead ? 4 : attack ? 15 : p.action == Action::Parry ? 12 : 8;
            Color tint = p.invulnerable > 0 && static_cast<int>(clock * 18) % 2 == 0
                             ? Fade(WHITE, 0.5f)
                             : WHITE;
            if (p.action == Action::Dodge)
                tint = mint;
            sprite("Knight", anim, frames, p.position, p.facing,
                   dead     ? 0.35f
                   : attack ? p.actionTime / attackSpec(p.action, p.combo).duration * 1.5f
                            : clock,
                   tint, 1.5f, !dead && !attack);
            if (p.action == Action::Parry)
                DrawCircleLines(static_cast<int>(p.position.x), static_cast<int>(p.position.y) - 25,
                                35, p.actionTime <= 0.22f ? mint : muted);
            if (attack && p.strikeDone) {
                const auto spec = attackSpec(p.action, p.combo);
                const float angle = std::atan2(p.facing.y, p.facing.x) * 180 / PI;
                DrawRing(rv(p.position), spec.range - 7, spec.range, angle - 65, angle + 65, 24,
                         Fade(p.action == Action::Heavy ? gold : pale, 0.6f));
            }
        } else if (item.kind == 1) {
            const auto& wolf = w.wolf();
            sprite("Wolf",
                   wolf.rescued && distance(wolf.position, p.position) > 55 ? "Running" : "Idle", 4,
                   wolf.position, wolf.facing, clock, wolf.burst > 0 ? mint : WHITE, 1.3f);
            if (!wolf.rescued) {
                DrawCircleLines(static_cast<int>(wolf.position.x),
                                static_cast<int>(wolf.position.y), 40, mint);
                text("FIND ME", static_cast<int>(wolf.position.x) - 32,
                     static_cast<int>(wolf.position.y) - 70, 14, mint);
            }
        } else {
            const auto& e = w.enemies()[item.index];
            Color tint = e.kind == EnemyKind::Brute   ? Color{231, 154, 124, 255}
                         : e.kind == EnemyKind::Scout ? Color{166, 216, 193, 255}
                                                      : WHITE;
            if (e.flash > 0)
                tint = gold;
            const bool dead = e.state == EnemyState::Dead;
            const char* anim = dead                                        ? "Death"
                               : e.state == EnemyState::Windup             ? "Attack"
                               : e.state == EnemyState::Stunned            ? "Attacked"
                               : e.state == EnemyState::Chase && e.alerted ? "Running"
                                                                           : "Idle";
            sprite("Skeleton", anim, e.state == EnemyState::Windup ? 8 : 4, e.position, e.facing,
                   dead ? 0.8f - e.timer : e.age, dead ? Fade(tint, e.timer / 0.8f) : tint,
                   e.kind == EnemyKind::Brute ? 1.8f : 1.4f, !dead);
            if (!dead && (e.health < e.maxHealth || e.state == EnemyState::Windup))
                bar(static_cast<int>(e.position.x) - 24, static_cast<int>(e.position.y) - 88, 48, 4,
                    e.health, e.maxHealth, {222, 100, 91, 255});
            if (e.state == EnemyState::Stunned)
                text("*", static_cast<int>(e.position.x) - 4, static_cast<int>(e.position.y) - 100,
                     24, gold);
        }
    }
    for (const auto& ev : w.events()) {
        const float progress = 1 - ev.remaining / 0.7f;
        if (ev.kind == EventKind::Hit || ev.kind == EventKind::Hurt || ev.kind == EventKind::Heal)
            text(TextFormat("%s%d", ev.kind == EventKind::Heal ? "+" : "", ev.amount),
                 static_cast<int>(ev.position.x) - 10,
                 static_cast<int>(ev.position.y - 70 - progress * 35), 22,
                 Fade(ev.kind == EventKind::Hurt   ? Color{255, 120, 100, 255}
                      : ev.kind == EventKind::Heal ? mint
                                                   : pale,
                      1 - progress));
        else if (ev.kind == EventKind::Parry || ev.kind == EventKind::Wolf ||
                 ev.kind == EventKind::Rescue)
            text(ev.kind == EventKind::Parry  ? "PARRY!"
                 : ev.kind == EventKind::Wolf ? "WOLF STRIKE"
                                              : "REUNITED",
                 static_cast<int>(ev.position.x) - 50,
                 static_cast<int>(ev.position.y - 100 - progress * 20), 20, mint);
        if (ev.kind == EventKind::Hit || ev.kind == EventKind::Kill)
            for (int n = 0; n < 6; ++n) {
                const float angle = static_cast<float>(n) * PI / 3;
                DrawCircleV({ev.position.x + std::cos(angle) * progress * 45,
                             ev.position.y - 25 + std::sin(angle) * progress * 45},
                            3 * (1 - progress), gold);
            }
    }
    EndMode2D();
}
void Renderer::overlay(const World& w, bool showMap) {
    hud(w);
    map(w, showMap);
}
void Renderer::hud(const World& w) {
    const auto& p = w.player();
    DrawRectangle(20, 20, 340, 110, panelColor);
    text("KNIGHT / WOLF", 36, 32, 18, gold);
    text(TextFormat("%d / %d", static_cast<int>(p.health), static_cast<int>(p.maxHealth)), 245, 32,
         18, pale);
    bar(36, 60, 308, 14, p.health, p.maxHealth, {205, 88, 83, 255});
    bar(36, 82, 308, 8, p.stamina, 100, mint);
    text(TextFormat("POTIONS %d   |   SCORE %d", p.potions, w.score()), 36, 102, 16, muted);
    DrawRectangle(380, 20, 590, 86, panelColor);
    text(w.mode() == Mode::Survival ? TextFormat("ENDLESS HUNT  /  WAVE %d", w.wave())
                                    : stageName(w.stage()),
         398, 32, 20, gold);
    const char* objective =
        w.mode() == Mode::Survival ? "Survive. Each cleared wave restores health and a potion."
        : w.stage() == 0           ? "Clear the wildwood, then enter the eastern gate."
        : w.stage() == 1 ? "Find your wolf in the northeast. Escape through the east gate."
                         : "Defend your home. Defeat every skeleton in the village.";
    text(objective, 398, 62, 14, pale);
    text(TextFormat("%d ENEMIES LEFT", w.livingEnemies()), 398, 83, 14, muted);
    DrawRectangle(20, height - 62, width - 40, 42, panelColor);
    text("WASD  Move    J  Combo    K  Heavy    SPACE  Dodge    L  Parry    Q  Wolf    H  Heal    "
         "E  Gate",
         36, height - 49, 17, pale);
    text("TAB Map / ESC Pause", 1030, height - 26, 12, muted);
    if (w.wolf().rescued)
        text(w.wolf().cooldown > 0 ? TextFormat("WOLF  %.1fs", w.wolf().cooldown) : "WOLF  READY",
             36, 142, 18, w.wolf().cooldown > 0 ? muted : mint);
    if (p.comboWindow > 0 || p.action == Action::Light)
        text(TextFormat("COMBO %d / 3", p.combo), 36, 170, 18, gold);
    if (w.mode() == Mode::Campaign && w.stage() < 2 && distance(p.position, w.level().exit) < 100) {
        const bool ready = w.stage() == 0 ? w.livingEnemies() == 0 : w.wolf().rescued;
        DrawRectangle(400, 550, 480, 45, panelColor);
        text(ready            ? "E  /  CONTINUE YOUR JOURNEY"
             : w.stage() == 0 ? "Clear the remaining enemies first"
                              : "Find your wolf before leaving",
             425, 565, 20, ready ? mint : gold);
    }
    if (w.waveDelay() > 0)
        text(TextFormat("WAVE CLEARED  /  Next hunt in %.0f", std::ceil(w.waveDelay())), 430, 145,
             22, mint);
}
void Renderer::map(const World& w, bool large) {
    const auto& l = w.level();
    const float scale =
        large
            ? std::min(800.0f / static_cast<float>(l.width), 450.0f / static_cast<float>(l.height))
            : std::min(250.0f / static_cast<float>(l.width), 150.0f / static_cast<float>(l.height));
    const float ox = large ? (width - static_cast<float>(l.width) * scale) / 2 : 1004;
    const float oy = large ? 150 : 22;
    if (large)
        DrawRectangle(0, 0, width, height, Fade(ink, 0.8f));
    DrawRectangle(static_cast<int>(ox) - 8, static_cast<int>(oy) - 8,
                  static_cast<int>(static_cast<float>(l.width) * scale) + 16,
                  static_cast<int>(static_cast<float>(l.height) * scale) + 16, panelColor);
    for (int y = 0; y < l.height; ++y)
        for (int x = 0; x < l.width; ++x)
            DrawRectangle(static_cast<int>(ox + static_cast<float>(x) * scale),
                          static_cast<int>(oy + static_cast<float>(y) * scale),
                          static_cast<int>(std::ceil(scale)), static_cast<int>(std::ceil(scale)),
                          l.solid(x, y) ? Color{36, 52, 59, 255} : Color{76, 92, 87, 255});
    auto marker = [&](Vec2 p, Color c, float r) {
        DrawCircleV({ox + p.x / 64 * scale, oy + p.y / 64 * scale}, r, c);
    };
    for (const auto& e : w.enemies())
        if (e.state != EnemyState::Dead)
            marker(e.position, {244, 114, 91, 255}, large ? 4.0f : 2.0f);
    if (w.mode() == Mode::Campaign && w.stage() < 2)
        marker(l.exit, gold, large ? 7.0f : 4.0f);
    marker(w.wolf().position, mint, large ? 6.0f : 3.0f);
    marker(w.player().position, pale, large ? 6.0f : 3.0f);
    if (large)
        text("YOU / WHITE      WOLF / GREEN      ENEMIES / RED      GATE / GOLD", 290, 630, 18,
             pale);
}
void Renderer::menu(float clock, int best) {
    ClearBackground(ink);
    const auto bg = assets_.texture("MenuBackground.png");
    DrawTexturePro(bg, {0, 0, static_cast<float>(bg.width), static_cast<float>(bg.height)},
                   {0, 0, width, height}, {0, 0}, 0, Fade(WHITE, 0.52f));
    DrawRectangleGradientH(0, 0, width, height, {9, 17, 23, 250}, {9, 17, 23, 100});
    text("A BOND STRONGER THAN STEEL", 82, 102, 18, mint);
    text("KNIGHT WOLF'S", 78, 154, 64, pale);
    text("ADVENTURE", 78, 226, 72, gold);
    text("Into the wildwood. Through the hollow keep. Home together.", 84, 328, 20, muted);
    DrawRectangle(82, 390, 470, 62, panelColor);
    text("ENTER   Begin the journey", 104, 410, 24, pale);
    DrawRectangle(82, 466, 470, 62, panelColor);
    text("S   Endless hunt", 104, 486, 24, pale);
    text(TextFormat("SURVIVAL BEST  %d", best), 84, 552, 18, mint);
    text("F1  Field guide     F11  Fullscreen     M  Sound     ESC  Quit", 84, 650, 18, muted);
    sprite("Knight", "Idle", 8, {920, 540}, {-1, 0}, clock, WHITE, 5);
    sprite("Wolf", "Idle", 4, {1080, 565}, {-1, 0}, clock, WHITE, 3);
    DrawLine(82, 360, 550, 360, Fade(gold, 0.4f));
}
void Renderer::panel(const char* eyebrow, const char* title, const char* body, const char* footer) {
    DrawRectangle(0, 0, width, height, Fade(ink, 0.85f));
    DrawRectangle(230, 135, 820, 450, panelColor);
    DrawRectangle(230, 135, 4, 450, gold);
    text(eyebrow, 270, 170, 18, mint);
    text(title, 270, 212, 40, pale);
    text(body, 270, 285, 19, muted);
    text(footer, 270, 535, 20, gold);
}
} // namespace kwa

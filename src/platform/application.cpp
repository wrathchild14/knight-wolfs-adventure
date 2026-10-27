#include "application.hpp"
#include "post_effects.hpp"
#include <algorithm>
#include <cstdlib>
#include <fstream>
#include <iostream>
#include <string>
namespace kwa {
namespace {
struct Window {
    Window() {
        SetConfigFlags(FLAG_WINDOW_RESIZABLE | FLAG_VSYNC_HINT);
        InitWindow(Renderer::width, Renderer::height, "Knight Wolf's Adventure");
        SetWindowMinSize(800, 450);
        SetExitKey(KEY_NULL);
        SetTargetFPS(120);
    }
    ~Window() { CloseWindow(); }
};
struct Canvas {
    RenderTexture2D target = LoadRenderTexture(Renderer::width, Renderer::height);
    ~Canvas() { UnloadRenderTexture(target); }
};
class Audio {
  public:
    Audio() {
        InitAudioDevice();
        available_ = IsAudioDeviceReady();
    }
    ~Audio() {
        if (hit_.frameCount)
            UnloadSound(hit_);
        if (music_.ctxData)
            UnloadMusicStream(music_);
        if (available_)
            CloseAudioDevice();
    }
    void track(const std::filesystem::path& file) {
        if (!available_ || file == path_)
            return;
        if (music_.ctxData)
            UnloadMusicStream(music_);
        music_ = LoadMusicStream(file.string().c_str());
        path_ = file;
        if (music_.ctxData) {
            music_.looping = true;
            SetMusicVolume(music_, muted_ ? 0 : 0.35f);
            PlayMusicStream(music_);
        }
    }
    void update() {
        if (music_.ctxData)
            UpdateMusicStream(music_);
    }
    void effects(const World& world, const std::filesystem::path& root) {
        if (!available_)
            return;
        if (!hit_.frameCount)
            hit_ = LoadSound((root / "content/Sounds/punch.wav").string().c_str());
        bool impact = false;
        for (const auto& event : world.events())
            if (event.serial > lastEvent_) {
                lastEvent_ = event.serial;
                impact |= event.kind == EventKind::Hit || event.kind == EventKind::Hurt ||
                          event.kind == EventKind::Parry;
            }
        if (impact && hit_.frameCount && !muted_) {
            SetSoundVolume(hit_, 0.45f);
            PlaySound(hit_);
        }
    }
    void toggle() {
        muted_ = !muted_;
        if (music_.ctxData)
            SetMusicVolume(music_, muted_ ? 0 : 0.35f);
    }

  private:
    Sound hit_{};
    std::uint64_t lastEvent_{};
    Music music_{};
    bool available_{}, muted_{};
    std::filesystem::path path_;
};
std::filesystem::path scorePath() {
#ifdef _WIN32
    if (const char* base = std::getenv("LOCALAPPDATA"))
        return std::filesystem::path(base) / "KnightWolf" / "best.txt";
#else
    if (const char* base = std::getenv("XDG_DATA_HOME"))
        return std::filesystem::path(base) / "knight-wolf" / "best.txt";
    if (const char* base = std::getenv("HOME"))
        return std::filesystem::path(base) / ".local/share/knight-wolf/best.txt";
#endif
    return std::filesystem::temp_directory_path() / "knight-wolf-best.txt";
}
int readBest(const std::filesystem::path& path) {
    int score = 0;
    std::ifstream file(path);
    if (!(file >> score) || score < 0)
        return 0;
    return score;
}
void writeBest(const std::filesystem::path& path, int score) {
    std::error_code error;
    std::filesystem::create_directories(path.parent_path(), error);
    if (error) {
        std::cerr << "Cannot save survival score: " << error.message() << '\n';
        return;
    }
    std::ofstream file(path);
    if (!(file << score << '\n'))
        std::cerr << "Cannot save survival score\n";
}
Input controls() {
    Input i;
    i.move = {static_cast<float>(IsKeyDown(KEY_D) || IsKeyDown(KEY_RIGHT)) -
                  static_cast<float>(IsKeyDown(KEY_A) || IsKeyDown(KEY_LEFT)),
              static_cast<float>(IsKeyDown(KEY_S) || IsKeyDown(KEY_DOWN)) -
                  static_cast<float>(IsKeyDown(KEY_W) || IsKeyDown(KEY_UP))};
    i.light = IsKeyPressed(KEY_J) || IsMouseButtonPressed(MOUSE_BUTTON_LEFT);
    i.heavy = IsKeyPressed(KEY_K) || IsMouseButtonPressed(MOUSE_BUTTON_RIGHT);
    i.dodge = IsKeyPressed(KEY_SPACE);
    i.parry = IsKeyPressed(KEY_L);
    i.wolf = IsKeyPressed(KEY_Q);
    i.heal = IsKeyPressed(KEY_H);
    i.interact = IsKeyPressed(KEY_E);
    return i;
}
void mergeEdges(Input& pending, const Input& next) {
    pending.move = next.move;
    pending.light |= next.light;
    pending.heavy |= next.heavy;
    pending.dodge |= next.dodge;
    pending.parry |= next.parry;
    pending.wolf |= next.wolf;
    pending.heal |= next.heal;
    pending.interact |= next.interact;
}
const char* levelFile(int stage) {
    return stage == 0 ? "forest.level" : stage == 1 ? "dungeon.level" : "village.level";
}
} // namespace
int run(const Options& options) {
    Window window;
    const auto root = options.assets.empty()
                          ? std::filesystem::path(GetApplicationDirectory()) / "assets"
                          : options.assets;
    if (!std::filesystem::exists(root / "levels/forest.level"))
        throw std::runtime_error(
            "Assets missing beside the executable. Use --assets DIRECTORY or rebuild the game.");
    Assets assets(root);
    Renderer renderer(assets);
    Canvas canvas;
    PostEffects effects(root);
    if (options.noEffects)
        effects.disable();
    if (options.smokeFrames > 0 && !options.noEffects && !effects.ready())
        throw std::runtime_error("Shader compilation failed during graphics smoke test");
    Audio audio;
    World world;
    enum class Screen { Menu, Playing, Paused, Guide, Upgrade, End };
    Screen screen = Screen::Menu, guideReturn = Screen::Menu;
    int best = readBest(scorePath());
    int frames = 0;
    float clock = 0, accumulator = 0;
    constexpr float step = 1.0f / 120.0f;
    Input pending;
    auto music = [&](int stage) {
        audio.track(root / "content/Sounds" /
                    (stage == 0   ? "Forest.wav"
                     : stage == 1 ? "Dungeon.wav"
                                  : "Town.wav"));
    };
    auto start = [&](Mode mode) {
        world.start(Level::load(root / "levels" /
                                (mode == Mode::Campaign ? "forest.level" : "survival.level")),
                    mode);
        music(0);
        screen = Screen::Playing;
        accumulator = 0;
        pending = {};
    };
    audio.track(root / "content/Sounds/Forest.wav");
    if (options.smokeFrames > 0) {
        // Exercise every source asset without relying on a particular camera view.
        for (const char* file :
             {"forest.level", "dungeon.level", "village.level", "survival.level"}) {
            const auto level = Level::load(root / "levels" / file);
            for (int tile : level.tiles)
                assets.texture("Tiles/" + level.theme + "/Tile" + std::to_string(tile) + ".png");
        }
        for (const char* anim : {"Idle", "Running", "Attack", "Pray", "Death"})
            assets.texture(std::string("Sprites/Knight/Knight") + anim + ".png");
        for (const char* anim : {"Idle", "Running", "Attack", "Attacked", "Death"})
            assets.texture(std::string("Sprites/Skeleton/Skeleton") + anim + ".png");
        assets.texture("Sprites/Wolf/WolfRunning.png");
        assets.texture("Sprites/Wolf/WolfIdle.png");
    }
    if (options.smokePlay) {
        start(Mode::Campaign);
        if (options.smokeStage > 0) {
            world.start(Level::load(root / "levels" / levelFile(options.smokeStage)),
                        Mode::Campaign, options.smokeStage);
            music(options.smokeStage);
        }
    }
    while (!WindowShouldClose()) {
        const float dt = options.smokeFrames > 0 ? 1.0f / 60.0f : std::min(GetFrameTime(), 0.1f);
        clock += dt;
        audio.update();
        if (IsKeyPressed(KEY_F11))
            ToggleFullscreen();
        if (IsKeyPressed(KEY_F2))
            effects.cycle();
        if (IsKeyPressed(KEY_M))
            audio.toggle();
        if (!IsWindowFocused() && screen == Screen::Playing && options.smokeFrames == 0)
            screen = Screen::Paused;
        const bool escape = IsKeyPressed(KEY_ESCAPE), enter = IsKeyPressed(KEY_ENTER);
        const float viewScale = std::min(static_cast<float>(GetScreenWidth()) / Renderer::width,
                                         static_cast<float>(GetScreenHeight()) / Renderer::height);
        const Vector2 pointer{
            (GetMouseX() - (GetScreenWidth() - Renderer::width * viewScale) / 2) / viewScale,
            (GetMouseY() - (GetScreenHeight() - Renderer::height * viewScale) / 2) / viewScale};
        const bool click = IsMouseButtonPressed(MOUSE_BUTTON_LEFT);
        if (screen == Screen::Menu) {
            if (escape)
                break;
            if (enter || (click && CheckCollisionPointRec(pointer, {82, 390, 470, 62})))
                start(Mode::Campaign);
            else if (IsKeyPressed(KEY_S) ||
                     (click && CheckCollisionPointRec(pointer, {82, 466, 470, 62})))
                start(Mode::Survival);
            else if (IsKeyPressed(KEY_F1)) {
                guideReturn = screen;
                screen = Screen::Guide;
            }
        } else if (screen == Screen::Guide) {
            if (escape || enter || IsKeyPressed(KEY_F1))
                screen = guideReturn;
        } else if (screen == Screen::Paused) {
            if (escape || enter) {
                screen = Screen::Playing;
                pending = {};
                accumulator = 0;
            } else if (IsKeyPressed(KEY_F1)) {
                guideReturn = screen;
                screen = Screen::Guide;
            } else if (IsKeyPressed(KEY_BACKSPACE))
                screen = Screen::Menu;
        } else if (screen == Screen::Upgrade) {
            int choice = IsKeyPressed(KEY_ONE)     ? 1
                         : IsKeyPressed(KEY_TWO)   ? 2
                         : IsKeyPressed(KEY_THREE) ? 3
                                                   : 0;
            if (choice) {
                world.upgrade(choice);
                const int next = world.stage() + 1;
                world.start(Level::load(root / "levels" / levelFile(next)), Mode::Campaign, next,
                            true);
                music(next);
                screen = Screen::Playing;
                pending = {};
                accumulator = 0;
            } else if (escape)
                screen = Screen::Menu;
        } else if (screen == Screen::End) {
            if (escape)
                screen = Screen::Menu;
            else if (enter) {
                // Restart the current chapter; fresh combat resources prevent a death spiral.
                if (world.mode() == Mode::Campaign && world.outcome() == Outcome::Defeat) {
                    const int stage = world.stage();
                    world.start(Level::load(root / "levels" / levelFile(stage)), Mode::Campaign,
                                stage);
                    music(stage);
                    screen = Screen::Playing;
                    accumulator = 0;
                    pending = {};
                } else
                    start(world.mode());
            }
        } else if (screen == Screen::Playing) {
            if (escape) {
                screen = Screen::Paused;
                pending = {};
                accumulator = 0;
            } else if (IsKeyPressed(KEY_F1)) {
                guideReturn = screen;
                screen = Screen::Guide;
                pending = {};
                accumulator = 0;
            } else {
                mergeEdges(pending, controls());
                accumulator += dt;
                while (accumulator >= step) {
                    world.tick(pending, step);
                    pending.clearEdges();
                    accumulator -= step;
                }
                audio.effects(world, root);
                if (world.mode() == Mode::Survival && world.score() > best)
                    best = world.score();
                if (world.outcome() == Outcome::StageClear)
                    screen = Screen::Upgrade;
                else if (world.outcome() == Outcome::Defeat ||
                         world.outcome() == Outcome::Victory) {
                    screen = Screen::End;
                    if (options.smokeFrames == 0)
                        writeBest(scorePath(), best);
                    audio.track(root / "content/Sounds" /
                                (world.outcome() == Outcome::Victory ? "Win.wav" : "end.wav"));
                }
            }
        }
        const bool atTitle =
            screen == Screen::Menu || (screen == Screen::Guide && guideReturn == Screen::Menu);
        if (!atTitle) {
            effects.begin(world, Renderer::camera(world), clock);
            renderer.world(world, clock);
            effects.end();
        }
        BeginTextureMode(canvas.target);
        if (atTitle)
            renderer.menu(clock, best);
        else {
            ClearBackground(BLACK);
            effects.composite();
            renderer.overlay(world, IsKeyDown(KEY_TAB));
            renderer.text(effects.label(), 36, Renderer::height - 24, 12, {150, 169, 175, 255});
        }
        if (screen == Screen::Guide)
            renderer.panel(
                "FIELD GUIDE", "Fight with intent",
                "Move with WASD or arrows. Your movement sets attack direction.\nJ / left click: "
                "chain three cuts. Tap again near the end of a swing.\nK / right click: heavy "
                "cleave. SPACE: dodge through danger.\nL: face a strike and parry just before it "
                "lands (green window).\nQ: your wolf stuns a nearby foe. H: drink a healing "
                "potion.\nRed circles warn of attacks. Step away during the windup.\nRest between "
                "attacks to recover stamina. TAB shows the map.",
                "ENTER / ESC  Back");
        if (screen == Screen::Paused)
            renderer.panel("TAKE A BREATH", "Journey paused",
                           "ENTER / ESC    Resume\nF1                     Field guide\nBACKSPACE   "
                           "    Return to title\n\nLeaving a run discards its current progress.",
                           "F2  Cycle effects     M  Sound     F11  Fullscreen");
        if (screen == Screen::Upgrade)
            renderer.panel(
                "CHAPTER COMPLETE", "Rest at the waystone",
                TextFormat("%s\nYour health is restored. Choose a gift for the road ahead.\n\n1    "
                           "Tempered steel       +6 sword damage\n2    Stalwart heart          +25 "
                           "maximum health\n3    Traveler's reserve    +2 healing potions",
                           world.stage() == 0
                               ? "Your wolf follows a howl into the keep. Bring him home."
                               : "Together again. But the village bells ring under siege."),
                "1 / 2 / 3  Choose and continue     ESC  Title");
        if (screen == Screen::End)
            renderer.panel(
                world.outcome() == Outcome::Victory ? "HOME, TOGETHER" : "THE HUNT IS NOT OVER",
                world.outcome() == Outcome::Victory ? "The village is safe" : "You fell in battle",
                TextFormat("Score  %d     Enemies defeated  %d\n\n%s", world.score(), world.kills(),
                           world.outcome() == Outcome::Victory
                               ? "You brought your companion home and broke the siege."
                               : "Watch the windup. Save stamina for your escape.\nYour next "
                                 "attempt begins with fresh supplies."),
                "ENTER  Try again     ESC  Title");
        EndTextureMode();
        BeginDrawing();
        ClearBackground(BLACK);
        const float scale = std::min(static_cast<float>(GetScreenWidth()) / Renderer::width,
                                     static_cast<float>(GetScreenHeight()) / Renderer::height);
        DrawTexturePro(canvas.target.texture, {0, 0, Renderer::width, -Renderer::height},
                       {(static_cast<float>(GetScreenWidth()) - Renderer::width * scale) / 2,
                        (static_cast<float>(GetScreenHeight()) - Renderer::height * scale) / 2,
                        Renderer::width * scale, Renderer::height * scale},
                       {0, 0}, 0, WHITE);
        EndDrawing();
        ++frames;
        if (options.smokeFrames > 0 && frames >= options.smokeFrames) {
            if (!options.screenshot.empty()) {
                Image shot = LoadImageFromScreen();
                const bool saved = ExportImage(shot, options.screenshot.string().c_str());
                UnloadImage(shot);
                if (!saved)
                    throw std::runtime_error("Could not write screenshot");
            }
            break;
        }
    }
    if (options.smokeFrames == 0)
        writeBest(scorePath(), best);
    return 0;
}
} // namespace kwa

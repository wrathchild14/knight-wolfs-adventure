#include "post_effects.hpp"
#include "renderer.hpp"
#include <algorithm>
#include <cmath>
#include <rlgl.h>
#include <stdexcept>
#include <vector>
namespace kwa {
PostEffects::PostEffects(const std::filesystem::path& root) {
    scene_ = LoadRenderTexture(Renderer::width, Renderer::height);
    if (!IsRenderTextureValid(scene_))
        throw std::runtime_error("Cannot allocate world render target");
    const auto path = root / "shaders/atmosphere.fs";
    shader_ = LoadShader(nullptr, path.string().c_str());
    // raylib returns its default shader on compilation failure. Never unload it.
    ready_ = shader_.id != 0 && shader_.id != rlGetShaderIdDefault();
    if (!ready_) {
        TraceLog(LOG_WARNING, "Atmosphere unavailable; using unfiltered rendering");
        return;
    }
    resolutionLoc_ = GetShaderLocation(shader_, "resolution");
    originLoc_ = GetShaderLocation(shader_, "worldOrigin");
    timeLoc_ = GetShaderLocation(shader_, "clock");
    themeLoc_ = GetShaderLocation(shader_, "theme");
    strengthLoc_ = GetShaderLocation(shader_, "strength");
    hurtLoc_ = GetShaderLocation(shader_, "hurt");
    pulseLoc_ = GetShaderLocation(shader_, "pulse");
    lightsLoc_ = GetShaderLocation(shader_, "lights[0]");
    const Vector2 resolution{Renderer::width, Renderer::height};
    SetShaderValue(shader_, resolutionLoc_, &resolution, SHADER_UNIFORM_VEC2);
}
PostEffects::~PostEffects() {
    if (ready_)
        UnloadShader(shader_);
    UnloadRenderTexture(scene_);
}
const char* PostEffects::label() const {
    if (!ready_)
        return "F2  FX UNAVAILABLE";
    return quality_ == 0 ? "F2  FX CINEMATIC" : quality_ == 1 ? "F2  FX SOFT" : "F2  FX OFF";
}
void PostEffects::begin(const World& world, Camera2D camera, float time) {
    camera_ = camera;
    time_ = time;
    lights_ = {};
    auto light = [&](int index, Vec2 p, float radius, float cool) {
        const auto screen = GetWorldToScreen2D({p.x, p.y}, camera);
        lights_[static_cast<std::size_t>(index)] = {screen.x, screen.y, radius, cool};
    };
    light(0, world.player().position, 245, 0);
    light(1, world.wolf().position, 155, 1);
    light(2, world.level().exit, 180, 1);
    if (world.level().theme == "Dungeon") {
        std::vector<Vec2> torches;
        const auto& level = world.level();
        for (int y = 1; y < level.height; ++y)
            for (int x = 0; x < level.width; ++x)
                if (!level.solid(x, y) && level.solid(x, y - 1) && (x + 2 * y) % 3 == 0)
                    torches.push_back(
                        {(static_cast<float>(x) + 0.5f) * 64, static_cast<float>(y) * 64 + 12});
        std::sort(torches.begin(), torches.end(), [&](Vec2 a, Vec2 b) {
            return distance(a, world.player().position) < distance(b, world.player().position);
        });
        for (std::size_t i = 0; i < std::min<std::size_t>(5, torches.size()); ++i)
            light(static_cast<int>(i) + 3, torches[i], 160, 0);
    }
    if (ready_) {
        const Vector2 origin{camera.target.x - camera.offset.x, camera.target.y - camera.offset.y};
        const float theme = world.level().theme == "Dungeon" ? 1.0f
                            : world.level().theme == "Town"  ? 2.0f
                                                             : 0.0f;
        const float strength = quality_ == 0 ? 1.0f : quality_ == 1 ? 0.45f : 0.0f;
        float hurt = 0;
        Vector4 pulse{};
        for (const auto& event : world.events()) {
            if (event.kind == EventKind::Hurt)
                hurt = std::max(hurt, event.remaining / 0.7f);
            if (event.kind == EventKind::Parry || event.kind == EventKind::Wolf) {
                const auto p = GetWorldToScreen2D({event.position.x, event.position.y}, camera);
                pulse = {p.x, p.y, 0.7f - event.remaining, 1};
            }
        }
        SetShaderValue(shader_, originLoc_, &origin, SHADER_UNIFORM_VEC2);
        SetShaderValue(shader_, timeLoc_, &time, SHADER_UNIFORM_FLOAT);
        SetShaderValue(shader_, themeLoc_, &theme, SHADER_UNIFORM_FLOAT);
        SetShaderValue(shader_, strengthLoc_, &strength, SHADER_UNIFORM_FLOAT);
        SetShaderValue(shader_, hurtLoc_, &hurt, SHADER_UNIFORM_FLOAT);
        SetShaderValue(shader_, pulseLoc_, &pulse, SHADER_UNIFORM_VEC4);
        SetShaderValueV(shader_, lightsLoc_, lights_.data(), SHADER_UNIFORM_VEC4, 8);
    }
    BeginTextureMode(scene_);
}
void PostEffects::end() {
    // Physical sconces match the lighting sources, including when effects are off.
    for (std::size_t i = 3; i < lights_.size(); ++i) {
        const auto& l = lights_[i];
        if (l.z == 0)
            continue;
        DrawRectangle(static_cast<int>(l.x) - 3, static_cast<int>(l.y), 6, 15, {54, 36, 30, 255});
        DrawCircleV({l.x, l.y}, 5 + std::sin(time_ * 8 + static_cast<float>(i)),
                    {239, 145, 56, 255});
        DrawCircleV({l.x, l.y - 2}, 2, {255, 234, 153, 255});
    }
    EndTextureMode();
}
void PostEffects::composite() const {
    if (ready_ && quality_ != 2)
        BeginShaderMode(shader_);
    DrawTexturePro(scene_.texture, {0, 0, Renderer::width, -Renderer::height},
                   {0, 0, Renderer::width, Renderer::height}, {0, 0}, 0, WHITE);
    if (ready_ && quality_ != 2)
        EndShaderMode();
}
} // namespace kwa

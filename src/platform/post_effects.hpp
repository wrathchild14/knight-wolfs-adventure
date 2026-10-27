#pragma once
#include "kwa/world.hpp"
#include <array>
#include <filesystem>
#include <raylib.h>
namespace kwa {
// Owns a world-only render target. UI is composited after this pass.
class PostEffects {
  public:
    explicit PostEffects(const std::filesystem::path& root);
    ~PostEffects();
    PostEffects(const PostEffects&) = delete;
    PostEffects& operator=(const PostEffects&) = delete;
    void begin(const World& world, Camera2D camera, float time);
    void end();
    void composite() const;
    void cycle() { quality_ = (quality_ + 1) % 3; }
    void disable() { quality_ = 2; }
    bool ready() const { return ready_; }
    const char* label() const;

  private:
    RenderTexture2D scene_{};
    Shader shader_{};
    bool ready_{};
    int quality_{};
    int resolutionLoc_{}, originLoc_{}, timeLoc_{}, themeLoc_{}, strengthLoc_{}, hurtLoc_{},
        pulseLoc_{}, lightsLoc_{};
    std::array<Vector4, 8> lights_{};
    Camera2D camera_{};
    float time_{};
};
} // namespace kwa

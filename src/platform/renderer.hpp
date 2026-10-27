#pragma once
#include "assets.hpp"
#include "kwa/world.hpp"
namespace kwa {
class Renderer {
  public:
    static constexpr int width = 1280, height = 720;
    explicit Renderer(Assets& assets) : assets_(assets) {}
    static Camera2D camera(const World& world);
    void world(const World& world, float clock);
    void overlay(const World& world, bool showMap);
    void menu(float clock, int best);
    void panel(const char* eyebrow, const char* title, const char* body, const char* footer);
    void text(const char* value, int x, int y, int size, Color color) const;

  private:
    Assets& assets_;
    void sprite(const std::string& family, const std::string& animation, int frames, Vec2 p,
                Vec2 facing, float time, Color tint, float scale = 1.5f, bool loop = true);
    void hud(const World& world);
    void map(const World& world, bool large);
};
} // namespace kwa

#include "assets.hpp"
#include <stdexcept>
namespace kwa {
Assets::~Assets() {
    for (const auto& [name, texture] : textures_)
        UnloadTexture(texture);
}
Texture2D Assets::texture(const std::string& name) {
    if (auto it = textures_.find(name); it != textures_.end())
        return it->second;
    const auto path = root_ / "content" / name;
    auto t = LoadTexture(path.string().c_str());
    if (t.id == 0)
        throw std::runtime_error("Cannot load texture: " + path.string());
    SetTextureFilter(t, TEXTURE_FILTER_POINT);
    textures_.emplace(name, t);
    return t;
}
} // namespace kwa

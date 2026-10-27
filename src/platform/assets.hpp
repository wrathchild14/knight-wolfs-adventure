#pragma once
#include <filesystem>
#include <raylib.h>
#include <string>
#include <unordered_map>
namespace kwa {
class Assets {
  public:
    explicit Assets(std::filesystem::path root) : root_(std::move(root)) {}
    ~Assets();
    Assets(const Assets&) = delete;
    Assets& operator=(const Assets&) = delete;
    Texture2D texture(const std::string& name);
    const std::filesystem::path& root() const { return root_; }

  private:
    std::filesystem::path root_;
    std::unordered_map<std::string, Texture2D> textures_;
};
} // namespace kwa

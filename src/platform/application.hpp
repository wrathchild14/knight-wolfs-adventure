#pragma once
#include "renderer.hpp"
namespace kwa {
struct Options {
    std::filesystem::path assets;
    int smokeFrames{};
    std::filesystem::path screenshot;
    bool smokePlay{};
};
int run(const Options& options);
} // namespace kwa

#include "application.hpp"
#include <iostream>
#include <stdexcept>
#include <string>
int main(int argc, char** argv) {
    try {
        kwa::Options options;
        for (int i = 1; i < argc; ++i) {
            const std::string arg = argv[i];
            if (arg == "--assets" && i + 1 < argc)
                options.assets = argv[++i];
            else if (arg == "--smoke-test" && i + 1 < argc)
                options.smokeFrames = std::stoi(argv[++i]);
            else if (arg == "--screenshot" && i + 1 < argc)
                options.screenshot = argv[++i];
            else if (arg == "--no-effects")
                options.noEffects = true;
            else if (arg == "--smoke-stage" && i + 1 < argc) {
                options.smokeStage = std::stoi(argv[++i]);
                if (options.smokeStage < 0 || options.smokeStage > 2)
                    throw std::runtime_error("Smoke stage must be 0, 1 or 2");
                options.smokePlay = true;
            } else if (arg == "--smoke-play")
                options.smokePlay = true;
            else if (arg == "--help") {
                std::cout << "Knight Wolf's Adventure\n  --assets DIR\n  --smoke-test FRAMES "
                             "[--smoke-play] [--smoke-stage 0|1|2] [--screenshot FILE.png]\n  "
                             "--no-effects\n";
                return 0;
            } else
                throw std::runtime_error("Unknown or incomplete option: " + arg);
        }
        return kwa::run(options);
    } catch (const std::exception& e) {
        std::cerr << "Knight Wolf: " << e.what() << '\n';
        return 1;
    }
}

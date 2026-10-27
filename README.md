# Knight Wolf's Adventure — C++ edition

A native C++20 action adventure for **Linux and Windows**, built with raylib 5.5. Journey through the wildwood, rescue your wolf from the hollow keep, and defend your village. The original MonoGame source and artwork remain in `Project1/`; the active game is the CMake project at the repository root.

## Play the prepared builds

If supplied with this checkout, open `dist/linux/knight_wolf` on this Linux machine, or copy the **entire** `dist/windows` directory to Windows and launch `knight_wolf.exe`. Keep the adjacent `assets` folder. These output folders are deliberately excluded from Git.

The Linux executable targets the system on which it was built; build from source on older Linux distributions. The Windows build targets x64 and statically includes the MinGW C++ runtime. Both require a working OpenGL 3.3 graphics driver. Linux uses X11 or XWayland. Audio is optional; the game continues without an audio device.

## Build on Linux

On Ubuntu / Debian, install the development tools once:

```sh
sudo apt-get update
sudo apt-get install build-essential cmake ninja-build libasound2-dev libx11-dev libxrandr-dev libxi-dev libgl1-mesa-dev libxcursor-dev libxinerama-dev
cmake -S . -B build -G Ninja -DCMAKE_BUILD_TYPE=Release
cmake --build build --parallel
ctest --test-dir build --output-on-failure
./build/knight_wolf
```

## Build on Windows

Install Visual Studio 2022 with **Desktop development with C++** and CMake. In a Developer PowerShell at the repository root:

```powershell
cmake -S . -B build -A x64
cmake --build build --config Release --parallel
ctest --test-dir build -C Release --output-on-failure
.\build\Release\knight_wolf.exe
```

CMake finds an installed raylib 5.5+ package or downloads the pinned, SHA-256-verified 5.5 source archive. The first build therefore needs internet access. To build offline, supply a local source checkout with `-DFETCHCONTENT_SOURCE_DIR_RAYLIB=/path/to/raylib-5.5`. CMake 3.22+ and a C++20 compiler are required. No .NET, MonoGame content compiler, or Python is needed.

## Controls

| Input | Action |
| --- | --- |
| WASD / arrow keys | Move and face the attack direction |
| J / left mouse | Light attack; tap near the end of a swing to chain up to three cuts |
| K / right mouse | Heavy cleave with longer windup and stronger stagger |
| Space | Dodge, with a short invulnerability window; costs 25 stamina |
| L | Parry a frontal strike during the first 0.22 seconds; rewards stamina and staggers the attacker |
| Q | Wolf strike: nearby visible enemy, five-second cooldown; available after rescue in the dungeon |
| H | Use one healing potion when injured |
| E | Enter an unlocked gate |
| Tab (hold) | Full map |
| Esc | Pause / resume; title navigation is shown on screen |
| F1 | Field guide |
| F11 | Fullscreen |
| M | Toggle sound |

The title screen supports keyboard and mouse. The game pauses when it loses focus. Window resizing preserves the 16:9 view with letterboxing.

## What's new

- Timed attacks with one damage application per swing, directional hit checks, three-hit combos, brief input buffering, stamina management, and recovery cancels into dodges.
- Three skeleton types: fast scouts, soldiers, and slow brutes. Visible windups, committed attack directions, recovery windows, knockback, stagger, and collision-aware pursuit make fights readable.
- Wolf attack command, hit sounds, damage numbers, impact particles, and parry feedback.
- Original forest / dungeon / town tile layouts, with two earlier forest encounters, a rescue objective, usable gates, stage upgrades, and a victory state.
- Survival waves grow from six enemies to a cap of 32 per wave. Spawns stay away from the player; clearing a wave grants health, a potion, and four seconds of rest.
- Chapter retries after death, a minimap, a full map, a field guide, and a saved survival best score.

Campaign progress is session-only. Returning to the title discards the current run; death retries the current chapter with fresh base equipment and score. Upgrades carry forward during an uninterrupted campaign. Survival high scores are stored in `$XDG_DATA_HOME/knight-wolf` (or `~/.local/share/knight-wolf`) on Linux and `%LOCALAPPDATA%\KnightWolf` on Windows.

## Tests and packaging

The simulation has no graphics dependencies. To run only its tests:

```sh
cmake --preset core-tests
cmake --build --preset core-tests
ctest --preset core-tests
```

Tests cover collision and navigation, damage, attack direction, parry and dodge timing, potion use, companion cooldowns, map reachability, and progression. A deterministic input-driven player completes the entire campaign and three survival waves using the same gameplay API as the real application.

```sh
# Brief graphical check; also loads every used tile and sprite asset.
./build/knight_wolf --smoke-test 60 --smoke-play --screenshot /tmp/knight-wolf.png
# Copy a self-contained game directory (use --config Release on Windows).
cmake --install build --prefix dist/linux
# Alternatively create a ZIP from the build directory.
cd build
cpack -C Release
```

The smoke test needs a display; on headless Linux, use `xvfb-run -a`. `--assets DIRECTORY` overrides the executable-relative asset folder. GitHub Actions is configured to compile and test Linux and Windows and upload packaged artifacts; this workflow runs when pushed.

For the hosted builds, see [GitHub build setup](docs/GITHUB_BUILDS.md).

See [architecture](docs/ARCHITECTURE.md) and [asset / library notes](docs/THIRD_PARTY.md).

Original MonoGame release: [Knight Wolf's Adventure on itch.io](https://wrathchild14.itch.io/knight-wolfs-adventure).

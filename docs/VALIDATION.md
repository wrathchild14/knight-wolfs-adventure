# Validation — 27 October 2026

- Linux release built with GCC 15.2 and raylib 5.5.
- Windows x64 release cross-compiled with MinGW GCC 13.2; executable import inspection found only Windows system DLLs, with no MinGW runtime DLL requirement.
- 119 gameplay / map checks passed in the headless test executable.
- Input-driven integration test completed all three campaign chapters (32 defeated enemies) and cleared three survival waves, reaching wave four.
- Both test executables passed again with undefined-behavior instrumentation enabled.
- Linux title screen and gameplay ran on an X11 virtual display using Mesa software OpenGL. The packaged executable launched from `/tmp`, loaded every used tile and sprite resource, and exited normally. Screenshots are in `docs/screenshots`.
- C++ formatting checks and Git whitespace checks passed.

Windows has been compiled but has not been run on a native Windows desktop. The GitHub Actions Linux / Windows workflow is supplied but has not been executed remotely during this task. Graphical smoke tests verify startup, asset loading and rendering; the full campaign simulation verifies gameplay progression, not a manual playthrough of every screen. Campaign balance remains open to playtesting preferences.

Build tools were downloaded and extracted into a temporary directory for this task; no system packages were installed. The prepared release folders and archives under `dist/` are ignored by Git.

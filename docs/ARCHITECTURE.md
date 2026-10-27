# Architecture

The application depends on the gameplay library; the gameplay library never depends on raylib, operating system calls, textures, or audio.

```
main -> application (window, input, screens, fixed-step clock, audio, score file)
                  -> renderer -> Assets (owned texture cache)
                  -> World -> Level + math
                    ^
                    tests (same public Input interface)
```

## Responsibilities

- `include/kwa/world.hpp`, `src/core/world.cpp`: owns player, enemies, companion, campaign outcome and survival waves. A bounded fixed tick advances explicit action states. Combat and AI are private systems operating on value-owned actors. Rendering gets read-only access.
- `include/kwa/level.hpp`, `src/core/level.cpp`: validates `.level` data; provides walkability, axis-separated substep movement, line tests, breadth-first navigation fields and steering. Steer uses actor clearance to avoid getting stuck at corners. The player-centered enemy flow field is refreshed five times per second.
- `src/platform/application.cpp`: translates devices to `Input`, accumulates edge presses until the next 120 Hz simulation tick, owns screen transitions and audio, and persists the survival record. Long frame gaps are capped; pausing discards pending inputs. Catch-up ticks consume edge presses once.
- `src/platform/renderer.cpp`: draws the world in feet-position order, reuses sprite sheets and map textures, and renders HUD / overlays onto a fixed logical canvas. It never changes simulation state.
- `src/platform/assets.cpp`: owns textures with deterministic cleanup. The asset root is relative to the executable, not the working directory. Missing assets fail with their path.
- `tests`: headless mechanics checks, route checks across shipped maps, and a complete input-driven journey / survival regression.

Value ownership avoids dangling entity pointers. Enemy identity is stable across vector compaction; a swing identifier prevents repeated damage. Wolf target pointers are used only inside a call that does not resize the enemy vector. Dead actors are removed after their short death animation. Transient events expire and carry monotonic IDs so audio doesn't replay them every rendered frame.

## Extending the game

**Balance and attacks:** change `attackSpec` and `enemySpec`. Damage, range, timing, cost, movement speed, and health are centralized. Keep new behavior in the simulation and cover its observable input/output behavior in headless tests.

**Enemy types:** extend `EnemyKind` and `enemySpec`, then add the visual treatment in the renderer. If types need substantially different behaviors, extract the enemy system behind a strategy interface rather than adding renderer-driven rules.

**Maps:** `.level` files begin with `KWA_LEVEL 1`, theme / width / height, player and wolf coordinates, then a row-major grid. Tiles are 64 world units; `99` creates an enemy over floor tile 1. Forest tiles 10+ are solid except gate 15; dungeon tiles 7+ are solid except gate 15; town tiles 14+ are solid. Theme IDs intentionally preserve the original art's semantics. Level loading validates dimensions and IDs and resolves obstructed character spawn points to nearby floor. The test suite checks shipped encounter, wolf, and gate connectivity.

**New chapters:** add a map and extend the application chapter lookup and the world's objective rule. For a larger campaign, replace those short mappings with chapter definitions that pair map paths, objective types, narrative and music. Avoid a generic scripting engine until actual content requires it.

**Presentation:** add effects by consuming `Event`, without affecting combat timing. Textures and audio must outlive their use and be released before the window / audio device closes. Local declaration order currently enforces this.

**Saving:** only the survival high score is persisted. A future campaign save should be a separately versioned snapshot with validated bounds; don't serialize renderer resources or raw object memory.

## Intentional limits

No networking, gamepad mapping, campaign save slots, localization, editor, or dynamic scripting layer. Navigation is a grid field, not a navmesh. Enemy separation is quadratic but wave size is capped at 32. The renderer uses a simple read-only snapshot interface rather than an ECS; introduce one only when profiling and entity variety justify the complexity.

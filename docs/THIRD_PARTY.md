# Assets and dependencies

The existing artwork, sprite sheets, tile sets and WAV recordings are reused from this repository's `Project1/Content` directory. Their original ownership and licensing are unchanged. This rewrite does not grant new rights to redistribute those assets. The original project remains available at https://wrathchild14.itch.io/knight-wolfs-adventure.

The native application uses raylib 5.5 (https://github.com/raysan5/raylib/tree/5.5), under the zlib/libpng license, reproduced in `raylib-LICENSE`. Raylib includes third-party components such as GLFW, miniaudio, stb and GLAD. Their notices are retained in the fetched dependency's source; see raylib's `src/external` directory. The game's runtime uses PNG images and WAV audio.

No fonts, artwork, or audio were downloaded for this rewrite. The user interface uses raylib's built-in font. The original C# code is retained for reference and is not linked into the new executable.

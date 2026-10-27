#pragma once
#include <cmath>
namespace kwa {
struct Vec2 {
    float x{}, y{};
    Vec2 operator+(Vec2 b) const { return {x + b.x, y + b.y}; }
    Vec2 operator-(Vec2 b) const { return {x - b.x, y - b.y}; }
    Vec2 operator*(float n) const { return {x * n, y * n}; }
    Vec2& operator+=(Vec2 b) {
        x += b.x;
        y += b.y;
        return *this;
    }
};
inline float length(Vec2 v) {
    return std::sqrt(v.x * v.x + v.y * v.y);
}
inline Vec2 normalized(Vec2 v) {
    const float n = length(v);
    return n > 0.001f ? v * (1.0f / n) : Vec2{};
}
inline float dot(Vec2 a, Vec2 b) {
    return a.x * b.x + a.y * b.y;
}
inline float distance(Vec2 a, Vec2 b) {
    return length(a - b);
}
} // namespace kwa

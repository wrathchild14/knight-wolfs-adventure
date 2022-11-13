﻿using Microsoft.Xna.Framework;
using Project1.Sprites;

namespace Project1
{
    public class Camera
    {
        private readonly int height_;
        private Vector2 position_;

        private readonly int width_;

        public Camera(int width, int height)
        {
            width_ = width;
            height_ = height;
        }

        public Matrix ViewMatrix { get; private set; }

        private static int ScreenWidth => KWAGame.ScreenWidth;

        private static int ScreenHeight => KWAGame.ScreenHeight;

        public void Update(Knight player)
        {
            position_.X = player.X - ScreenWidth / 2.0f;
            position_.Y = player.Y - ScreenHeight / 2.0f;

            // Contain camera in screen
            if (position_.X < 0)
                position_.X = 0;
            if (position_.Y < 0)
                position_.Y = 0;

            if (position_.X > width_ - ScreenWidth)
                position_.X = width_ - ScreenWidth;
            if (position_.Y > height_ - ScreenHeight)
                position_.Y = height_ - ScreenHeight;

            ViewMatrix = Matrix.CreateTranslation(new Vector3(-position_, 0));
        }
    }
}
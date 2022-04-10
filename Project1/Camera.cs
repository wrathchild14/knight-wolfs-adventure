using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Sprites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1
{
    public class Camera
    {
        private Vector2 position_;
        private Matrix viewMatrix_;

        private int width_;
        private int height_;

        public Camera(int width, int height)
        {
            width_ = width;
            height_ = height;
        }

        public Matrix ViewMatrix
        {
            get { return viewMatrix_; }
        }

        public int ScreenWidth
        {
            get { return Game1.ScreenWidth; }
        }

        public int ScreenHight
        {
            get { return Game1.ScreenHeight; }
        }

        public Matrix Transform { get; private set; }

        public void Update(Knight player)
        {
            position_.X = player.X - (ScreenWidth / 2);
            position_.Y = player.Y - (ScreenHight / 2);

            // Contain camera in screen
            if (position_.X < 0)
                position_.X = 0;
            if (position_.Y < 0)
                position_.Y = 0;

            if (position_.X > width_ - ScreenWidth)
                position_.X = width_ - ScreenWidth;
            if (position_.Y > height_ - ScreenHight)
                position_.Y = height_ - ScreenHight;

            viewMatrix_ = Matrix.CreateTranslation(new Vector3(-position_, 0));
        }
    }
}
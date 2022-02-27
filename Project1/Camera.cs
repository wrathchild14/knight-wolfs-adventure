using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1
{
    public class Camera
    {
        private Vector2 position_;
        private Matrix view_matrix_;

        private int level_width_, level_height_;

        public Matrix ViewMatrix
        {
            get { return view_matrix_; }
        }

        public int ScreenWidth
        {
            get { return Game1.screen_width; }
        }

        public int ScreenHight
        {
            get { return Game1.screen_height; }
        }

        public Matrix Transform { get; private set; }

        internal void Update(Vector2 player_position)
        {
            position_.X = player_position.X - (ScreenWidth / 2);
            position_.Y = player_position.Y - (ScreenHight / 2);

            // Contain camera in screen
            if (position_.X < 0)
                position_.X = 0;
            if (position_.Y < 0)
                position_.Y = 0;

            if (position_.X > level_width_ - ScreenWidth)
                position_.X = level_width_ - ScreenWidth;
            if (position_.Y > level_height_ - ScreenHight)
                position_.Y = level_height_ - ScreenHight;

            view_matrix_ = Matrix.CreateTranslation(new Vector3(-position_, 0));
        }

        internal void SetBounds(int width, int height)
        {
            level_width_ = width;
            level_height_ = height;
        }
    }
}
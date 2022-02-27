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
        private Matrix view_matrix_;

        public Texture2D current_background_;
        public Texture2D next_background_ = null;

        private int width_;
        private int height_;

        //public Camera(Texture2D texture2D)
        //{
        //    current_background_ = texture2D;
        //}

        public Camera(int width, int height)
        {
            width_ = width;
            height_ = height;
        }

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

        internal void Update(Knight player)
        {
            position_.X = player.Position.X - (ScreenWidth / 2);
            position_.Y = player.Position.Y - (ScreenHight / 2);

            // Contain camera in screen
            if (position_.X < 0)
                position_.X = 0;
            if (position_.Y < 0)
                position_.Y = 0;

            //if (position_.X > current_background_.Width - ScreenWidth)
            //    position_.X = current_background_.Width - ScreenWidth;
            //if (position_.Y > current_background_.Height - ScreenHight)
            //    position_.Y = current_background_.Height - ScreenHight;

            if (position_.X > width_ - ScreenWidth)
                position_.X = width_ - ScreenWidth;
            if (position_.Y > height_ - ScreenHight)
                position_.Y = height_ - ScreenHight;

            // Going to next level
            //if (player.Position.X > current_background_.Width && next_background_ != null)
            //{
            //    Console.WriteLine("Switching to next background");
            //    current_background_ = next_background_;
            //    next_background_ = null;
            //    player.Position = new Vector2(0, player.Position.Y);
            //    Console.WriteLine(current_background_.ToString());
            //}

            view_matrix_ = Matrix.CreateTranslation(new Vector3(-position_, 0));
        }
    }
}
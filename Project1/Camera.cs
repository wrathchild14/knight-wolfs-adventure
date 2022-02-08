using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1
{
    public class Camera
    {
        public Matrix Transform { get; private set; }

        public void Follow(Sprite target)
        {
            var position = Matrix.CreateTranslation(
              -target.Position.X - (target.Rectangle.Width / 2),
              -target.Position.Y - (target.Rectangle.Height / 2),
              0);

            var offset = Matrix.CreateTranslation(
                Game1.screen_width / 2 - 200,
                Game1.screen_height / 2,
                0);

            Transform = position * offset;
        }
    }
}

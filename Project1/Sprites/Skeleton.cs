using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1.Sprites
{
    public class Skeleton : Sprite
    {
        public Vector2 Velocity;

        public Skeleton(Dictionary<string, Animation> animations) : base(animations)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Animations
            SetAnimation();
            _AnimationManager.Update(gameTime);

            // Reset after animation
            //Velocity.X = 0;
        }

        private void SetAnimation()
        {
            if (Velocity.X < 0)
            {
                _AnimationManager.Right = false;
                _AnimationManager.Play(_Animations["Running"]);
            }
            else if (Velocity.X > 0)
            {
                _AnimationManager.Right = true;
                _AnimationManager.Play(_Animations["Running"]);
            }
            else if (Velocity.X == 0)
                _AnimationManager.Play(_Animations["Idle"]);
        }
    }
}

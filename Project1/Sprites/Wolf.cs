using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1.Sprites
{
    public class Wolf : Sprite
    {
        public Vector2 Velocity;

        private float speed_ = 200f;
        private Sprite player_;

        public Wolf(Dictionary<string, Animation> animations, Sprite player) : base(animations)
        {
            player_ = player;
            animations_["Running"].FrameSpeed = 0.1f;
        }

        public override void Update(GameTime gameTime)
        {
            // Follows the player
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!Rectangle.Intersects(player_.Rectangle))
            {
                Vector2 moveDir = player_.Position - Position;
                moveDir.Normalize();
                Position += moveDir * speed_ * dt;

                Velocity.X += moveDir.X;
            }

            SetAnimation();
            animation_manager_.Update(gameTime);

            Velocity.X = 0;
        }

        private void SetAnimation()
        {
            if (Velocity.X < 0)
            {
                animation_manager_.Right = false;
                animation_manager_.Play(animations_["Running"]);
            }
            else if (Velocity.X > 0)
            {
                animation_manager_.Right = true;
                animation_manager_.Play(animations_["Running"]);
            }
            else if (Velocity.X == 0)
                animation_manager_.Play(animations_["Idle"]);
        }
    }
}
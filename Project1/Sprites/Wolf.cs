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

        private float _Speed = 200f;
        private Sprite _Player;

        public Wolf(Dictionary<string, Animation> animations, Sprite player) : base(animations)
        {
            _Player = player;
        }

        public override void Update(GameTime gameTime)
        {
            // Follows the player
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!Rectangle.Intersects(_Player.Rectangle))
            {
                Vector2 moveDir = _Player.Position - Position;
                moveDir.Normalize();
                Position += moveDir * _Speed * dt;

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

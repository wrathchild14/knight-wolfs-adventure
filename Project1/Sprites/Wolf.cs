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

        public bool Stay = false;

        private float speed_ = 200f;
        private Sprite player_;
        private bool state_follow_player_ = true;
        private Vector2 target_position_;

        public Wolf(Dictionary<string, Animation> animations, Sprite player) : base(animations)
        {
            player_ = player;
            animations_["Running"].FrameSpeed = 0.1f;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Stay)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (state_follow_player_)
                {
                    if (!Rectangle.Intersects(player_.Rectangle))
                    {
                        Vector2 moveDir = player_.Position - Position;
                        moveDir.Normalize();
                        Position += moveDir * speed_ * dt;

                        Velocity.X += moveDir.X;
                    }
                }
                else
                {
                    if (target_position_ != Position) // idk how much does, but it must be something
                    {
                        Vector2 moveDir = target_position_ - Position;
                        moveDir.Normalize();
                        Position += moveDir * speed_ * dt;

                        Velocity.X += moveDir.X;
                    }
                }
            }

            SetAnimation();
            animation_manager_.Update(gameTime);

            Velocity.X = 0;
        }

        public void GoTo(Vector2 position)
        {
            state_follow_player_ = false;
            target_position_ = position;
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
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
        private bool folow_player_bool_ = true;
        private Vector2 target_pos_;

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

                if (folow_player_bool_)
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
                    if (target_pos_ != Position)
                    {
                        Vector2 moveDir = target_pos_ - Position;
                        moveDir.Normalize();
                        Position += moveDir * speed_ * dt;

                        Velocity.X += moveDir.X;
                    }
                }
            }

            SetAnimation();
            animationManager.Update(gameTime);

            Velocity.X = 0;
        }

        public void GoTo(Vector2 position)
        {
            folow_player_bool_ = false;
            target_pos_ = position;
        }

        private void SetAnimation()
        {
            if (Velocity.X < 0)
            {
                animationManager.Right = false;
                animationManager.Play(animations_["Running"]);
            }
            else if (Velocity.X > 0)
            {
                animationManager.Right = true;
                animationManager.Play(animations_["Running"]);
            }
            else if (Velocity.X == 0)
                animationManager.Play(animations_["Idle"]);
        }
    }
}
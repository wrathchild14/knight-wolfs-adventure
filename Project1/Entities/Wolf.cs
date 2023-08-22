using Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Sprites
{
    public class Wolf : Sprite
    {
        private bool folow_player_bool_ = true;
        private readonly Sprite player_;

        private readonly float speed_ = 200f;

        public bool Stay = false;
        private Vector2 target_pos_;
        public Vector2 Velocity;

        public Wolf(Dictionary<string, Animation> animations, Sprite player) : base(animations)
        {
            player_ = player;
            animations_["Running"].FrameSpeed = 0.1f;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Stay)
            {
                var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (folow_player_bool_)
                {
                    if (!Rectangle.Intersects(player_.Rectangle))
                    {
                        var moveDir = player_.Position - Position;
                        moveDir.Normalize();
                        Position += moveDir * speed_ * dt;

                        Velocity.X += moveDir.X;
                    }
                }
                else
                {
                    if (target_pos_ != Position)
                    {
                        var moveDir = target_pos_ - Position;
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
            {
                animationManager.Play(animations_["Idle"]);
            }
        }
    }
}
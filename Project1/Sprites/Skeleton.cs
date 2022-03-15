using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Models;
using System;
using System.Collections.Generic;

namespace Project1.Sprites
{
    public class Skeleton : Sprite
    {
        private Vector2 velocity_;
        private Knight player_;

        private double attacked_timer_ = 0.2;
        private double elapsed_attacked_time_;

        private double attack_timer_ = 1.6;
        private double elapsed_attack_time_;

        private float speed_ = 50f;

        private bool attacked_ = false;
        private bool attacking_ = true;

        private Healthbar health_bar_;

        public Skeleton(Texture2D healthbarTexture, Knight player, Dictionary<string, Animation> animations) : base(animations)
        {
            player_ = player;
            health_bar_ = new Healthbar(healthbarTexture, this);
            animations_["Attacked"].FrameSpeed = 0.1f;
        }

        public override void Update(GameTime gameTime)
        {
            if (Dead)
                animation_manager_.UpdateTillEnd(gameTime);
            else
            {
                // Taking damage
                elapsed_attacked_time_ += gameTime.ElapsedGameTime.TotalSeconds;
                if (player_.Attacking && Rectangle.Intersects(player_.AttackRectangle) && elapsed_attacked_time_ >= attacked_timer_)
                {
                    elapsed_attacked_time_ = 0;
                    elapsed_attack_time_ = 0;
                    health_bar_.TakeDamage(10);

                    if (animation_manager_.Right)
                        X -= 2;
                    else
                        X += 2;
                }

                if (elapsed_attacked_time_ <= attacked_timer_)
                    attacked_ = true;
                else
                    attacked_ = false;

                // Follows the player (Reused code from dog)
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (!Rectangle.Intersects(player_.Rectangle))
                {
                    attacking_ = false;
                    Vector2 moveDir = player_.Position - Position;
                    moveDir.Normalize();
                    Position += moveDir * speed_ * dt;

                    velocity_.X += moveDir.X;
                }
                else
                {
                    // Attack
                    attacking_ = true;
                    elapsed_attack_time_ += gameTime.ElapsedGameTime.TotalSeconds;
                    if (elapsed_attack_time_ >= attack_timer_)
                    {
                        Console.WriteLine("Attacked player");
                        // player_.TakeDamage(10);
                        player_.TakeDamage(10);
                        elapsed_attack_time_ = 0;
                    }
                }

                // Updates
                animation_manager_.Update(gameTime);
                health_bar_.Update(gameTime);
            }

            SetAnimation();
            velocity_.X = 0;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (texture_ != null)
                spriteBatch.Draw(texture_, Position, null, Colour * Opacity, Rotation, Origin, Scale, SpriteEffects.None, Layer);

            if (animation_manager_ != null)
                animation_manager_.Draw(spriteBatch);

            if (health_bar_ != null)
                health_bar_.Draw(gameTime, spriteBatch);
        }

        private void SetAnimation()
        {
            if (Dead)
                animation_manager_.Play(animations_["Dead"]);
            else
            {
                if (attacked_)
                    animation_manager_.Play(animations_["Attacked"]);
                else if (velocity_.X < 0)
                {
                    animation_manager_.Right = false;
                    animation_manager_.Play(animations_["Running"]);
                }
                else if (velocity_.X > 0)
                {
                    animation_manager_.Right = true;
                    animation_manager_.Play(animations_["Running"]);
                }
                else if (attacking_)
                    animation_manager_.Play(animations_["Attack"]);
                else if (velocity_.X == 0)
                    animation_manager_.Play(animations_["Idle"]);
            }
        }
    }
}
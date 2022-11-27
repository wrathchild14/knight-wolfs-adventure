using System.Collections.Generic;
using Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sprites
{
    public class Skeleton : Sprite
    {
        private readonly double attack_timer_ = 1.6;

        private bool attacked_;

        private readonly double attacked_timer_ = 0.2;
        private bool attacking_;
        private readonly Texture2D debug_rectangle_;
        private readonly bool debug_rectangle_bool_ = false;
        private double elapsed_attack_time_;
        private double elapsed_attacked_time_;
        private readonly int followDistance_ = 500;

        private readonly Healthbar healthbar_;
        private readonly Knight player_;
        private bool seen_player_bool_;

        private readonly float speed_ = 40f;

        private Vector2 velocity_;

        public Skeleton(Texture2D debug_rect, Texture2D healthbarTexture, Knight player, int follow_distance,
            Dictionary<string, Animation> animations) : base(animations)
        {
            player_ = player;
            healthbar_ = new Healthbar(healthbarTexture, this);
            animations_["Attacked"].FrameSpeed = 0.1f;
            debug_rectangle_ = debug_rect;
            followDistance_ = follow_distance;
        }

        public override void Update(GameTime gameTime)
        {
            if (Dead)
            {
                animationManager.UpdateTillEnd(gameTime);
            }
            else
            {
                // Taking damage
                elapsed_attacked_time_ += gameTime.ElapsedGameTime.TotalSeconds;
                if (player_.Attacking && Rectangle.Intersects(player_.AttackRectangle) &&
                    elapsed_attacked_time_ >= attacked_timer_)
                {
                    elapsed_attacked_time_ = 0;
                    elapsed_attack_time_ = 0;
                    healthbar_.TakeDamage(10);

                    if (animationManager.Right)
                        X -= 2;
                    else
                        X += 2;
                }

                if (elapsed_attacked_time_ <= attacked_timer_)
                    attacked_ = true;
                else
                    attacked_ = false;

                var distance = Vector2.Distance(Position, player_.Position);
                if (distance < followDistance_ || seen_player_bool_)
                {
                    seen_player_bool_ = true;
                    // Follows the player (Reused code from dog)
                    var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (!Rectangle.Intersects(player_.Rectangle))
                    {
                        attacking_ = false;
                        var moveDir = player_.Position - Position;
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
                            //Console.WriteLine("Attacked player");
                            // player_.TakeDamage(10);
                            player_.TakeDamage(25);
                            elapsed_attack_time_ = 0;
                        }
                    }
                }

                // Updates
                animationManager.Update(gameTime);
                healthbar_.Update(gameTime);
            }

            SetAnimation();
            velocity_.X = 0;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (debug_rectangle_bool_)
                spriteBatch.Draw(debug_rectangle_, Rectangle, Color.Red);

            if (texture_ != null)
                spriteBatch.Draw(texture_, Position, null, Colour * Opacity, Rotation, Origin, Scale,
                    SpriteEffects.None, Layer);

            animationManager?.Draw(spriteBatch);

            healthbar_?.Draw(gameTime, spriteBatch);
        }

        private void SetAnimation()
        {
            if (Dead)
            {
                animationManager.Play(animations_["Dead"]);
            }
            else
            {
                if (attacked_)
                {
                    animationManager.Play(animations_["Attacked"]);
                }
                else if (velocity_.X < 0)
                {
                    animationManager.Right = false;
                    animationManager.Play(animations_["Running"]);
                }
                else if (velocity_.X > 0)
                {
                    animationManager.Right = true;
                    animationManager.Play(animations_["Running"]);
                }
                else if (attacking_)
                {
                    animationManager.Play(animations_["Attack"]);
                }
                else if (velocity_.X == 0)
                {
                    animationManager.Play(animations_["Idle"]);
                }
            }
        }
    }
}
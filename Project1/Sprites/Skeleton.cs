using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Models;
using System;
using System.Collections.Generic;

namespace Project1.Sprites
{
    public class Skeleton : Sprite
    {
        private int followDistance_ = 500;

        private Vector2 velocity_;
        private Knight player_;

        private double attackedTimer_ = 0.2;
        private double elapsedAttackedTime_;

        private double attackTimer_ = 1.6;
        private double elapsedAttackTime_;

        private float speed_ = 40f;

        private bool attacked_ = false;
        private bool attacking_ = false;

        private Healthbar healthBar_;
        private Texture2D debugRectangle_;
        private bool debugRectangleBool_ = false;
        private bool seenPlayerBool_ = false;

        public Skeleton(Texture2D debug_rect, Texture2D healthbarTexture, Knight player, int follow_distance, Dictionary<string, Animation> animations) : base(animations)
        {
            player_ = player;
            healthBar_ = new Healthbar(healthbarTexture, this);
            animations_["Attacked"].FrameSpeed = 0.1f;
            debugRectangle_ = debug_rect;
            followDistance_ = follow_distance;
        }

        public override void Update(GameTime gameTime)
        {
            if (Dead)
                animationManager.UpdateTillEnd(gameTime);
            else
            {
                // Taking damage
                elapsedAttackedTime_ += gameTime.ElapsedGameTime.TotalSeconds;
                if (player_.Attacking && Rectangle.Intersects(player_.AttackRectangle) && elapsedAttackedTime_ >= attackedTimer_)
                {
                    elapsedAttackedTime_ = 0;
                    elapsedAttackTime_ = 0;
                    healthBar_.TakeDamage(10);

                    if (animationManager.Right)
                        X -= 2;
                    else
                        X += 2;
                }

                if (elapsedAttackedTime_ <= attackedTimer_)
                    attacked_ = true;
                else
                    attacked_ = false;

                float distance = Vector2.Distance(Position, player_.Position);
                if (distance < followDistance_ || seenPlayerBool_)
                {
                    seenPlayerBool_ = true;
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
                        elapsedAttackTime_ += gameTime.ElapsedGameTime.TotalSeconds;
                        if (elapsedAttackTime_ >= attackTimer_)
                        {
                            //Console.WriteLine("Attacked player");
                            // player_.TakeDamage(10);
                            player_.TakeDamage(25);
                            elapsedAttackTime_ = 0;
                        }
                    }
                }

                // Updates
                animationManager.Update(gameTime);
                healthBar_.Update(gameTime);
            }

            SetAnimation();
            velocity_.X = 0;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (debugRectangleBool_)
                spriteBatch.Draw(debugRectangle_, Rectangle, Color.Red);

            if (texture_ != null)
                spriteBatch.Draw(texture_, Position, null, Colour * Opacity, Rotation, Origin, Scale, SpriteEffects.None, Layer);

            animationManager?.Draw(spriteBatch);

            healthBar_?.Draw(gameTime, spriteBatch);
        }

        private void SetAnimation()
        {
            if (Dead)
                animationManager.Play(animations_["Dead"]);
            else
            {
                if (attacked_)
                    animationManager.Play(animations_["Attacked"]);
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
                    animationManager.Play(animations_["Attack"]);
                else if (velocity_.X == 0)
                    animationManager.Play(animations_["Idle"]);
            }
        }
    }
}
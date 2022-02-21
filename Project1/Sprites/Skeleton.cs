using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Models;
using System.Collections.Generic;

namespace Project1.Sprites
{
    public class Skeleton : Sprite
    {
        private Vector2 velocity_;
        private Knight player_;

        private double elapsed_attacked_time_;
        private double attacked_timer_ = 0.60;

        private bool attacked_ = false;

        private Health health_bar_;

        public Skeleton(Texture2D healthbarTexture, Knight player, Dictionary<string, Animation> animations) : base(animations)
        {
            player_ = player;
            health_bar_ = new Health(healthbarTexture, this);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Dead)
            {
                elapsed_attacked_time_ += gameTime.ElapsedGameTime.TotalSeconds;
                if (player_.IsAttacking && Rectangle.Intersects(player_.AttackRectangle) && elapsed_attacked_time_ >= attacked_timer_)
                {
                    elapsed_attacked_time_ = 0;
                    health_bar_.TakeDamage(25);
                }

                if (elapsed_attacked_time_ <= attacked_timer_)
                    attacked_ = true;
                else
                    attacked_ = false;

                animation_manager_.Update(gameTime);
                health_bar_.Update(gameTime);
            }
            if (Dead)
            {
                animation_manager_.UpdateTillEnd(gameTime);
            }
            SetAnimation();
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
                if (velocity_.X < 0)
                {
                    animation_manager_.Right = false;
                    animation_manager_.Play(animations_["Running"]);
                }
                else if (velocity_.X > 0)
                {
                    animation_manager_.Right = true;
                    animation_manager_.Play(animations_["Running"]);
                }
                else if (attacked_)
                    animation_manager_.Play(animations_["Attacked"]);
                else if (velocity_.X == 0)
                    animation_manager_.Play(animations_["Idle"]);
            }
        }
    }
}
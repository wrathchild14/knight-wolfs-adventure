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
        private Knight _player;

        private double _elapsedAttackedTime;
        private double _attackedTimer = 0.60;

        private Healthbar _healthbar;

        public Skeleton(Texture2D healthbarTexture, Knight player, Dictionary<string, Animation> animations) : base(animations)
        {
            _player = player;
            _healthbar = new Healthbar(healthbarTexture, this);
        }

        public override void Update(GameTime gameTime)
        {
            // He can get attacked once if the sword rectangle gets him
            _elapsedAttackedTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (_player.IsAttacking && Rectangle.Intersects(_player.AttackRectangle) && _elapsedAttackedTime >= _attackedTimer)
            {
                _elapsedAttackedTime = 0;
                _healthbar.TakeDamage(10);
            }

            // Animations
            SetAnimation();
            _AnimationManager.Update(gameTime);

            _healthbar.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_Texture != null)
                spriteBatch.Draw(_Texture, Position, null, Colour * Opacity, Rotation, Origin, Scale, SpriteEffects.None, Layer);

            if (_AnimationManager != null)
                _AnimationManager.Draw(spriteBatch);

            if (_healthbar != null)
                _healthbar.Draw(gameTime, spriteBatch);
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

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
        private int _health = 100;

        private double _elapsedAttackedTime;
        private double _attackedTimer = 0.60;

        private Vector2 _healthPosition;
        private Rectangle _healthRectangle;
        private Texture2D _healthTexture;

        public Skeleton(Texture2D healthbarTexture, Knight player, Dictionary<string, Animation> animations) : base(animations)
        {
            _player = player;

            _healthTexture = healthbarTexture;
            _healthPosition = new Vector2(Position.X - Rectangle.Width/2, Position.Y - 20);
            _healthRectangle = new Rectangle(0, 0, _healthTexture.Width, _healthTexture.Height);
        }

        public override void Update(GameTime gameTime)
        {
            // He can get attacked once if the sword rectangle gets him
            _elapsedAttackedTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (_player.Attacking && Rectangle.Intersects(_player.AttackRectangle) && _elapsedAttackedTime >= _attackedTimer)
            {
                _elapsedAttackedTime = 0;
                TakeDamage(10);
            }

            // Animations
            SetAnimation();
            _AnimationManager.Update(gameTime);

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_Texture != null)
                spriteBatch.Draw(_Texture, Position, null, Colour * Opacity, Rotation, Origin, Scale, SpriteEffects.None, Layer);

            if (_AnimationManager != null)
                _AnimationManager.Draw(spriteBatch);

            spriteBatch.Draw(_healthTexture, _healthPosition, _healthRectangle, Color.White);
        }


        private void TakeDamage(int damage)
        {
            _healthPosition = new Vector2(Position.X, Position.Y);
            _health -= damage;
            _healthRectangle.Width -= damage;
            Console.WriteLine("ENEMY HEALTh " + _health.ToString());
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

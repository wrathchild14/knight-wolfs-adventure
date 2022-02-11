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
        private double _attackedTimer = 1;

        public Skeleton(Knight player, Dictionary<string, Animation> animations) : base(animations)
        {
            _player = player;
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

        private void TakeDamage(int damage)
        {
            _health -= damage;
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

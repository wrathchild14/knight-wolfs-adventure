using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1.Sprites
{
    public class Knight : Sprite
    {
        public Vector2 Velocity;
        public bool Attacking;
        public Rectangle AttackRectangle;

        private float _speedX = 3.6f;
        private float _speedY = 2.5f;
        private bool _pray;

        private double _elapsedAttackTime;
        private double _attackTimer = 0.6;

        public Knight(Dictionary<string, Animation> animations) : base(animations)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Attack
            _elapsedAttackTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Attack();
                Attacking = true;
                _elapsedAttackTime = 0f;
            }
            if (_elapsedAttackTime >= _attackTimer)
                Attacking = false;

            // Movement
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Y -= _speedY;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Y += _speedY;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                X -= _speedX;
                Velocity.X -= _speedX;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                X += _speedX;
                Velocity.X += _speedX;
            }

            // Sprint
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                _speedX = 5f;
            else
                _speedX = 3.6f;

            // Animations
            SetAnimation();
            _AnimationManager.Update(gameTime);

            // Reset after animation
            // Pray option, don't mind
            if (Keyboard.GetState().IsKeyDown(Keys.P))
                _pray = true;
            else
                _pray = false;

            Velocity.X = 0;
        }

        private void Attack()
        {
            // Working so so
            // Create a rectangle on the swipe of the sword
            if (_AnimationManager.Right)
            {
                AttackRectangle = new Rectangle((int)(Position.X + 20 + Rectangle.Width), (int)(Position.Y + 10), 5, 5);
            }
            else if (!_AnimationManager.Right)
            {
                AttackRectangle = new Rectangle((int)(Position.X - 20), (int)(Position.Y + 10), 5, 5);
            }
        }

        private void SetAnimation()
        {
            if (Attacking)
            {
                _AnimationManager.Play(_Animations["Attack"]);
            }
            else if (Velocity.X < 0)
            {
                _AnimationManager.Right = false;
                _AnimationManager.Play(_Animations["Running"]);
            }
            else if (Velocity.X > 0)
            {
                _AnimationManager.Right = true;
                _AnimationManager.Play(_Animations["Running"]);
            }
            else if (_pray)
                _AnimationManager.Play(_Animations["Pray"]);
            else if (Velocity.X == 0)
                _AnimationManager.Play(_Animations["Idle"]);
        }
    }
}

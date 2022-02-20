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

        public bool IsAttacking;
        public Rectangle AttackRectangle;

        private float speed_x_ = 3.6f;
        private float speed_y_ = 2.5f;
        private bool _pray;

        private Texture2D debug_rectangle_;

        public Knight(Texture2D texture2D, Dictionary<string, Animation> animations) : base(animations)
        {
            debug_rectangle_ = texture2D;
        }

        public override void Update(GameTime gameTime)
        {
            // Attack
            //_elapsedAttackTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                Attack();
            else
                IsAttacking = false;
            
            //if (_elapsedAttackTime >= _attackTimer)
            //    IsAttacking = false;

            // Movement
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Y -= speed_y_;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Y += speed_y_;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                X -= speed_x_;
                Velocity.X -= speed_x_;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                X += speed_x_;
                Velocity.X += speed_x_;
            }

            // Sprint
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                speed_x_ = 5f;
            else
                speed_x_ = 3.6f;

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
            IsAttacking = true;

            // Rectangle for attack zone
            if (_AnimationManager.Right)
            {
                AttackRectangle = new Rectangle((int)(Position.X + 20 + Rectangle.Width), (int)(Position.Y + 10), 5, 5);
            }
            else if (!_AnimationManager.Right)
            {
                AttackRectangle = new Rectangle((int)(Position.X - 20), (int)(Position.Y + 10), 5, 5);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsAttacking)
                spriteBatch.Draw(debug_rectangle_, AttackRectangle, Color.Red);

            base.Draw(gameTime, spriteBatch);
        }

        private void SetAnimation()
        {
            if (IsAttacking)
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

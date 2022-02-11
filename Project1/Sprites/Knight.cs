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

        private float _SpeedX = 3.6f;
        private float _SpeedY = 2.5f;
        private bool _Pray;
        private bool _Attack;

        private double _ElapsedTime;
        private double _AttackTimer = 0.65;

        public Knight(Dictionary<string, Animation> animations) : base(animations)
        {
        }

        public override void Update(GameTime gameTime)
        {
            // Attack
            _ElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                _Attack = true;
                _ElapsedTime = 0f;
            }
            if (_ElapsedTime >= _AttackTimer)
                _Attack = false;


            // Movement
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Y -= _SpeedY;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Y += _SpeedY;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                X -= _SpeedX;
                Velocity.X -= _SpeedX;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                X += _SpeedX;
                Velocity.X += _SpeedX;
            }

            // Sprint
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                _SpeedX = 5f;
            else
                _SpeedX = 3.6f;

            // Animations
            SetAnimation();
            _AnimationManager.Update(gameTime);

            // Reset after animation
            // Pray option, don't mind
            if (Keyboard.GetState().IsKeyDown(Keys.P))
                _Pray = true;
            else
                _Pray = false;

            Velocity.X = 0;
        }

        private void SetAnimation()
        {
            if (_Attack) { 
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
            else if (_Pray)
                _AnimationManager.Play(_Animations["Pray"]);
            else if (Velocity.X == 0)
                _AnimationManager.Play(_Animations["Idle"]);
        }
    }
}

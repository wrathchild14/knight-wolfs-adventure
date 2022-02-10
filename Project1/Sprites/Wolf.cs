using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1.Sprites
{
    public class Wolf : Sprite
    {
        public Vector2 Velocity;

        private float m_SpeedX = 3.6f;
        private float m_SpeedY = 2.5f;

        public Wolf(Dictionary<string, Animation> animations) : base(animations)
        {

        }
        
        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Y -= m_SpeedY;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Y += m_SpeedY;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                X -= m_SpeedX;
                Velocity.X = -m_SpeedX;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                X += m_SpeedX;
                Velocity.X = m_SpeedX;
            }

            SetAnimation();
            m_AnimationManager.Update(gameTime);

            base.Update(gameTime);
        }

        private void SetAnimation()
        {
            if (Velocity.X < 0)
                m_AnimationManager.Play(m_Animations["RunningLeft"]);
            else if (Velocity.X > 0)
                m_AnimationManager.Play(m_Animations["RunningRight"]);
        }
    }
}

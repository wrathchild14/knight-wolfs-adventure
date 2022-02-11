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

        private float m_SpeedX = 3.6f;
        private float m_SpeedY = 2.5f;
        private bool m_Pray;

        public Knight(Dictionary<string, Animation> animations) : base(animations)
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

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                m_SpeedX = 5f;
            else
                m_SpeedX = 3.6f;

                SetAnimation();
            m_AnimationManager.Update(gameTime);

            Velocity.X = 0;

            if (Keyboard.GetState().IsKeyDown(Keys.P))
                m_Pray = true;
            else
                m_Pray = false;
        }

        private void SetAnimation()
        {
            if (Velocity.X < 0)
            {
                m_AnimationManager.Right = false;
                m_AnimationManager.Play(m_Animations["Running"]);
            }
            else if (Velocity.X > 0)
            {
                m_AnimationManager.Right = true;
                m_AnimationManager.Play(m_Animations["Running"]);
            }
            else if (m_Pray)
                m_AnimationManager.Play(m_Animations["Pray"]);
            else if (Velocity.X == 0)
                m_AnimationManager.Play(m_Animations["Idle"]);
        }
    }
}

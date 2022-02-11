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

        private float m_Speed = 200f;
        private Sprite m_Player;

        public Wolf(Dictionary<string, Animation> animations, Sprite player) : base(animations)
        {
            m_Player = player;
        }

        public override void Update(GameTime gameTime)
        {
            // Follows the player
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!Rectangle.Intersects(m_Player.Rectangle))
            {
                Vector2 moveDir = m_Player.Position - Position;
                moveDir.Normalize();
                Position += moveDir * m_Speed * dt;

                Velocity.X += moveDir.X;
            }

            SetAnimation();
            m_AnimationManager.Update(gameTime);

            Velocity.X = 0;
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
            else if (Velocity.X == 0)
                m_AnimationManager.Play(m_Animations["Idle"]);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1.Managers
{
    public class AnimationManager
    {
        public bool Right = false;

        private Animation m_Animation;
        private float m_Timer;
        private bool m_Updated;

        public int FrameWidth
        {
            get
            {
                return m_Animation.FrameWidth;
            }
        }

        public int FrameHeight
        {
            get
            {
                return m_Animation.FrameHeight;
            }
        }

        public Vector2 Position { get; set; }

        public float Layer { get; set; }

        public AnimationManager(Animation animation)
        {
            m_Animation = animation;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!m_Updated)
                throw new Exception("Need to call 'Update' first");

            m_Updated = false;

            if (Right)
                spriteBatch.Draw(m_Animation.Texture,
                             Position,
                             new Rectangle(m_Animation.CurrentFrame * m_Animation.FrameWidth,
                                           0,
                                           m_Animation.FrameWidth,
                                           m_Animation.FrameHeight),
                             Color.White,
                             0f,
                             new Vector2(0, 0),
                             1f,
                             SpriteEffects.None,
                             Layer);
            else
                spriteBatch.Draw(m_Animation.Texture,
                            Position,
                            new Rectangle(m_Animation.CurrentFrame * m_Animation.FrameWidth,
                                          0,
                                          m_Animation.FrameWidth,
                                          m_Animation.FrameHeight),
                            Color.White,
                            0f,
                            new Vector2(0, 0),
                            1f,
                            SpriteEffects.FlipHorizontally,
                            Layer);
        }

        public void Play(Animation animation)
        {
            if (m_Animation == animation)
                return;

            m_Animation = animation;
            m_Animation.CurrentFrame = 0;
            m_Timer = 0;
        }

        public void Stop()
        {
            m_Timer = 0f;
            m_Animation.CurrentFrame = 0;
        }

        public void Update(GameTime gameTime)
        {
            m_Updated = true;
            
            m_Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (m_Timer > m_Animation.FrameSpeed)
            {
                m_Timer = 0f;
                m_Animation.CurrentFrame++;
                if (m_Animation.CurrentFrame >= m_Animation.FrameCount)
                    m_Animation.CurrentFrame = 0;
            }
        }
    }
}
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

        private Animation _Animation;
        private float _Timer;
        private bool _Updated;

        public int FrameWidth
        {
            get
            {
                return _Animation.FrameWidth;
            }
        }

        public int FrameHeight
        {
            get
            {
                return _Animation.FrameHeight;
            }
        }

        public Vector2 Position { get; set; }

        public float Layer { get; set; }

        public AnimationManager(Animation animation)
        {
            _Animation = animation;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_Updated)
                throw new Exception("Need to call 'Update' first");

            _Updated = false;

            if (Right)
                spriteBatch.Draw(_Animation.Texture,
                             Position,
                             new Rectangle(_Animation.CurrentFrame * _Animation.FrameWidth,
                                           0,
                                           _Animation.FrameWidth,
                                           _Animation.FrameHeight),
                             Color.White,
                             0f,
                             new Vector2(0, 0),
                             1f,
                             SpriteEffects.None,
                             Layer);
            else
                spriteBatch.Draw(_Animation.Texture,
                            Position,
                            new Rectangle(_Animation.CurrentFrame * _Animation.FrameWidth,
                                          0,
                                          _Animation.FrameWidth,
                                          _Animation.FrameHeight),
                            Color.White,
                            0f,
                            new Vector2(0, 0),
                            1f,
                            SpriteEffects.FlipHorizontally,
                            Layer);
        }

        public void Play(Animation animation)
        {
            if (_Animation == animation)
                return;

            _Animation = animation;
            _Animation.CurrentFrame = 0;
            _Timer = 0;
        }

        public void Stop()
        {
            _Timer = 0f;
            _Animation.CurrentFrame = 0;
        }

        public void Update(GameTime gameTime)
        {
            _Updated = true;
            
            _Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_Timer > _Animation.FrameSpeed)
            {
                _Timer = 0f;
                _Animation.CurrentFrame++;
                if (_Animation.CurrentFrame >= _Animation.FrameCount)
                    _Animation.CurrentFrame = 0;
            }
        }
    }
}
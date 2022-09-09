using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Models;

namespace Project1.Managers
{
    public class AnimationManager
    {
        private Animation _animation;
        public bool Right = false;
        private float _timer;
        private bool _updated;

        public AnimationManager(Animation animation)
        {
            _animation = animation;
        }

        public int FrameWidth => _animation.FrameWidth;

        public int FrameHeight => _animation.FrameHeight;

        public Vector2 Position { get; set; }

        public float Layer { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_updated)
                Console.WriteLine("AnimationManager.cs - Need to call Update first");

            _updated = false;

            spriteBatch.Draw(_animation.Texture,
                Position,
                new Rectangle(_animation.CurrentFrame * _animation.FrameWidth,
                    0,
                    _animation.FrameWidth,
                    _animation.FrameHeight),
                Color.White,
                0f,
                new Vector2(0, 0),
                1f,
                Right ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                Layer);
        }

        public void Play(Animation animation)
        {
            if (_animation == animation)
                return;

            _animation = animation;
            _animation.CurrentFrame = 0;
            _timer = 0;
        }

        public void Stop()
        {
            _timer = 0f;
            _animation.CurrentFrame = 0;
        }

        public void UpdateTillEnd(GameTime gameTime)
        {
            _updated = true;

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer > _animation.FrameSpeed)
            {
                _timer = 0f;
                _animation.CurrentFrame++;
            }
        }

        public void Update(GameTime gameTime)
        {
            _updated = true;

            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer > _animation.FrameSpeed)
            {
                _timer = 0f;
                _animation.CurrentFrame++;
                if (_animation.CurrentFrame >= _animation.FrameCount)
                    _animation.CurrentFrame = 0;
            }
        }
    }
}
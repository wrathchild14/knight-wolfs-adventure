using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Models;

namespace Project1.Managers
{
    public class AnimationManager
    {
        private Animation animation_;
        public bool Right = false;
        private float timer_;
        private bool updated_;

        public AnimationManager(Animation animation)
        {
            animation_ = animation;
        }

        public int FrameWidth => animation_.FrameWidth;

        public int FrameHeight => animation_.FrameHeight;

        public Vector2 Position { get; set; }

        public float Layer { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!updated_)
                Console.WriteLine("AnimationManager.cs - Need to call Update frist");

            updated_ = false;

            if (Right)
                spriteBatch.Draw(animation_.Texture,
                    Position,
                    new Rectangle(animation_.CurrentFrame * animation_.FrameWidth,
                        0,
                        animation_.FrameWidth,
                        animation_.FrameHeight),
                    Color.White,
                    0f,
                    new Vector2(0, 0),
                    1f,
                    SpriteEffects.None,
                    Layer);
            else
                spriteBatch.Draw(animation_.Texture,
                    Position,
                    new Rectangle(animation_.CurrentFrame * animation_.FrameWidth,
                        0,
                        animation_.FrameWidth,
                        animation_.FrameHeight),
                    Color.White,
                    0f,
                    new Vector2(0, 0),
                    1f,
                    SpriteEffects.FlipHorizontally,
                    Layer);
        }

        public void Play(Animation animation)
        {
            if (animation_ == animation)
                return;

            animation_ = animation;
            animation_.CurrentFrame = 0;
            timer_ = 0;
        }

        public void Stop()
        {
            timer_ = 0f;
            animation_.CurrentFrame = 0;
        }

        public void UpdateTillEnd(GameTime game_time)
        {
            updated_ = true;

            timer_ += (float)game_time.ElapsedGameTime.TotalSeconds;
            if (timer_ > animation_.FrameSpeed)
            {
                timer_ = 0f;
                animation_.CurrentFrame++;
            }
        }

        public void Update(GameTime game_time)
        {
            updated_ = true;

            timer_ += (float)game_time.ElapsedGameTime.TotalSeconds;
            if (timer_ > animation_.FrameSpeed)
            {
                timer_ = 0f;
                animation_.CurrentFrame++;
                if (animation_.CurrentFrame >= animation_.FrameCount)
                    animation_.CurrentFrame = 0;
            }
        }
    }
}
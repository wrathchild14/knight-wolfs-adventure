using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Managers;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project1
{
    public abstract class Sprite : Component
    {
        protected AnimationManager _AnimationManager;
        protected Dictionary<string, Animation> _Animations;

        protected Texture2D texture_;
        protected Vector2 position_;
        protected float layer_ { get; set; }

        public Color Colour { get; set; }
        public float Opacity { get; set; }
        public Vector2 Origin { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }

        public Vector2 Position
        {
            get { return position_; }
            set
            {
                position_ = value;

                if (_AnimationManager != null)
                    _AnimationManager.Position = position_;
            }
        }

        internal void Dead()
        {
            throw new NotImplementedException();
        }

        public float X
        {
            get { return Position.X; }
            set
            {
                Position = new Vector2(value, Position.Y);
            }
        }

        public float Y
        {
            get { return Position.Y; }
            set
            {
                Position = new Vector2(Position.X, value);
            }
        }

        public float Layer
        {
            get { return layer_; }
            set
            {
                layer_ = value;

                if (_AnimationManager != null)
                    _AnimationManager.Layer = layer_;
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                //int x = 0;
                //int y = 0;
                int width = 0;
                int height = 0;

                if (texture_ != null)
                {
                    width = texture_.Width;
                    height = texture_.Height;
                } else if (_AnimationManager != null)
                {
                    width = _AnimationManager.FrameWidth;
                    height = _AnimationManager.FrameHeight;
                }

                return new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), (int)(width * Scale), (int)(height * Scale));
            }
        }

        public bool IsRemoved { get; set; }

        public Sprite(Texture2D texture)
        {
            texture_ = texture;
            Opacity = 1f;
            Scale = 1f;
            Origin = new Vector2(0, 0);
            Colour = Color.White;
        }

        public Sprite(Dictionary<string, Animation> animations)
        {
            _Animations = animations;
            _AnimationManager = new AnimationManager(_Animations.First().Value);
            Opacity = 1f;
            Scale = 1f;
            Colour = Color.White;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (texture_ != null)
                spriteBatch.Draw(texture_, Position, null, Colour * Opacity, Rotation, Origin, Scale, SpriteEffects.None, Layer);

            if (_AnimationManager != null)
                _AnimationManager.Draw(spriteBatch);
        }

        public virtual void OnCollide(Sprite sprite)
        {

        }

        public bool IsTouching(Sprite sprite)
        {
            return this.Rectangle.Right >= sprite.Rectangle.Left &&
                this.Rectangle.Left <= sprite.Rectangle.Right &&
                this.Rectangle.Bottom >= sprite.Rectangle.Top &&
                this.Rectangle.Top <= sprite.Rectangle.Bottom;
        }

        public bool IsTouchingTopOf(Sprite sprite)
        {
            return this.Rectangle.Right >= sprite.Rectangle.Left &&
                this.Rectangle.Left <= sprite.Rectangle.Right &&
                this.Rectangle.Bottom >= sprite.Rectangle.Top &&
                this.Rectangle.Top < sprite.Rectangle.Top;
        }

        public bool IsTouchingLeftOf(Sprite sprite)
        {
            return this.Rectangle.Bottom >= sprite.Rectangle.Top &&
              this.Rectangle.Top <= sprite.Rectangle.Bottom &&
              this.Rectangle.Right >= sprite.Rectangle.Left &&
              this.Rectangle.Left < sprite.Rectangle.Left;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #region Positions
        public Vector2 TopLeft
        {
            get
            {
                return new Vector2(Rectangle.X, Rectangle.Y);
            }
        }

        public Vector2 TopRight
        {
            get
            {
                return new Vector2(Rectangle.X + Rectangle.Width, Rectangle.Y);
            }
        }

        public Vector2 BottomLeft
        {
            get
            {
                return new Vector2(Rectangle.X, Rectangle.Y + Rectangle.Height);
            }
        }

        public Vector2 BottomRight
        {
            get
            {
                return new Vector2(Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height);
            }
        }

        public Vector2 Centre
        {
            get
            {
                return new Vector2(Rectangle.X + (Rectangle.Width / 2), Rectangle.Y + (Rectangle.Height / 2));
            }
        }
        #endregion
    }
}
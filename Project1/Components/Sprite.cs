using Components;
using Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileMap;
using System.Collections.Generic;
using System.Linq;

namespace Components
{
    public abstract class Sprite : Component
    {
        protected AnimationManager animationManager;
        protected Dictionary<string, Animation> animations_;
        public bool Dead;
        protected Vector2 position_;

        protected Texture2D texture_;

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
            animations_ = animations;
            animationManager = new AnimationManager(animations_.First().Value);
            Opacity = 1f;
            Scale = 1f;
            Colour = Color.White;
        }

        protected float layer_ { get; set; }

        public Color Colour { get; set; }
        public float Opacity { get; set; }
        public Vector2 Origin { get; set; }
        public float Rotation { get; set; }
        public float Scale { get; set; }

        public Vector2 Position
        {
            get => position_;
            set
            {
                position_ = value;

                if (animationManager != null)
                    animationManager.Position = position_;
            }
        }

        public float X
        {
            get => Position.X;
            set => Position = new Vector2(value, Position.Y);
        }

        public float Y
        {
            get => Position.Y;
            set => Position = new Vector2(Position.X, value);
        }

        public float Layer
        {
            get => layer_;
            set
            {
                layer_ = value;

                if (animationManager != null)
                    animationManager.Layer = layer_;
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                //int x = 0;
                //int y = 0;
                var width = 0;
                var height = 0;

                if (texture_ != null)
                {
                    width = texture_.Width;
                    height = texture_.Height;
                }
                else if (animationManager != null)
                {
                    width = animationManager.FrameWidth;
                    height = animationManager.FrameHeight;
                }

                //return new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), (int)(width * Scale), (int)(height * Scale));
                return new Rectangle((int)Position.X, (int)Position.Y, (int)(width * Scale), (int)(height * Scale));
            }
        }

        public bool IsRemoved { get; set; }

        public void Die()
        {
            Dead = true;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (texture_ != null)
                spriteBatch.Draw(texture_, Position, null, Colour * Opacity, Rotation, Origin, Scale,
                    SpriteEffects.None, Layer);

            animationManager?.Draw(spriteBatch);
        }

        internal void Collision(CollisionTile tile, int x_offset, int y_offset)
        {
            var tile_rectangle = tile.Rectangle;
            if (Rectangle.TouchTopOf(tile_rectangle))
                Y = tile_rectangle.Y - Rectangle.Height;

            if (Rectangle.TouchLeftOf(tile_rectangle))
                X = tile_rectangle.X - Rectangle.Width - 2;

            if (Rectangle.TouchRightOf(tile_rectangle))
                X = tile_rectangle.X + tile_rectangle.Width + 4;

            if (Rectangle.TouchBottomOf(tile_rectangle))
                Y = tile_rectangle.Y + tile_rectangle.Height + Rectangle.Height / 2;

            if (X < 0) X = 0;
            if (X > x_offset - Rectangle.Width) X = x_offset - Rectangle.Width;
            if (Y < 0) Y = 0;
            if (Y > y_offset - Rectangle.Height) Y = y_offset - Rectangle.Height;
        }

        public virtual void OnCollide(Sprite sprite)
        {
        }

        public bool IsTouching(Sprite sprite)
        {
            return Rectangle.Right >= sprite.Rectangle.Left &&
                   Rectangle.Left <= sprite.Rectangle.Right &&
                   Rectangle.Bottom >= sprite.Rectangle.Top &&
                   Rectangle.Top <= sprite.Rectangle.Bottom;
        }

        public bool IsTouchingTopOf(Sprite sprite)
        {
            return Rectangle.Right >= sprite.Rectangle.Left &&
                   Rectangle.Left <= sprite.Rectangle.Right &&
                   Rectangle.Bottom >= sprite.Rectangle.Top &&
                   Rectangle.Top < sprite.Rectangle.Top;
        }

        public bool IsTouchingLeftOf(Sprite sprite)
        {
            return Rectangle.Bottom >= sprite.Rectangle.Top &&
                   Rectangle.Top <= sprite.Rectangle.Bottom &&
                   Rectangle.Right >= sprite.Rectangle.Left &&
                   Rectangle.Left < sprite.Rectangle.Left;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        #region Positions

        public Vector2 TopLeft => new Vector2(Rectangle.X, Rectangle.Y);

        public Vector2 TopRight => new Vector2(Rectangle.X + Rectangle.Width, Rectangle.Y);

        public Vector2 BottomLeft => new Vector2(Rectangle.X, Rectangle.Y + Rectangle.Height);

        public Vector2 BottomRight => new Vector2(Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height);

        public Vector2 Centre => new Vector2(Rectangle.X + Rectangle.Width / 2, Rectangle.Y + Rectangle.Height / 2);

        #endregion
    }
}
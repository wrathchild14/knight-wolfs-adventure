using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Components
{
    public class Button : Component
    {
        #region Fields

        private MouseState _CurrentMouse;
        private readonly SpriteFont _Font;
        private bool _IsHovering;
        private MouseState _PreviousMouse;
        private readonly Texture2D _Texture;

        #endregion

        #region Properties

        public event EventHandler Click;
        public bool Clicked { get; private set; }
        public Color PenColour { get; set; }
        public Vector2 Position { get; set; }

        public Rectangle Rectangle => new Rectangle((int)Position.X, (int)Position.Y, _Texture.Width, _Texture.Height);

        public string Text { get; set; }

        #endregion

        #region Methods

        public Button(Texture2D texture, SpriteFont font)
        {
            _Texture = texture;

            _Font = font;

            PenColour = Color.Black;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var colour = Color.White;

            if (_IsHovering)
                colour = Color.Gray;

            spriteBatch.Draw(_Texture, Rectangle, colour);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = Rectangle.X + Rectangle.Width / 2 - _Font.MeasureString(Text).X / 2;
                var y = Rectangle.Y + Rectangle.Height / 2 - _Font.MeasureString(Text).Y / 2;

                spriteBatch.DrawString(_Font, Text, new Vector2(x, y), PenColour);
            }
        }

        public override void Update(GameTime gameTime)
        {
            _PreviousMouse = _CurrentMouse;
            _CurrentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(_CurrentMouse.X, _CurrentMouse.Y, 1, 1);

            _IsHovering = false;

            if (mouseRectangle.Intersects(Rectangle))
            {
                _IsHovering = true;

                if (_CurrentMouse.LeftButton == ButtonState.Released &&
                    _PreviousMouse.LeftButton == ButtonState.Pressed) Click?.Invoke(this, new EventArgs());
            }
        }

        #endregion
    }
}
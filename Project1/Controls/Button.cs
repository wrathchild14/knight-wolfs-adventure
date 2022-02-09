using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Project1
{
    public class Button : Component
    {
        #region Fields
        private MouseState m_CurrentMouse;
        private SpriteFont m_Font;
        private bool m_IsHovering;
        private MouseState m_PreviousMouse;
        private Texture2D m_Texture;
        #endregion

        #region Properties
        public event EventHandler Click;
        public bool Clicked { get; private set; }
        public Color PenColour { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, m_Texture.Width, m_Texture.Height);
            }
        }

        public string Text { get; set; }
        #endregion

        #region Methods
        public Button(Texture2D texture, SpriteFont font)
        {
            m_Texture = texture;

            m_Font = font;

            PenColour = Color.Black;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var colour = Color.White;

            if (m_IsHovering)
                colour = Color.Gray;

            spriteBatch.Draw(m_Texture, Rectangle, colour);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (m_Font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (m_Font.MeasureString(Text).Y / 2);

                spriteBatch.DrawString(m_Font, Text, new Vector2(x, y), PenColour);
            }
        }

        public override void Update(GameTime gameTime)
        {
            m_PreviousMouse = m_CurrentMouse;
            m_CurrentMouse = Mouse.GetState();

            var mouseRectangle = new Rectangle(m_CurrentMouse.X, m_CurrentMouse.Y, 1, 1);

            m_IsHovering = false;

            if (mouseRectangle.Intersects(Rectangle))
            {
                m_IsHovering = true;

                if (m_CurrentMouse.LeftButton == ButtonState.Released && m_PreviousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
        }
        #endregion
    }
}

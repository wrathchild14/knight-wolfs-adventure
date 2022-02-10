using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Project1
{
    class Player : Sprite
    {
        private float m_SpeedX = 3.6f;
        private float m_SpeedY = 2.5f;
        private float m_OriginalX;
        public bool punching;
        public Player(Game1 game) : base(game.Content.Load<Texture2D>("Sprites/enemy-wolf"))
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
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                X += m_SpeedX;
            }
            if (punching)
            {
                X = m_OriginalX;
                punching = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                m_OriginalX = X;
                punching = true;
                X += 50;
            }
        }
    }
}

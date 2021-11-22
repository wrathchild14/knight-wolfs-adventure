using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Project1
{
    class Player : Sprite
    {
        // public int Lifes { get; set; }
        private float speedX = 3.6f;
        private float speedY = 2.5f;

        public Player(Game1 game) : base(Vector2.Zero, 100, 100)
        {
            texture = game.Content.Load<Texture2D>("enemy-wolf");

            // Lifes = 5;
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                position.Y -= speedY;
                /*
                if (position.Y > allowedY)
                {
                    position.Y -= speedY;
                }
                 */
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                position.Y += speedY;

            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                position.X -= speedX;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                position.X += speedX;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rect, Color.White);
        }
    }
}

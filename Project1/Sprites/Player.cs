using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Project1
{
    class Player : Sprite
    {
        private float speedX = 3.6f;
        private float speedY = 2.5f;
        private float originalX;
        public bool punching;

        public Player(Game1 game) : base(game.Content.Load<Texture2D>("Sprites/enemy-wolf"))
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Y -= speedY;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Y += speedY;

            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                X -= speedX;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                X += speedX;
            }
            if (punching)
            {
                X = originalX;
                punching = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                originalX = X;
                punching = true;
                X += 50;
            }

        }

    }
}

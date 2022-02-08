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

        public Player(Game1 game) : base(new Vector2(0, 200), 100, 100)
        {
            texture = game.Content.Load<Texture2D>("Sprites/enemy-wolf");
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                position.Y -= speedY;
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
            if (punching)
            {
                position.X = originalX;
                punching = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                originalX = position.X;
                punching = true;
                position.X += 50;
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rect, Color.White);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Project1
{
    class Enemy : Sprite
    {
        // public int Lifes { get; set; }
        private float speedX = 1.5f;
        // private float speedY = 2.5f;

        public Enemy(Game1 game) : base(new Vector2(600, 250), 100, 100)
        {
            texture = game.Content.Load<Texture2D>("p2");

            // Lifes = 5;
        }

        public override void Update(GameTime gameTime)
        {
            position.X -= speedX;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rect, Color.White);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Project1
{
    class Enemy : Sprite
    {
        private float speedX = 1.5f;
        private bool standing;

        public Enemy(Game1 game, int x, int y, bool standing) : base(new Vector2(x, y), 100, 100)
        {
            this.standing = standing;
            texture = game.Content.Load<Texture2D>("p2");
        }

        public override void Update(GameTime gameTime)
        {
            if (!standing)
            { 
                position.X -= speedX;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rect, Color.White);
        }
    }
}

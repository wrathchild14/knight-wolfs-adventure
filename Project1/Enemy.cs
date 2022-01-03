using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Project1
{
    class Enemy : Sprite
    {
        private float speedX = 1.5f;
        private float speed = new Random().Next(0, 100);
        private bool standing;
        private Player player;

        public Enemy(Game1 game, int x, int y, bool standing, Player player) : base(new Vector2(x, y), 100, 100)
        {
            this.standing = standing;
            texture = game.Content.Load<Texture2D>("p2");
            this.player = player;
        }

        public override void Update(GameTime gameTime)
        {
            if (!standing)
            { 
                // position.X -= speedX;
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                Vector2 moveDir = this.player.Position - position;
                moveDir.Normalize();
                //position += moveDir * speed * dt;
                // int state = 0;
                position += moveDir * speed * dt; // move enemy towards player
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rect, Color.White);
        }
    }
}

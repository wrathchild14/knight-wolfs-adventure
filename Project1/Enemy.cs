using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Project1
{
    class Enemy : Sprite
    {
        private float _speed = new Random().Next(20, 100);
        private Player _player;

        public Enemy(Game1 game, int x, int y, Player player) : base(new Vector2(x, y), 100, 100)
        {
            texture = game.Content.Load<Texture2D>("p2");
            _player = player;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Move enemy towards player
            Vector2 moveDir = this._player.Position - position;
            moveDir.Normalize();
            position += moveDir * _speed * dt;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rect, Color.White);
        }
    }
}

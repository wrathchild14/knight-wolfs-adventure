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

        public Enemy(Game1 game, int x, int y, Player player) : base(game.Content.Load<Texture2D>("Sprites/p2"))
        {
            _player = player;

            X = x;
            Y = y;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Move enemy towards player
            Vector2 moveDir = this._player.Position - Position;
            moveDir.Normalize();
            Position += moveDir * _speed * dt;
        }
    }
}

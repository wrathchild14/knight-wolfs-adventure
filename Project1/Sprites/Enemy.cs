using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Project1
{
    class Enemy : Sprite
    {
        private float m_Speed = new Random().Next(20, 100);
        private Player m_Player;

        public Enemy(Game1 game, int x, int y, Player player) : base(game.Content.Load<Texture2D>("Sprites/enemy-fire"))
        {
            m_Player = player;

            X = x;
            Y = y;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // Move enemy towards player
            Vector2 moveDir = this.m_Player.Position - Position;
            moveDir.Normalize();
            Position += moveDir * m_Speed * dt;
        }
    }
}

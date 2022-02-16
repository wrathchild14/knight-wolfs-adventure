using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Project1
{
    public class Healthbar : Component
    {
        private int _health = 100;
        private Vector2 _healthPosition;
        private Rectangle _healthRectangle;
        private Texture2D _healthTexture;
        private Sprite _sprite;

        public Healthbar(Texture2D texture, Sprite sprite)
        {
            _sprite = sprite;
            _healthTexture = texture;
            _healthPosition = new Vector2(0,0);
            _healthRectangle = new Rectangle(0, 0, _healthTexture.Width, _healthTexture.Height);
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
            _healthRectangle.Width -= damage;
            Console.WriteLine("Current health: " + _health.ToString());
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_healthTexture, _healthPosition, _healthRectangle, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            _healthPosition = new Vector2(_sprite.Position.X - 30, _sprite.Position.Y - 10);
        }
    }
}
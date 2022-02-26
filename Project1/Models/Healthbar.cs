using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Project1
{
    public class Healthbar : Component
    {
        private int health_ = 100;
        private Vector2 health_position_;
        private Rectangle health_rectangle_;
        private Texture2D health_texture_;
        private Sprite sprite_;

        public Healthbar(Texture2D texture, Sprite sprite)
        {
            sprite_ = sprite;
            health_texture_ = texture;
            health_position_ = new Vector2(0, 0);
            health_rectangle_ = new Rectangle(0, 0, health_texture_.Width, health_texture_.Height);
        }

        public void TakeDamage(int damage)
        {
            health_ -= damage;
            health_rectangle_.Width -= damage;
            Console.WriteLine("Current health: " + health_.ToString());

            if (health_ <= 0)
                sprite_.Die();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(health_texture_, health_position_, health_rectangle_, Color.White);
        }

        public override void Update(GameTime gameTime)
        {
            health_position_ = new Vector2(sprite_.Position.X - 30, sprite_.Position.Y - 30);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project1.States
{
    public abstract class State
    {
        protected ContentManager content;
        protected GraphicsDevice _graphicsDevice;
        protected Game1 _game;

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void PostUpdate(GameTime gameTime);

        public State(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            _game = game;
            _graphicsDevice = graphicsDevice;
            this.content = content;
        }

        public abstract void Update(GameTime gameTime);
    }
}
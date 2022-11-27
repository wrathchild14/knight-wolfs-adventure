﻿using KWA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace States
{
    public abstract class State
    {
        protected ContentManager content;
        protected KWAGame game_;
        protected GraphicsDevice graphicsDevice_;

        public State(KWAGame game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            game_ = game;
            graphicsDevice_ = graphicsDevice;
            this.content = content;
        }

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public abstract void PostUpdate(GameTime gameTime);

        public abstract void Update(GameTime gameTime);
    }
}
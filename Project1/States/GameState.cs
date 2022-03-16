using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1.States
{
    public class GameState : State
    {
        private Level1 scene_;

        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            scene_ = new Level1(game, content);
        }

        // GameState for loading games from the json save file
        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, bool sceneLoad) : this(game, graphicsDevice, content)
        {
            scene_ = new Level1(game, content);

            // TODO: Imporve this
            scene_.Load();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            scene_.Draw(gameTime, spriteBatch);
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime)
        {
            scene_.Update(gameTime);
        }
    }
}

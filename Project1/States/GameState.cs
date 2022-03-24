using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project1.Levels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1.States
{
    public class GameState : State
    {
        private Level level_;

        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, int level_number) : base(game, graphicsDevice, content)
        {
            switch(level_number)
            {
                case 1:
                    level_ = new Level1(game, content);
                    break;
                case 2:
                    level_ = new Level2(game, content);
                    break;
                case 3:
                    level_ = new Level3(game, content);
                    break;
                default:
                    break;
            }
        }

        // GameState for loading games from the json save file
        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, bool sceneLoad) : base(game, graphicsDevice, content)
        {
            level_ = new Level1(game, content);

            // TODO: Imporve this
            level_.Load();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            level_.Draw(gameTime, spriteBatch);
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime)
        {
            level_.Update(gameTime);
        }
    }
}

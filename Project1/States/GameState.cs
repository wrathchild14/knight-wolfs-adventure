using KWA;
using Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace States
{
    public class GameState : State
    {
        private readonly Level level_;

        public GameState(KWAGame game, GraphicsDevice graphicsDevice, ContentManager content, int levelNumber) : base(
            game, graphicsDevice, content)
        {
            switch (levelNumber)
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
                case 69:
                    game.Instance?.Stop();
                    game.Instance = game.Songs[2].CreateInstance();
                    game.Instance.Volume = 0.5f;
                    game.Instance.IsLooped = true;
                    game.Instance.Play();

                    level_ = new Survival(game, content);
                    break;
            }
        }

        // GameState for loading games from the json save file
        public GameState(KWAGame game, GraphicsDevice graphicsDevice, ContentManager content, bool sceneLoad) : base(game,
            graphicsDevice, content)
        {
            level_ = new Level1(game, content);

            // TODO: Improve this
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
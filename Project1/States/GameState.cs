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
        private Rectangle m_MainFrame;
        private Player m_Player;
        private Scene m_Scene;

        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            m_Scene = new Scene(game, content);
        }

        // GameState for loading games from the json save file
        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content, bool sceneLoad) : this(game, graphicsDevice, content)
        {
            m_Scene = new Scene(game, content);

            // TODO: Imporve this
            m_Scene.Load();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            m_Scene.Draw(gameTime, spriteBatch);
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime)
        {
            m_Scene.Update(gameTime);
        }
    }
}

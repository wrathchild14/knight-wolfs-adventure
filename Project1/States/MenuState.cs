using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project1.States
{
    public class MenuState : State
    {
        private readonly Texture2D background_;
        private readonly List<Component> components_;

        public MenuState(KWAGame game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice,
            content)
        {
            var buttonTexture = content.Load<Texture2D>("Button");
            var buttonFont = content.Load<SpriteFont>("defaultFont");
            background_ = content.Load<Texture2D>("MenuBackground");

            // Used for centering the buttons
            var buttonWidth = KWAGame.ScreenWidth / 2 - 100;

            var startGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonWidth, 250),
                Text = "Play"
            };
            startGameButton.Click += NewGameButtonClick;

            var survivalButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonWidth, 300),
                Text = "Survival"
            };
            survivalButton.Click += SurvivalButtonClick;

            var quitGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(buttonWidth, 350),
                Text = "Quit Game"
            };
            quitGameButton.Click += QuitGameButtonClick;

            components_ = new List<Component>
            {
                startGameButton,
                survivalButton,
                quitGameButton
            };
        }

        private void SurvivalButtonClick(object sender, EventArgs e)
        {
            game_.ChangeState(new GameState(game_, graphicsDevice_, content, 69));
        }

        //private void LoadGameButtonClick(object sender, EventArgs e)
        //{
        //    // Last bool is if we are loading a game from stats.json
        //    game_.ChangeState(new GameState(game_, graphicsDevice_, content, true));
        //}

        private void NewGameButtonClick(object sender, EventArgs e)
        {
            //_Game.ChangeState(new GameState(_Game, _GraphicsDevice, content, 2));
            game_.NextLevelState();
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // remove sprites if they're not needed
        }

        private void QuitGameButtonClick(object sender, EventArgs e)
        {
            game_.Exit();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in components_)
                component.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background_, new Rectangle(0, 0, KWAGame.ScreenWidth, KWAGame.ScreenHeight), Color.White);

            foreach (var component in components_)
                component.Draw(gameTime, spriteBatch);
        }
    }
}
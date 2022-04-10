using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project1.States
{
    public class MenuState : State
    {
        private List<Component> components_;

        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            var buttonTexture = base.content.Load<Texture2D>("Button");
            var buttonFont = base.content.Load<SpriteFont>("defaultFont");

            // Used for centering the buttons
            var button_width = Game1.ScreenWidth / 2 - 100;

            Button start_game_button = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(button_width, 200),
                Text = "Play",
            };
            start_game_button.Click += NewGameButton_Click;

            Button survival_button = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(button_width, 250),
                Text = "Survival",
            };
            survival_button.Click += SurvivalButtonClick;

            var loadGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(button_width, 250),
                Text = "Load Game",
            };
            loadGameButton.Click += LoadGameButton_Click;

            var optionsButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(button_width, 300),
                Text = "Options",
            };
            optionsButton.Click += optionsButton_Click;

            var quitGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(button_width, 350),
                Text = "Quit Game",
            };
            quitGameButton.Click += QuitGameButton_Click;

            components_ = new List<Component>()
            {
                start_game_button,
                //loadGameButton,
                quitGameButton,
                optionsButton,
                survival_button
            };
        }

        private void SurvivalButtonClick(object sender, EventArgs e)
        {
            game_.ChangeState(new GameState(game_, graphicsDevice_, content, 69));
        }

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            // Last bool is if we are loading a game from stats.json
            game_.ChangeState(new GameState(game_, graphicsDevice_, content, true));
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            //_Game.ChangeState(new GameState(_Game, _GraphicsDevice, content, 2));
            game_.NextLevelState();
        }

        private void optionsButton_Click(object sender, EventArgs e)
        {
            game_.ChangeState(new OptionsState(game_, graphicsDevice_, content));
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // remove sprites if they're not needed
        }

        private void QuitGameButton_Click(object sender, EventArgs e)
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
            foreach (var component in components_)
                component.Draw(gameTime, spriteBatch);
        }
    }
}
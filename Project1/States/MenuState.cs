using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project1.States
{
    public class MenuState : State
    {
        private List<Component> _Components;


        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            var buttonTexture = base.content.Load<Texture2D>("Button");
            var buttonFont = base.content.Load<SpriteFont>("defaultFont");

            // Used for centering the buttons
            var button_width = graphicsDevice.Viewport.Width / 2 - 100;

            Button newGameButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(button_width, 200),
                Text = "Play Game",
            };
            newGameButton.Click += NewGameButton_Click;

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

            _Components = new List<Component>()
            {
                newGameButton,
                loadGameButton,
                quitGameButton,
                optionsButton,
            };
        }

        private void LoadGameButton_Click(object sender, EventArgs e)
        {
            // Last bool is if we are loading a game from stats.json
            _Game.ChangeState(new GameState(_Game, _GraphicsDevice, content, true));
        }

        private void NewGameButton_Click(object sender, EventArgs e)
        {
            _Game.ChangeState(new GameState(_Game, _GraphicsDevice, content));
        }

        private void optionsButton_Click(object sender, EventArgs e)
        {
            _Game.ChangeState(new OptionsState(_Game, _GraphicsDevice, content));
        }

        public override void PostUpdate(GameTime gameTime)
        {
            // remove sprites if they're not needed
        }

        private void QuitGameButton_Click(object sender, EventArgs e)
        {
            _Game.Exit();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _Components)
                component.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _Components)
                component.Draw(gameTime, spriteBatch);
        }
    }
}
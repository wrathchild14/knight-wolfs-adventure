using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project1.States
{
    internal class OptionsState : State
    {
        private List<Component> _Components;

        public OptionsState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            var buttonTexture = base.content.Load<Texture2D>("Button");
            var buttonFont = base.content.Load<SpriteFont>("defaultFont");

            Button backButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(0, 200),
                Text = "Back",
            };
            backButton.Click += backButton_Click;

            Button optionsButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(300, 250),
                Text = "Options (to do) ...",
            };
            optionsButton.Click += optionsButton_Click;

            _Components = new List<Component>()
            {
                backButton,
                optionsButton
            };
        }

        private void optionsButton_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            _Game.ChangeStateMenu();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _Components)
                component.Draw(gameTime, spriteBatch);
        }

        public override void PostUpdate(GameTime gameTime)
        {
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _Components)
                component.Update(gameTime);
        }
    }
}
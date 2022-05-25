using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project1.States
{
    public class OptionsState : State
    {
        private readonly Texture2D background_;
        private readonly List<Component> components_;

        public OptionsState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game,
            graphicsDevice, content)
        {
            var buttonTexture = content.Load<Texture2D>("Button");
            var buttonFont = content.Load<SpriteFont>("defaultFont");
            background_ = content.Load<Texture2D>("MenuBackground");

            var backButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(Game1.ScreenWidth / 2 - 100, 250),
                Text = "Back"
            };
            backButton.Click += backButton_Click;

            var optionsButton = new Button(buttonTexture, buttonFont)
            {
                Position = new Vector2(Game1.ScreenWidth / 2 - 100, 200),
                Text = "Options (to do) ..."
            };
            optionsButton.Click += optionsButton_Click;

            components_ = new List<Component>
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
            game_.ChangeStateMenu();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background_, new Rectangle(0, 0, Game1.ScreenWidth, Game1.ScreenHeight), Color.White);

            foreach (var component in components_)
                component.Draw(gameTime, spriteBatch);
        }

        public override void PostUpdate(GameTime gameTime)
        {
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in components_)
                component.Update(gameTime);
        }
    }
}
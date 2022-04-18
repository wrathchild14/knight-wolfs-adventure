using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project1.States
{
    public class EndState : State
    {
        private List<Component> components_;
        private SpriteFont font;

        private int screenCenter_;

        public EndState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            screenCenter_ = Game1.ScreenWidth / 2 - 100;

            var buttonTexture = content.Load<Texture2D>("Button");
            font = content.Load<SpriteFont>("defaultFont");

            Button backButton = new Button(buttonTexture, font)
            {
                Position = new Vector2(screenCenter_, 300),
                Text = "Main Menu",
            };
            backButton.Click += backButton_Click;

            components_ = new List<Component>()
            {
                backButton,
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
            foreach (var component in components_)
                component.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(font, "You died!", new Vector2(Game1.ScreenWidth / 2 - 100, 200), Color.Red);
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
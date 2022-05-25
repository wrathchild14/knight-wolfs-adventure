using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Project1.States
{
    public class EndState : State
    {
        private readonly List<Component> components_;
        private readonly SpriteFont font;

        private readonly int screen_center_;

        public EndState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice,
            content)
        {
            screen_center_ = Game1.ScreenWidth / 2 - 100;

            var buttonTexture = content.Load<Texture2D>("Button");
            font = content.Load<SpriteFont>("defaultFont");

            var backButton = new Button(buttonTexture, font)
            {
                Position = new Vector2(screen_center_, 300),
                Text = "Main Menu"
            };
            backButton.Click += backButton_Click;

            components_ = new List<Component>
            {
                backButton
            };
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
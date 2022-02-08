using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project1.States
{
    internal class EndState : State
    {
        private List<Component> _components;
        private SpriteFont _font;

        private int _screen_center;

        public EndState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _screen_center = graphicsDevice.Viewport.Width / 2 - 100;

            var buttonTexture = content.Load<Texture2D>("Button");
            _font = content.Load<SpriteFont>("defaultFont");

            Button backButton = new Button(buttonTexture, _font)
            {
                Position = new Vector2(_screen_center, 300),
                Text = "Main Menu",
            };
            backButton.Click += backButton_Click;

            _components = new List<Component>()
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
            _game.ChangeStateMenu();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(_font, "You defeated all the enemies!", new Vector2(_screen_center, 200), Color.Red);
        }

        public override void PostUpdate(GameTime gameTime)
        {
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);
        }
    }
}
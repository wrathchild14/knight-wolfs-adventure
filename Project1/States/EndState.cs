using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project1.States
{
    internal class EndState : State
    {
        private List<Component> m_Components;
        private SpriteFont m_Font;

        private int _screen_center;

        public EndState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _screen_center = graphicsDevice.Viewport.Width / 2 - 100;

            var buttonTexture = content.Load<Texture2D>("Button");
            m_Font = content.Load<SpriteFont>("defaultFont");

            Button backButton = new Button(buttonTexture, m_Font)
            {
                Position = new Vector2(_screen_center, 300),
                Text = "Main Menu",
            };
            backButton.Click += backButton_Click;

            m_Components = new List<Component>()
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
            m_Game.ChangeStateMenu();
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in m_Components)
                component.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(m_Font, "You defeated all the enemies!", new Vector2(_screen_center, 200), Color.Red);
        }

        public override void PostUpdate(GameTime gameTime)
        {
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in m_Components)
                component.Update(gameTime);
        }
    }
}
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
        private Texture2D _background;
        private Rectangle _mainFrame;
        private Player _player;
        private Scene _scene;

        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _background = content.Load<Texture2D>("level-sewer");
            _mainFrame = new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
            _player = new Player(game);
            _scene = new Scene(game, _player, _mainFrame)
            {
                PunchSound = content.Load<SoundEffect>("punch"),
                EndSound = content.Load<SoundEffect>("end"),
                Font = content.Load<SpriteFont>("defaultFont"),
            };
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Background drawn here for some reason, TODO: Change it
            spriteBatch.Draw(_background, _mainFrame, Color.White);
            _scene.Draw(gameTime, spriteBatch);
        }

        public override void PostUpdate(GameTime gameTime)
        {

        }

        public override void Update(GameTime gameTime)
        {
            _scene.Update(gameTime);
        }
    }
}

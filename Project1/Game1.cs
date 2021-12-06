using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Project1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Rectangle mainFrame;
        private Player player;
        private Texture2D background;
        private SpriteFont myFont;
        private Scene scene;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize(); // this must be here
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            mainFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            player = new Player(this);
            
            scene = new Scene(this, player, 3, 2, mainFrame, _spriteBatch, myFont);
        }

        protected override void LoadContent()
        {
            myFont = Content.Load<SpriteFont>("MyFont");
            background = Content.Load<Texture2D>("level-sewer");
        }

        protected override void Update(GameTime gameTime)
        {
            scene.update(gameTime); // updates player and enemies

            //base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);


            _spriteBatch.Begin();
            _spriteBatch.Draw(background, mainFrame, Color.White);
            
            scene.draw(gameTime); // draws player and enemies

            _spriteBatch.End();
            //base.Draw(gameTime);
        }
    }
}

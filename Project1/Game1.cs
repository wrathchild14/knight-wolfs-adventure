using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Project1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private MyControl controller;
        private SpriteBatch _spriteBatch;
        private Rectangle mainFrame;
        private Player player;
        private Texture2D background;
        private SpriteFont myFont;
        private Scene scene;
        List<SoundEffect> effects = new List<SoundEffect>();

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Adding the controller (UI)
            controller = new MyControl(this);
            this.Components.Add(controller);

            base.Initialize(); // this must be here
            
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            mainFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            player = new Player(this);
            scene = new Scene(this, player, 8, mainFrame, _spriteBatch, myFont);
            scene.punch = effects[2];
            scene.end = effects[1];
        }

        protected override void LoadContent()
        {
            myFont = Content.Load<SpriteFont>("defaultFont");
            background = Content.Load<Texture2D>("level-sewer");

            effects.Add(Content.Load<SoundEffect>("start"));
            effects.Add(Content.Load<SoundEffect>("end"));
            effects.Add(Content.Load<SoundEffect>("punch"));

            effects[0].Play();
        }

        protected override void Update(GameTime gameTime)
        {
            if (controller.play)
            {
                scene.Update(gameTime); // updates player and enemies
                if (scene.endGameBool)
                {
                    controller.play = false;
                    controller.Visible = true;
                    scene.endGameBool = false;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            _spriteBatch.Begin();
            _spriteBatch.Draw(background, mainFrame, Color.White);
            
            if (controller.play)
            {
                scene.Draw(gameTime); // draws player and enemies
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1.States;
using System;
using System.Collections.Generic;

namespace Project1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;
        public Vector2 CenterScreen
            => new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);

        private MenuState menuState_;
        private State currentState_;
        private State nextState_;
        private State endState_;

        private int level_ = 0;

        public List<SoundEffect> Songs;
        public SoundEffectInstance Instance;

        public Game1()
        {
            // Setting up graphics and content
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.ApplyChanges();

            IsMouseVisible = true;

            // Main spritebatch that we will pass around later
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Scene initialization is put in GameState for now
            endState_ = new EndState(this, GraphicsDevice, Content);
            menuState_ = new MenuState(this, GraphicsDevice, Content);
            currentState_ = menuState_;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: Effect start starts at "Play game", not before
            Songs = new List<SoundEffect>()
            {
                Content.Load<SoundEffect>("Sounds/Forest"),
                Content.Load<SoundEffect>("Sounds/Dungeon"),
                Content.Load<SoundEffect>("Sounds/Town")
            };
        }

        protected override void Update(GameTime gameTime)
        {
            if (nextState_ != null)
            {
                currentState_ = nextState_;
                nextState_ = null;
            }
            // Handles all the updates
            currentState_.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            currentState_.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            //base.Draw(gameTime);
        }

        // State-change methods
        public void ChangeState(State state)
        {
            nextState_ = state;
        }

        public void ChangeStateMenu()
        {
            nextState_ = menuState_;
            Instance?.Stop();
            level_ = 0; // Reset to level 1
        }

        public void NextLevelState()
        {
            System.Threading.Thread.Sleep(1000);
            Content.Load<SoundEffect>("Sounds/start").Play();
            
            // Handle music
            Instance?.Stop();
            Instance = Songs[level_].CreateInstance();
            Instance.Volume = 0.25f; // Doesn't work
            Instance.IsLooped = true;
            Instance.Play();

            level_++;
            nextState_ = new GameState(this, GraphicsDevice, Content, level_);
        }

        public void ChangeStateEnd()
        {
            System.Threading.Thread.Sleep(1000);
            Content.Load<SoundEffect>("Sounds/end").Play();
            nextState_ = endState_;
        }
    }
}
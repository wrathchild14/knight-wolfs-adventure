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

        public static int screen_width = 1280;
        public static int screen_height = 720;
        public Vector2 CenterScreen
            => new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);

        private MenuState _menuState;
        private State _currentState;
        private State _nextState;
        private State _endState;

        private int level_ = 0;

        public List<SoundEffect> songs;
        public SoundEffectInstance instance;

        public Game1()
        {
            // Setting up graphics and content
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = screen_width;
            graphics.PreferredBackBufferHeight = screen_height;
            graphics.ApplyChanges();

            IsMouseVisible = true;

            // Main spritebatch that we will pass around later
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Scene initialization is put in GameState for now
            _endState = new EndState(this, GraphicsDevice, Content);
            _menuState = new MenuState(this, GraphicsDevice, Content);
            _currentState = _menuState;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: Effect start starts at "Play game", not before
            songs = new List<SoundEffect>()
            {
                Content.Load<SoundEffect>("Sounds/Forest"),
                Content.Load<SoundEffect>("Sounds/Dungeon"),
                Content.Load<SoundEffect>("Sounds/Town")
            };
        }

        protected override void Update(GameTime gameTime)
        {
            if (_nextState != null)
            {
                _currentState = _nextState;
                _nextState = null;
            }
            // Handles all the updates
            _currentState.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            _currentState.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            //base.Draw(gameTime);
        }

        // State-change methods
        public void ChangeState(State state)
        {
            _nextState = state;
        }

        public void ChangeStateMenu()
        {
            _nextState = _menuState;
            instance?.Stop();
            level_ = 0; // Reset to level 1
        }

        public void NextLevelState()
        {
            System.Threading.Thread.Sleep(1000);
            Content.Load<SoundEffect>("Sounds/start").Play();
            
            // Handle music
            instance?.Stop();
            instance = songs[level_].CreateInstance();
            instance.Volume = 0.25f; // Doesn't work
            instance.IsLooped = true;
            instance.Play();

            level_++;
            _nextState = new GameState(this, GraphicsDevice, Content, level_);
        }

        public void ChangeStateEnd()
        {
            System.Threading.Thread.Sleep(1000);
            Content.Load<SoundEffect>("Sounds/end").Play();
            _nextState = _endState;
        }
    }
}
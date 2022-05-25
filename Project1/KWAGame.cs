using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Project1.States;

namespace Project1
{
    public class KWAGame : Game
    {
        public static int ScreenWidth = 1280;
        public static int ScreenHeight = 720;
        private State currentState_;
        private State endState_;
        private readonly GraphicsDeviceManager graphics;
        public SoundEffectInstance Instance;

        private int level_;

        private MenuState menuState_;
        private State nextState_;

        public List<SoundEffect> Songs;
        private SpriteBatch spriteBatch;

        public KWAGame()
        {
            // Setting up graphics and content
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        public Vector2 CenterScreen
            => new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.ApplyChanges();

            IsMouseVisible = true;

            // Main sprite batch that we will pass around later
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
            Songs = new List<SoundEffect>
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
            Thread.Sleep(250);
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
            Thread.Sleep(250);
            Content.Load<SoundEffect>("Sounds/end").Play();
            nextState_ = endState_;
        }
    }
}
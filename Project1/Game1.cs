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
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MenuState _menuState;
        List<SoundEffect> _effects = new List<SoundEffect>();

        private State _currentState;
        private State _nextState;
        private State _endState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            // Main spritebatch that we will pass around later
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Scene initialization is put in GameState for now
            _endState = new EndState(this, GraphicsDevice, Content);
            _menuState = new MenuState(this, GraphicsDevice, Content);
            _currentState = _menuState;
        }

        protected override void LoadContent()
        {
            // TODO: Effect start starts at "Play game", not before
            Content.Load<SoundEffect>("start").Play();
        }

        protected override void Update(GameTime gameTime)
        {
            if (_nextState != null)
            {
                _currentState = _nextState;
                _nextState = null;
            }
            // Handles all the updates/draws 
            _currentState.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            _spriteBatch.Begin();
            _currentState.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // State-change methods
        public void ChangeState(State state)
        {
            _nextState = state;
        }

        public void ChangeStateMenu()
        {
            _nextState = _menuState;
        }

        public void ChangeStateEnd()
        {
            _nextState = _endState;
        }
    }
}

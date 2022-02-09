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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static int screen_width = 1280;
        public static int screen_height = 720;

        private MenuState m_menuState;
        private State m_currentState;
        private State m_nextState;
        private State m_endState;

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
            m_endState = new EndState(this, GraphicsDevice, Content);
            m_menuState = new MenuState(this, GraphicsDevice, Content);
            m_currentState = m_menuState;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: Effect start starts at "Play game", not before
            Content.Load<SoundEffect>("Sounds/start").Play();
        }

        protected override void Update(GameTime gameTime)
        {
            if (m_nextState != null)
            {
                m_currentState = m_nextState;
                m_nextState = null;
            }
            // Handles all the updates 
            m_currentState.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            m_currentState.Draw(gameTime, spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        // State-change methods
        public void ChangeState(State state)
        {
            m_nextState = state;
        }

        public void ChangeStateMenu()
        {
            m_nextState = m_menuState;
        }

        public void ChangeStateEnd()
        {
            m_nextState = m_endState;
        }
    }
}

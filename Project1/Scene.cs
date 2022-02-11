using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project1.Models;
using Project1.Sprites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Project1
{
    class Scene
    {
        private readonly Game1 m_Game;

        private List<Enemy> m_Enemies = new List<Enemy>();

        private Sprite m_Player;
        private Sprite m_Wolf;

        private Random m_Random = new Random();
        private int m_Destroyed = 0;
        private Rectangle m_MainFrame;
        private PlayerStats m_PlayerStats;
        private Texture2D m_BackGroundCurrent;
        private Texture2D m_BackgroundNext;
        private string m_StatsPath = "Stats.json";

        // Content loaders
        private SoundEffect m_PunchSound;
        private SoundEffect m_EndSound;
        private SpriteFont m_Font;

        // Custom spritebatch for the scene
        private Camera m_Camera;
        private SpriteBatch m_SpriteBatch;

        public Scene(Game1 game, ContentManager content)
        {
            // Creating of our player which is a Knight + Wolf
            // Idk how this should work, for now we just take the input for both
            m_Player = new Knight(new Dictionary<string, Animation>()
            {
                { "Idle", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightIdle"), 8) },
                { "Running", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightRunning"), 8) }
            });

            m_Wolf = new Wolf(new Dictionary<string, Animation>()
            {
                { "Idle", new Animation(content.Load<Texture2D>("Sprites/Wolf/WolfIdle"), 4) },
                { "Running", new Animation(content.Load<Texture2D>("Sprites/Wolf/WolfRunning"), 4) }
            })
            {
                Position = new Vector2(m_Player.Position.X - 40, m_Player.Position.Y + 15)
            };


            m_Game = game;
            m_MainFrame = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
            m_PlayerStats = new PlayerStats()
            {
                Name = "Jovan",
                Score = 0,
            };

            m_BackGroundCurrent = content.Load<Texture2D>("Backgrounds/level-sewer");
            m_PunchSound = content.Load<SoundEffect>("Sounds/punch");
            m_EndSound = content.Load<SoundEffect>("Sounds/end");
            m_Font = content.Load<SpriteFont>("defaultFont");

            // TODO
            m_BackgroundNext = content.Load<Texture2D>("Backgrounds/level-cyberpunk");

            // Solution for camera: add a custom spritebatch to the scene
            m_Camera = new Camera();
            m_SpriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        public void Save(PlayerStats stats)
        {
            // This can be list so that we can have more Players/Enemies
            string serializedText = JsonSerializer.Serialize<PlayerStats>(stats);

            // This doesn't append (need to use "append" something)
            File.WriteAllText(m_StatsPath, serializedText);
        }

        // Called in create
        public void Load()
        {
            var fileContent = File.ReadAllText(m_StatsPath);
            m_PlayerStats = JsonSerializer.Deserialize<PlayerStats>(fileContent);
            m_Destroyed = m_PlayerStats.Score;
        }

        void EndGame()
        {
            m_EndSound.Play();
            System.Threading.Thread.Sleep(1000);

            // Exit to menu when game ends
            m_Game.ChangeStateMenu();
        }

        internal void Update(GameTime gameTime)
        {
            m_Player.Update(gameTime);
            m_Wolf.Update(gameTime);
            m_Camera.Follow(m_Player);

            //for (int i = 0; i < m_Enemies.Count; i++)
            //{
            //    if (m_Enemies[i] != null)
            //    {
            //        m_Enemies[i].Update(gameTime);

            //        // Making a rectangle to make collisions better
            //        //Rectangle collisionRect = _player.Rectangle;
            //        //collisionRect.Width -= 50;
            //        //collisionRect.Height -= 50;
            //        //if (collisionRect.Intersects(_enemies[i].Rectangle))
            //        if (m_Player.IsTouching(m_Enemies[i]))
            //        {
            //            if (m_Player.punching)
            //            {
            //                m_Enemies[i] = null;
            //                m_Enemies.Remove(m_Enemies[i]);
            //                m_Destroyed++;
            //                m_PunchSound.Play();

            //                // Maybe improve this
            //                m_PlayerStats.Score = m_Destroyed;
            //            }
            //            else
            //            {
            //                Save(m_PlayerStats);
            //                EndGame();
            //            }
            //        }
            //    }
            //}

            //// Some end game mechanic
            //if (m_Destroyed < 20)
            //{
            //    if (m_Enemies.Count < 10)
            //    {
            //        m_Enemies.Add(new Enemy(m_Game, m_Random.Next(m_MainFrame.Width - 50, m_MainFrame.Width + 200), m_Random.Next(200, 400), m_Player));
            //    }
            //}

            //// TODO
            //if (m_Destroyed > 10)
            //{
            //    m_BackGroundCurrent = m_BackgroundNext;
            //}

            //if (m_Enemies.Count == 0)
            //{
            //    m_Game.ChangeStateEnd();
            //}
        }

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            m_SpriteBatch.Begin(transformMatrix: m_Camera.Transform);

            // Background
            m_SpriteBatch.Draw(m_BackGroundCurrent, m_MainFrame, Color.White);

            // Sprites
            m_Player.Draw(gameTime, m_SpriteBatch);
            m_Wolf.Draw(gameTime, m_SpriteBatch);
            //foreach (var enemy in m_Enemies)
            //    enemy.Draw(gameTime, m_SpriteBatch);

            m_SpriteBatch.End();

            // Scoreboard (in the old spriteBatch)
            spriteBatch.DrawString(m_Font, m_Destroyed.ToString(), new Vector2(10, 10), Color.White);
        }
    }
}
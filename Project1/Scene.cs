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
        private readonly Game1 _Game;

        private Sprite _Player;
        private Sprite _Wolf;
        private Sprite _Skeleton;

        private int _Destroyed = 0;
        private PlayerStats _PlayerStats;
        private Texture2D _BackGroundCurrent;
        private Texture2D _BackgroundNext;
        private string _StatsPath = "Stats.json";

        // Content loaders
        private SoundEffect _PunchSound;
        private SoundEffect _EndSound;
        private SpriteFont _Font;

        // Custom spritebatch for the scene
        private Camera _Camera;
        private SpriteBatch _SpriteBatch;

        private List<Sprite> _SpriteList;

        public Scene(Game1 game, ContentManager content)
        {
            // Creating of our player which is a Knight + Wolf
            // Idk how this should work, for now we just take the input for both
            _Player = new Knight(new Dictionary<string, Animation>()
            {
                { "Attack", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightAttack"), 4) },
                { "Pray", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightPray"), 12) },
                { "Idle", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightIdle"), 8) },
                { "Running", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightRunning"), 8) }
            });

            _Wolf = new Wolf(new Dictionary<string, Animation>()
            {
                { "Idle", new Animation(content.Load<Texture2D>("Sprites/Wolf/WolfIdle"), 4) },
                { "Running", new Animation(content.Load<Texture2D>("Sprites/Wolf/WolfRunning"), 4) }
            }, _Player)
            {
                Position = new Vector2(_Player.Position.X - 40, _Player.Position.Y + 15)
            };

            _Skeleton = new Skeleton(new Dictionary<string, Animation>()
            {
                //{ "Attack", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightAttack"), 4) },
                { "Idle", new Animation(content.Load<Texture2D>("Sprites/Skeleton/SkeletonIdle"), 4) },
                { "Running", new Animation(content.Load<Texture2D>("Sprites/Skeleton/SkeletonRunning"), 4) }
            })
            {
                Position = new Vector2(500, 500)
            };

            _SpriteList = new List<Sprite>()
            {
                _Wolf, _Skeleton, _Player
            };

            _Game = game;
            //m_MainFrame = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
            _PlayerStats = new PlayerStats()
            {
                Name = "Jovan",
                Score = 0,
            };

            _BackGroundCurrent = content.Load<Texture2D>("Backgrounds/level-sewer");
            _PunchSound = content.Load<SoundEffect>("Sounds/punch");
            _EndSound = content.Load<SoundEffect>("Sounds/end");
            _Font = content.Load<SpriteFont>("defaultFont");

            // TODO
            _BackgroundNext = content.Load<Texture2D>("Backgrounds/level-cyberpunk");

            // Solution for camera: add a custom spritebatch to the scene
            _Camera = new Camera();
            _SpriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        public void Save(PlayerStats stats)
        {
            // This can be list so that we can have more Players/Enemies
            string serializedText = JsonSerializer.Serialize<PlayerStats>(stats);

            // This doesn't append (need to use "append" something)
            File.WriteAllText(_StatsPath, serializedText);
        }

        // Called in create
        public void Load()
        {
            var fileContent = File.ReadAllText(_StatsPath);
            _PlayerStats = JsonSerializer.Deserialize<PlayerStats>(fileContent);
            _Destroyed = _PlayerStats.Score;
        }

        void EndGame()
        {
            _EndSound.Play();
            System.Threading.Thread.Sleep(1000);

            // Exit to menu when game ends
            _Game.ChangeStateMenu();
        }

        internal void Update(GameTime gameTime)
        {
            foreach (var sprite in _SpriteList)
                sprite.Update(gameTime);

            _Camera.Follow(_Player);
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
            _SpriteBatch.Begin(transformMatrix: _Camera.Transform);
            // Background
            _SpriteBatch.Draw(_BackGroundCurrent, _BackGroundCurrent.Bounds, Color.White);

            // Sprites
            foreach (var sprite in _SpriteList)
                sprite.Draw(gameTime, _SpriteBatch);
            
            _SpriteBatch.End();

            // Scoreboard (in the old spriteBatch)
            spriteBatch.DrawString(_Font, _Destroyed.ToString(), new Vector2(10, 10), Color.White);
        }
    }
}
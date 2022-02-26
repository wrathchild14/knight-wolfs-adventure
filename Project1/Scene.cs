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
    internal class Scene
    {
        private readonly Game1 _Game;

        private Knight player_knight_;
        private Sprite wolf_dog_;

        private int destroyed_ = 0;
        private PlayerStats player_stats_;
        private Texture2D background_current_;
        private Texture2D _BackgroundNext;
        private string stats_path_ = "Stats.json";

        // Content loaders
        private SoundEffect punch_sound_;

        private SoundEffect end_sound_;
        private SpriteFont Font;

        // Custom spritebatch for the scene
        private Camera camera_;

        private SpriteBatch sprite_batch_;

        private List<Sprite> sprite_list_;

        public Scene(Game1 game, ContentManager content)
        {
            // Creating of our player which is a Knight + Wolf
            // Idk how this should work, for now we just take the input for both
            player_knight_ = new Knight(content.Load<Texture2D>("DebugRectangle"), new Dictionary<string, Animation>()
            {
                { "Attack", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightAttack"), 15) },
                { "Pray", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightPray"), 12) },
                { "Idle", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightIdle"), 8) },
                { "Running", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightRunning"), 8) }
            });

            wolf_dog_ = new Wolf(new Dictionary<string, Animation>()
            {
                { "Idle", new Animation(content.Load<Texture2D>("Sprites/Wolf/WolfIdle"), 4) },
                { "Running", new Animation(content.Load<Texture2D>("Sprites/Wolf/WolfRunning"), 4) }
            }, player_knight_)
            {
                Position = new Vector2(player_knight_.Position.X - 40, player_knight_.Position.Y + 15)
            };

            sprite_list_ = new List<Sprite>()
            {
                wolf_dog_, player_knight_,
                new Skeleton(content.Load<Texture2D>("Sprites/Healthbar"), player_knight_, new Dictionary<string, Animation>()
                {
                    //{ "Attack", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightAttack"), 4) },
                    { "Dead", new Animation(content.Load<Texture2D>("Sprites/Skeleton/SkeletonDeath"), 4)},
                    { "Idle", new Animation(content.Load<Texture2D>("Sprites/Skeleton/SkeletonIdle"), 4)},
                    { "Running", new Animation(content.Load<Texture2D>("Sprites/Skeleton/SkeletonRunning"), 4)},
                    { "Attacked", new Animation(content.Load<Texture2D>("Sprites/Skeleton/SkeletonAttacked"), 4)}
                })
                {
                    Position = new Vector2(200, 100)
                },
                new Skeleton(content.Load<Texture2D>("Sprites/Healthbar"), player_knight_, new Dictionary<string, Animation>()
                {
                    //{ "Attack", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightAttack"), 4) },
                    { "Dead", new Animation(content.Load<Texture2D>("Sprites/Skeleton/SkeletonDeath"), 4)},
                    { "Idle", new Animation(content.Load<Texture2D>("Sprites/Skeleton/SkeletonIdle"), 4)},
                    { "Running", new Animation(content.Load<Texture2D>("Sprites/Skeleton/SkeletonRunning"), 4)},
                    { "Attacked", new Animation(content.Load<Texture2D>("Sprites/Skeleton/SkeletonAttacked"), 4)},
                })
                {
                    Position = new Vector2(300, 200)
                }
            };

            _Game = game;
            //m_MainFrame = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
            player_stats_ = new PlayerStats()
            {
                Name = "Jovan",
                Score = 0,
            };

            background_current_ = content.Load<Texture2D>("Backgrounds/level-sewer");
            punch_sound_ = content.Load<SoundEffect>("Sounds/punch");
            end_sound_ = content.Load<SoundEffect>("Sounds/end");
            Font = content.Load<SpriteFont>("defaultFont");

            // TODO
            _BackgroundNext = content.Load<Texture2D>("Backgrounds/level-cyberpunk");

            // Solution for camera: add a custom spritebatch to the scene
            camera_ = new Camera();
            sprite_batch_ = new SpriteBatch(game.GraphicsDevice);
        }

        public void Save(PlayerStats stats)
        {
            // This can be list so that we can have more Players/Enemies
            string serializedText = JsonSerializer.Serialize<PlayerStats>(stats);

            // This doesn't append (need to use "append" something)
            File.WriteAllText(stats_path_, serializedText);
        }

        // Called in create
        public void Load()
        {
            var fileContent = File.ReadAllText(stats_path_);
            player_stats_ = JsonSerializer.Deserialize<PlayerStats>(fileContent);
            destroyed_ = player_stats_.Score;
        }

        private void EndGame()
        {
            end_sound_.Play();
            System.Threading.Thread.Sleep(1000);

            // Exit to menu when game ends
            _Game.ChangeStateMenu();
        }

        internal void Update(GameTime gameTime)
        {
            foreach (var sprite in sprite_list_)
                sprite.Update(gameTime);

            camera_.Follow(player_knight_);
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
            sprite_batch_.Begin(transformMatrix: camera_.Transform);
            // Background
            sprite_batch_.Draw(background_current_, background_current_.Bounds, Color.White);

            // Sprites
            foreach (var sprite in sprite_list_)
                sprite.Draw(gameTime, sprite_batch_);

            sprite_batch_.End();

            // Scoreboard (in the old spriteBatch)
            spriteBatch.DrawString(Font, destroyed_.ToString(), new Vector2(10, 10), Color.White);
        }
    }
}
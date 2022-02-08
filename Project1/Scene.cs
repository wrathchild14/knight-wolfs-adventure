using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Project1
{
    // Class serves to enemies, player and gameplay. Doesn't draw the background
    class Scene
    {
        readonly Game1 _game;

        private List<Enemy> _enemies = new List<Enemy>();
        private readonly Player _player;
        private Random _random = new Random();
        private int _destroyed = 0;
        private Rectangle _mainFrame;
        private PlayerStats _player_stats;
        private Texture2D _background_current;
        private Texture2D _background_next;
        private string _stats_path = "stats.json";

        // Content loaders
        private SoundEffect _punch_sound;
        private SoundEffect _end_sound;
        private SpriteFont _font;

        private Camera _camera;
        private SpriteBatch _sprite_batch;

        public Scene(Game1 game, ContentManager content)
        {
            _player = new Player(game);
            _game = game;
            _mainFrame = new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
            _player_stats = new PlayerStats()
            {
                Name = "Jovan",
                Score = 0,
            };

            _background_current = content.Load<Texture2D>("Backgrounds/level-sewer");
            _punch_sound = content.Load<SoundEffect>("Sounds/punch");
            _end_sound = content.Load<SoundEffect>("Sounds/end");
            _font = content.Load<SpriteFont>("defaultFont");
            
            // TODO
            _background_next = content.Load<Texture2D>("Backgrounds/level-cyberpunk");
            
            // Solution for camera: add a custom spritebatch to the scene
            _camera = new Camera();
            _sprite_batch = new SpriteBatch(game.GraphicsDevice);
        }

        public void Save(PlayerStats stats)
        {
            // This can be list so that we can have more Players/Enemies
            string serializedText = JsonSerializer.Serialize<PlayerStats>(stats);

            // This doesn't append (need to use "append" something)
            File.WriteAllText(_stats_path, serializedText);
        }

        // Called in create
        public void Load()
        {
            var fileContent = File.ReadAllText(_stats_path);
            _player_stats = JsonSerializer.Deserialize<PlayerStats>(fileContent);
            _destroyed = _player_stats.Score;
        }

        void EndGame()
        {
            _end_sound.Play();
            System.Threading.Thread.Sleep(1000);

            // Exit to menu when game ends
            _game.ChangeStateMenu();
        }

        internal void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
            _camera.Follow(_player);

            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i] != null)
                {
                    _enemies[i].Update(gameTime);

                    // Making a rectangle to make collisions better
                    //Rectangle collisionRect = _player.Rectangle;
                    //collisionRect.Width -= 50;
                    //collisionRect.Height -= 50;
                    //if (collisionRect.Intersects(_enemies[i].Rectangle))
                    if (_player.IsTouching(_enemies[i]))
                    {
                        if (_player.punching)
                        {
                            _enemies[i] = null;
                            _enemies.Remove(_enemies[i]);
                            _destroyed++;
                            _punch_sound.Play();

                            // Maybe improve this
                            _player_stats.Score = _destroyed;
                        }
                        else
                        {
                            Save(_player_stats);
                            EndGame();
                        }
                    }
                }
            }

            // Some end game mechanic
            if (_destroyed < 20)
            {
                if (_enemies.Count < 10)
                {
                    _enemies.Add(new Enemy(_game, _random.Next(_mainFrame.Width - 50, _mainFrame.Width + 200), _random.Next(200, 400), _player));
                }
            }

            // TODO
            if (_destroyed > 10)
            {
                _background_current = _background_next;
            }

            if (_enemies.Count == 0)
            {
                _game.ChangeStateEnd();
            }
        }

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _sprite_batch.Begin(transformMatrix: _camera.Transform);
            // Background
            _sprite_batch.Draw(_background_current, _mainFrame, Color.White);

            // Sprites
            _player.Draw(gameTime, _sprite_batch);
            foreach (var enemy in _enemies)
                enemy.Draw(gameTime, _sprite_batch);

            // Scoreboard, make it stick to the camera
            float x = 10;
            float y = 10;
            _sprite_batch.DrawString(_font, _destroyed.ToString(), new Vector2(x,y), Color.White);
            _sprite_batch.End();
        }
    }
}
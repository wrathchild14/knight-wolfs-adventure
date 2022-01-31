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
        private List<Enemy> _enemies = new List<Enemy>();
        private readonly Player _player;
        private static readonly Random _random = new Random();
        private int _destroyed = 0;
        private Rectangle _mainFrame;
        readonly Game1 _game;
        private PlayerStats _pStats;
        private Texture2D _background;
        private const string path = "stats.json";

        // Content loaders
        public SoundEffect PunchSound;
        public SoundEffect EndSound;
        public SpriteFont Font;
        private ContentManager _content;

        public Rectangle MainFrame { get => _mainFrame; set => _mainFrame = value; }

        public Scene(Game1 game, Player player, Rectangle mainFrame, ContentManager content)
        {
            _content = content;
            _player = player;
            _game = game;
            _mainFrame = mainFrame;
            _pStats = new PlayerStats()
            {
                Name = "Jovan",
                Score = 0,
            };

            _background = content.Load<Texture2D>("level-sewer");
        }

        public void Save(PlayerStats stats)
        {
            // This can be list so that we can have more Players/Enemies
            string serializedText = JsonSerializer.Serialize<PlayerStats>(stats);

            // This doesn't append (need to use "append" something)
            File.WriteAllText(path, serializedText);
        }

        // Called in create
        public void Load()
        {
            var fileContent = File.ReadAllText(path);
            _pStats = JsonSerializer.Deserialize<PlayerStats>(fileContent);
            _destroyed = _pStats.Score;
        }

        void EndGame()
        {
            EndSound.Play();
            System.Threading.Thread.Sleep(1000);

            // Exit to menu when game ends
            _game.ChangeStateMenu();
        }

        internal void Update(GameTime gameTime)
        {
            _player.Update(gameTime);

            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i] != null)
                {
                    _enemies[i].Update(gameTime);

                    // Making a rectangle to make collisions better
                    Rectangle collisionRect = _player.Rect;
                    collisionRect.Width -= 50;
                    collisionRect.Height -= 50;
                    if (collisionRect.Intersects(_enemies[i].Rect))
                    {
                        if (_player.punching)
                        {
                            _enemies[i] = null;
                            _enemies.Remove(_enemies[i]);
                            _destroyed++;
                            PunchSound.Play();

                            // Maybe improve this
                            _pStats.Score = _destroyed;
                        }
                        else
                        {
                            Save(_pStats);
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
                _background = _content.Load<Texture2D>("level-cyberpunk");
            }
            
            if (_enemies.Count == 0)
            {
                _game.ChangeStateEnd();
            }
        }

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, _mainFrame, Color.White);

            foreach (var enemy in _enemies)
                enemy.Draw(spriteBatch);

            _player.Draw(spriteBatch);

            // Scoreboard
            spriteBatch.DrawString(Font, _destroyed.ToString(), new Vector2(10, 10), Color.White);
        }
    }
}
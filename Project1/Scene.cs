using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project1
{
    class Scene
    {
        private List<Enemy> enemies = new List<Enemy>();
        private List<Enemy> standingEnemies = new List<Enemy>();
        private readonly Player player;
        private readonly SpriteBatch _spriteBatch;
        private static readonly Random r = new Random();
        private readonly SpriteFont font;
        private int destroyed = 0;
        private Rectangle mainFrame;
        readonly Game1 game;

        public SoundEffect punch;
        public SoundEffect end;

        public bool endGameBool = false;

        public Rectangle MainFrame { get => mainFrame; set => mainFrame = value; }

        public Scene(Game1 game, Player player, int enemyCount, Rectangle mainFrame, SpriteBatch _spriteBatch, SpriteFont myFont)
        {
            this.player = player;
            this._spriteBatch = _spriteBatch;
            this.game = game;
            font = myFont;
            this.MainFrame = mainFrame;

            // Stick to using the < 6 count
            /*
            for (int i = 0; i < enemyCount; i++)
            {
                enemies.Add(new Enemy(game,
                                      r.Next(400, 900),
                                      r.Next(200, 400),
                                      false,
                                      this.player));
            }
            */
        }

        void endGame()
        {
            endGameBool = true;
            end.Play();
            System.Threading.Thread.Sleep(2000);

            player.Position = new Vector2(100, 100);
            enemies.Clear();
            destroyed = 0;
        }

        internal void Update(GameTime gameTime)
        {
            if (!endGameBool) { 
                player.Update(gameTime);

                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i] != null)
                    {
                        enemies[i].Update(gameTime);

                        Rectangle collisionRect = player.Rect;
                        collisionRect.Width -= 50;
                        collisionRect.Height -= 50;
                        // if (player.Rect.Intersects(enemies[i].Rect))
                        if (collisionRect.Intersects(enemies[i].Rect))
                        {
                            if (player.punching) { 
                                enemies[i] = null;
                                enemies.Remove(enemies[i]);
                                destroyed++;
                                punch.Play();
                            } else
                            {
                                // end.Play();
                                // endGameBool = true;
                                endGame();
                                // game.Exit();
                            }
                        }
                    }
                }

                if (enemies.Count < 6)
                {
                    enemies.Add(new Enemy(game, r.Next(MainFrame.Width - 50, MainFrame.Width + 200), r.Next(200, 400), false, player));
                }
            }
            /*
            else
            {
                
                game.Exit();
            }
            */
        }

        internal void Draw(GameTime gameTime)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(_spriteBatch);
            }

            for (int i = 0; i < standingEnemies.Count; i++)
            {
                standingEnemies[i].Draw(_spriteBatch);
            }

            player.Draw(_spriteBatch);

            _spriteBatch.DrawString(font, destroyed.ToString(), new Vector2(10, 10), Color.White);
        }
    }
}
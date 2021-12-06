using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Project1
{
    class Scene
    {
        private List<Enemy> enemies = new List<Enemy>();
        private List<Enemy> standingEnemies = new List<Enemy>();
        private Player player;
        private SpriteBatch _spriteBatch;
        private Random r = new Random();
        private SpriteFont font;
        private int destroyed = 0;
        private Rectangle mainFrame;
        Game1 game;

        public Scene(Game1 game, Player player, int v1, int v2, Rectangle mainFrame, SpriteBatch _spriteBatch, SpriteFont myFont)
        {
            this.player = player;
            this._spriteBatch = _spriteBatch;
            this.game = game;
            this.font = myFont;
            this.mainFrame = mainFrame;

            for (int i = 0; i < v1; i++)
            {
                enemies.Add(new Enemy(game, r.Next(400, 900), r.Next(200, 400), false));
            }

            for (int i = 0; i < v2; i++)
            {
                standingEnemies.Add(new Enemy(game, r.Next(400, this.mainFrame.Width), r.Next(200, 400), true));
            }
        }

        internal void update(GameTime gameTime)
        {
            player.Update(gameTime);

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] != null)
                {
                    enemies[i].Update(gameTime);
                    // if (player.Rect.Intersects(enemies[i].Rect) && player.punching)
                    if (player.Rect.Intersects(enemies[i].Rect))
                    {
                        if (player.punching) { 
                            enemies[i] = null;
                            enemies.Remove(enemies[i]);
                            destroyed++;
                        } else
                        {
                            game.Exit();
                        }
                    }
                }
            }

            for (int i = 0; i < standingEnemies.Count; i++)
            {
                if (standingEnemies[i] != null)
                {
                    standingEnemies[i].Update(gameTime);
                    // if (player.Rect.Intersects(standingEnemies[i].Rect) && player.punching)
                    if (player.Rect.Intersects(standingEnemies[i].Rect) && player.punching)
                    {
                        if (player.punching)
                        {
                            standingEnemies[i] = null;
                            standingEnemies.Remove(standingEnemies[i]);
                            destroyed++;
                        } else
                        {
                            game.Exit();
                        }
                    }
                }
            }

            if (enemies.Count < 4)
            {
                enemies.Add(new Enemy(this.game, r.Next(this.mainFrame.Width, this.mainFrame.Width + 200), r.Next(200, 400), false));
            }
        }

        internal void draw(GameTime gameTime)
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

            _spriteBatch.DrawString(this.font, destroyed.ToString(), new Vector2(10, 10), Color.White);
        }
    }
}
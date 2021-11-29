using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Project1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Rectangle mainFrame;
        private Player player;
        private List<Enemy> enemies = new List<Enemy>();
        private Texture2D background;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize(); // this must be here
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            mainFrame = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            player = new Player(this);
            
            // generating enemies
            Random r = new Random();
            for (int i = 0; i < 3; i++)
            {
                enemies.Add(new Enemy(this, r.Next(400, 900), r.Next(200, 400), false));
            }

            for (int i = 0; i < 2; i++)
            {
                enemies.Add(new Enemy(this, r.Next(400, mainFrame.Width), r.Next(200, 400), true));
            }

        }

        protected override void LoadContent()
        {
            background = Content.Load<Texture2D>("level-sewer");
        }

        protected override void Update(GameTime gameTime)
        {
            player.Update(gameTime);
            
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] != null)
                {
                    enemies[i].Update(gameTime);
                    if (player.Rect.Intersects(enemies[i].Rect) && player.punching)
                    {
                        enemies[i] = null;
                        enemies.Remove(enemies[i]);
                    }
                }
            }
            //base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            _spriteBatch.Begin();
            _spriteBatch.Draw(background, mainFrame, Color.White);
            
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(_spriteBatch);
            }
            
            player.Draw(_spriteBatch);
            _spriteBatch.End();
            //base.Draw(gameTime);
        }
    }
}

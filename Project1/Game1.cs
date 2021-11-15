using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D texture;
        private Texture2D background;
        private Vector2 position;

        private float speedX = 3.6f;
        private float speedY = 2.5f;

        private float allowedY;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("level-sewer");
            texture = Content.Load<Texture2D>("enemy-wolf");
            position = new Vector2(_graphics.GraphicsDevice.Viewport.Width/2-texture.Width/2,
                _graphics.GraphicsDevice.Viewport.Height/2-texture.Height/2 + 100); // lil bit of offset so he starts more down

            allowedY = background.Height / 2 - texture.Height / 2;
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (position.Y > allowedY)
                {
                    position.Y -= speedY;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                position.Y += speedY;

            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                position.X -= speedX;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                position.X += speedX;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            _spriteBatch.Begin();
            _spriteBatch.Draw(background, Vector2.Zero, Color.White);
            _spriteBatch.Draw(texture, position, Color.White);
            _spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

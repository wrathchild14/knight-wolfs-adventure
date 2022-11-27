using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TileMap
{
    public class Tile
    {
        protected Texture2D texture;

        public Rectangle Rectangle { get; protected set; }

        public static ContentManager Content { get; set; }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Rectangle, Color.White);
        }
    }
}
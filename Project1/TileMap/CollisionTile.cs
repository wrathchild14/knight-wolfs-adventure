using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileMap
{
    public class CollisionTile : Tile
    {
        public CollisionTile(int i, Rectangle newRectangle, string path)
        {
            texture = Content.Load<Texture2D>(path + "/Tile" + i);
            Rectangle = newRectangle;
            Id = i;
        }

        public int Id { get; }
    }
}
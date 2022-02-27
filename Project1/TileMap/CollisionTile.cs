using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.TileMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1.TileMap
{
    class CollisionTile : Tile
    {
        public CollisionTile(int i, Rectangle new_rectangle)
        {
            texture = Content.Load<Texture2D>("Tiles/Tile" + i);
            Rectangle = new_rectangle;
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.TileMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1.TileMap
{
    public class CollisionTile : Tile
    {
        private int id_;

        public int Id
        {
            get { return id_; }
        }

        public CollisionTile(int i, Rectangle new_rectangle, String path)
        {
            texture = Content.Load<Texture2D>(path + "/Tile" + i);
            Rectangle = new_rectangle;
            id_ = i;
        }
    }
}
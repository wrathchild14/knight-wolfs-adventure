using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Models;
using Project1.Sprites;
using Project1.TileMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1.TileMap
{
    public class Map
    {
        private List<CollisionTile> collisionTiles_ = new List<CollisionTile>();

        public List<CollisionTile> CollisionTiles
        {
            get { return collisionTiles_; }
        }

        private int width_, height_;
        private List<Skeleton> enemies_ = new List<Skeleton>();
        private Dictionary<string, Texture2D> skeleton_tex_for_animation_;
        private Texture2D healthbar_tex_;
        private Knight player_;
        private Texture2D debug_;

        public List<Skeleton> GetEnemies()
        {
            return enemies_;
        }

        public int Width
        {
            get { return width_; }
        }

        public int Height
        {
            get { return height_; }
        }

        public Map(Texture2D debug, Dictionary<string, Texture2D> skeletonTexForAnimation, Texture2D healthBarTex, Knight player_knight)
        {
            skeleton_tex_for_animation_ = skeletonTexForAnimation;
            healthbar_tex_ = healthBarTex;
            player_ = player_knight;
            debug_ = debug;
        }

        public void Generate(int[,] map, int size, String path, int skeletonFollowDist)
        {
            for (int x = 0; x < map.GetLength(1); x++)
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    int number = map[y, x];

                    if (number == 99)
                    {
                        // Basic block behind enemy
                        collisionTiles_.Add(new CollisionTile(1, new Rectangle(x * size, y * size, size, size), path));

                        Skeleton temp = new Skeleton(debug_, healthbar_tex_, player_, skeletonFollowDist, new Dictionary<string, Animation>()
                            {
                                { "Attack", new Animation(skeleton_tex_for_animation_["Attack"], 8) },
                                { "Dead", new Animation(skeleton_tex_for_animation_["Dead"], 4)},
                                { "Idle", new Animation(skeleton_tex_for_animation_["Idle"], 4)},
                                { "Running", new Animation(skeleton_tex_for_animation_["Running"], 4)},
                                { "Attacked", new Animation(skeleton_tex_for_animation_["Attacked"], 4)}
                            })
                        {
                            Position = new Vector2(x * size, y * size)
                        };
                        enemies_.Add(temp);
                    }
                    else if (number > 0)
                        collisionTiles_.Add(new CollisionTile(number, new Rectangle(x * size, y * size, size, size), path));

                    width_ = (x + 1) * size;
                    height_ = (y + 1) * size;
                }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (CollisionTile tile in collisionTiles_)
                tile.Draw(spriteBatch);
        }
    }
}
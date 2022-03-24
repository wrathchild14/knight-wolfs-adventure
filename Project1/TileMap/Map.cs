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
    class Map
    {
        private List<CollisionTile> collison_tiles_ = new List<CollisionTile>();

        public List<CollisionTile> CollisionTiles
        {
            get { return collison_tiles_; }
        }

        private int width_, height_;
        private List<Skeleton> enemies_ = new List<Skeleton>();
        private Dictionary<string, Texture2D> skeleton_textures_for_animation_;
        private Texture2D healthbar_texture_;
        private Knight player_knight_;
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

        public Map(Texture2D debug, Dictionary<string, Texture2D> skeleton_textures_for_animation, Texture2D healthbar_texture, Knight player_knight)
        {
            skeleton_textures_for_animation_ = skeleton_textures_for_animation;
            healthbar_texture_ = healthbar_texture;
            player_knight_ = player_knight;
            debug_ = debug;
        }

        public void Generate(int[,] map, int size, String path, int skeleton_follow_distance)
        {
            for (int x = 0; x < map.GetLength(1); x++)
                for (int y = 0; y < map.GetLength(0); y++)
                {
                    int number = map[y, x];

                    if (number == 99)
                    {
                        // Basic block behind enemy
                        collison_tiles_.Add(new CollisionTile(1, new Rectangle(x * size, y * size, size, size), path));

                        Skeleton temp = new Skeleton(debug_, healthbar_texture_, player_knight_, skeleton_follow_distance, new Dictionary<string, Animation>()
                            {
                                { "Attack", new Animation(skeleton_textures_for_animation_["Attack"], 8) },
                                { "Dead", new Animation(skeleton_textures_for_animation_["Dead"], 4)},
                                { "Idle", new Animation(skeleton_textures_for_animation_["Idle"], 4)},
                                { "Running", new Animation(skeleton_textures_for_animation_["Running"], 4)},
                                { "Attacked", new Animation(skeleton_textures_for_animation_["Attacked"], 4)}
                            })
                        {
                            Position = new Vector2(x * size, y * size)
                        };
                        enemies_.Add(temp);
                    }
                    else if (number > 0)
                        collison_tiles_.Add(new CollisionTile(number, new Rectangle(x * size, y * size, size, size), path));

                    width_ = (x + 1) * size;
                    height_ = (y + 1) * size;
                }
        }

        public void Draw(SpriteBatch sprite_batch)
        {
            foreach (CollisionTile tile in collison_tiles_)
                tile.Draw(sprite_batch);
        }
    }
}
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project1.Models;
using Project1.Sprites;

namespace Project1.TileMap
{
    public class Map
    {
        private readonly Texture2D debug_;
        private readonly List<Skeleton> enemies_ = new List<Skeleton>();
        private readonly Texture2D healthbarTex_;
        private readonly Knight player_;
        private readonly Dictionary<string, Texture2D> skeletonTexForAnimation;

        public Map(Texture2D debug, Dictionary<string, Texture2D> skeletonTexForAnimation, Texture2D healthBarTex,
            Knight playerKnight)
        {
            this.skeletonTexForAnimation = skeletonTexForAnimation;
            healthbarTex_ = healthBarTex;
            player_ = playerKnight;
            debug_ = debug;
        }

        public List<CollisionTile> CollisionTiles { get; } = new List<CollisionTile>();

        public int Width { get; private set; }

        public int Height { get; private set; }

        public List<Skeleton> GetEnemies()
        {
            return enemies_;
        }

        public void Generate(int[,] map, int size, string path, int skeletonFollowDist)
        {
            for (var x = 0; x < map.GetLength(1); x++)
            for (var y = 0; y < map.GetLength(0); y++)
            {
                var number = map[y, x];

                if (number == 99)
                {
                    // Basic block behind enemy
                    CollisionTiles.Add(new CollisionTile(1, new Rectangle(x * size, y * size, size, size), path));

                    var temp = new Skeleton(debug_, healthbarTex_, player_, skeletonFollowDist,
                        new Dictionary<string, Animation>
                        {
                            { "Attack", new Animation(skeletonTexForAnimation["Attack"], 8) },
                            { "Dead", new Animation(skeletonTexForAnimation["Dead"], 4) },
                            { "Idle", new Animation(skeletonTexForAnimation["Idle"], 4) },
                            { "Running", new Animation(skeletonTexForAnimation["Running"], 4) },
                            { "Attacked", new Animation(skeletonTexForAnimation["Attacked"], 4) }
                        })
                    {
                        Position = new Vector2(x * size, y * size)
                    };
                    enemies_.Add(temp);
                }
                else if (number > 0)
                {
                    CollisionTiles.Add(new CollisionTile(number, new Rectangle(x * size, y * size, size, size), path));
                }

                Width = (x + 1) * size;
                Height = (y + 1) * size;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tile in CollisionTiles)
                tile.Draw(spriteBatch);
        }
    }
}
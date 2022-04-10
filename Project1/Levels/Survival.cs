using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1.Components;
using Project1.Models;
using Project1.Sprites;
using Project1.TileMap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Project1.Levels
{
    class Survival : Level
    {
        private Random random = new Random();
        private int max_enemies_ = 20;

        public Survival(Game1 game, ContentManager content) : base(game, content)
        {
            light_mask = content.Load<Texture2D>("lightmask-1");

            player_knight_.Position = new Vector2(1540, 970);

            map_.Generate(new int[,]
            {
                { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 8,  7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 10, 10, 10, 10, 10, 10, 10, 9,  9,  10, 10, 8,  7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 10, 10, 10, 10, 10, 10, 10, 8,  7,  10, 10, 8,  7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 9,  9,  9,  9,  9,  9,  9,  1,  1,  9,  9,  4,  2,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9  },
                { 1,  1,  1, 13,  1,  3,  1,  4,  1,  1,  1, 13,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1  },
                { 2,  3,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 12,  1,  1,  1,  1,  1, 4  },
                { 1,  1,  5,  1,  3,  1,  4,  1,  1,  1,  4,  1,  1,  1,  1,  1,  1,  1,  1,  4,  1,  1,  1,  1  },
                { 1,  1,  1,  3,  1,  1,  1,  1,  1,  1,  1,  3,  1,  3, 13,  1,  1,  4,  1,  1,  5,  1,  1,  1  },
                { 1,  12, 1,  4,  6,  1,  2,  1,  5,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  2,  1,  6,  1,  1  },
                { 1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 14,  1,  1, 14,  1,  1,  1,  1,  1  },
                { 1,  1,  1,  1,  1,  1,  1,  1, 11, 11, 11,  4,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1  },
                { 11, 11, 11, 11, 11, 1,  1,  7, 10, 10, 10,  8,  1,  1,  1,  1,  1,  3,  1,  1, 99,  4,  1,  1  },
                { 10, 10, 10, 10, 10, 8, 11, 7, 10, 10, 10,  11, 1,  1, 13,  1,  1,  1,  4,  6,  1,  1, 99,  1  },
                { 10, 10, 10, 10, 10,10, 10, 10, 10, 10, 10,  10, 8,  11,  11, 11, 11, 11,  11,  11, 11, 11, 11, 11  },
                { 10, 10, 10, 10, 10,10, 10, 10, 10, 10, 10,  10, 10, 10,  10, 10, 10,  10, 10, 10,  10, 10, 10,  10  },
                { 10, 10, 10, 10, 10,10, 10, 10, 10, 10, 10,  10, 10, 10,  10, 10, 10,  10, 10, 10,  10, 10, 10,  10  },

            }, 128, "Tiles/Forest", 1200);
            // Get generated enemies from map
            enemies_ = map_.GetEnemies();
            camera_ = new Camera(map_.Width, map_.Height);

            dialog_box_ = new DialogBox(game_, Font)
            {
                Text = "This is survival mode, fight as many enemies as you can, they won't stop coming"
            };
            dialog_box_.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            if (destroyed_ > 10)
                max_enemies_ = 30;
            else if (destroyed_ > 30)
                max_enemies_ = 50;
            else if (destroyed_ > 50)
                max_enemies_ = 100;

            if (enemies_.Count < max_enemies_)
                enemies_.Add(new Skeleton(game_.Content.Load<Texture2D>("DebugRectangle"),
                    game_.Content.Load<Texture2D>("Sprites/Healthbar"),
                    player_knight_,
                    1200,
                    new Dictionary<string, Animation>()
                            {
                                { "Attack", new Animation(game_.Content.Load<Texture2D>("Sprites/Skeleton/SkeletonAttack"), 8) },
                                { "Dead", new Animation(game_.Content.Load<Texture2D>("Sprites/Skeleton/SkeletonDeath"), 4)},
                                { "Idle", new Animation(game_.Content.Load<Texture2D>("Sprites/Skeleton/SkeletonIdle"), 4)},
                                { "Running", new Animation(game_.Content.Load<Texture2D>("Sprites/Skeleton/SkeletonRunning"), 4)},
                                { "Attacked", new Animation(game_.Content.Load<Texture2D>("Sprites/Skeleton/SkeletonAttacked"), 4)}
                            })
                {
                    Position = new Vector2(random.Next(120, 3000), random.Next(450, 1500))
                });
            
            
            for(int i = 0; i < enemies_.Count; i++)
            {
                enemies_[i].Update(gameTime);
                if (enemies_[i].Dead) {
                    destroyed_++;
                    enemies_.RemoveAt(i);
                }
            }

            // Physics
            foreach (CollisionTile tile in map_.CollisionTiles)
            {
                if (tile.Id >= 10 && tile.Id != 15) // Temp
                    player_knight_.Collision(tile, map_.Width, map_.Height);

                foreach (var enemy in enemies_)
                    if (tile.Id >= 10 && tile.Id != 15) // Temp
                        enemy.Collision(tile, map_.Width, map_.Height);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }

        public void Save(PlayerStats stats)
        {
            // This can be list so that we can have more Players/Enemies
            string serializedText = JsonSerializer.Serialize<PlayerStats>(stats);

            // This doesn't append (need to use "append" something)
            File.WriteAllText(stats_path_, serializedText);
        }

        // Called in create
        public override void Load()
        {
            var fileContent = File.ReadAllText(stats_path_);
            player_stats_ = JsonSerializer.Deserialize<PlayerStats>(fileContent);
            destroyed_ = player_stats_.Score;
        }
    }
}

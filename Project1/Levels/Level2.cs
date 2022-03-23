using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project1.Models;
using Project1.Sprites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using Project1.TileMap;
using Project1.Levels;

namespace Project1
{
    internal class Level2 : Level
    {
        private Game1 game_;

        private Knight player_knight_;
        private Sprite wolf_dog_;

        private int destroyed_ = 0;
        private PlayerStats player_stats_;
        private string stats_path_ = "Stats.json";

        private SoundEffect punch_sound_;
        private SoundEffect end_sound_;
        private SpriteFont Font;

        private Camera camera_;
        private SpriteBatch sprite_batch_;
        private List<Sprite> player_sprite_list_;
        private List<Skeleton> enemies_ = new List<Skeleton>();

        private Map map_;

        public Level2(Game1 game, ContentManager content)
        {
            Texture2D debug = content.Load<Texture2D>("DebugRectangle");
            sprite_batch_ = new SpriteBatch(game.GraphicsDevice);
            // Content loaders
            Texture2D healthbar_texture = content.Load<Texture2D>("Sprites/Healthbar");
            // Enemy (which is generated in Map.cs)
            Dictionary<string, Texture2D> skeleton_textures_for_animations = new Dictionary<string, Texture2D>()
            {
                { "Attack", content.Load<Texture2D>("Sprites/Skeleton/SkeletonAttack") },
                { "Dead", content.Load<Texture2D>("Sprites/Skeleton/SkeletonDeath")},
                { "Idle", content.Load<Texture2D>("Sprites/Skeleton/SkeletonIdle")},
                { "Running", content.Load<Texture2D>("Sprites/Skeleton/SkeletonRunning")},
                { "Attacked", content.Load<Texture2D>("Sprites/Skeleton/SkeletonAttacked")}
            };
            // Player
            player_knight_ = new Knight(debug, healthbar_texture, new Dictionary<string, Animation>()
            {
                { "Dead", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightDeath"), 4) },
                { "Attack", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightAttack"), 15) },
                { "Pray", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightPray"), 12) },
                { "Idle", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightIdle"), 8) },
                { "Running", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightRunning"), 8) }
            })
            { Position = new Vector2(50, 400) };
            // Dog
            wolf_dog_ = new Wolf(new Dictionary<string, Animation>()
            {
                { "Idle", new Animation(content.Load<Texture2D>("Sprites/Wolf/WolfIdle"), 4) },
                { "Running", new Animation(content.Load<Texture2D>("Sprites/Wolf/WolfRunning"), 4) }
            }, player_knight_)
            {
                Position = new Vector2(player_knight_.Position.X - 40, player_knight_.Position.Y + 15)
            };
            player_sprite_list_ = new List<Sprite>()
            {
                wolf_dog_, player_knight_
            };

            player_stats_ = new PlayerStats()
            {
                Name = "Jovan",
                Score = 0,
            };

            punch_sound_ = content.Load<SoundEffect>("Sounds/punch");
            end_sound_ = content.Load<SoundEffect>("Sounds/end");
            Font = content.Load<SpriteFont>("defaultFont");

            Tile.Content = content;

            map_ = new Map(debug, skeleton_textures_for_animations, healthbar_texture, player_knight_);
            map_.Generate(new int[,]
            {
                { 1,  1,  4,  1,  1,  5,  1,  1,  4,  1,  5,  1,  2,  9,  1,  1,  2,  1,  1,  4,  1,  1,  1,  1  },
                { 1,  3,  1,  2,  2,  1,  3,  1,  2,  1,  1,  3,  1,  2,  1,  1,  3,  1,  5,  1,  1,  1,  1,  1  },
                { 11,11, 11, 11, 14, 13, 13, 13, 13, 13, 13, 12, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11 },
                { 1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  9,  1,  1,  1,  1,  1,  1,  1,  1  },
                { 1,  1,  1,  1,  2,  1,  1,  5,  1,  1,  2,  1,  1,  1,  1,  1,  1,  1,  4,  1,  1,  1,  1,  1  },
                { 2,  1,  6,  1,  1,  1,  1,  1,  1,  1,  1,  4,  1,  1,  1,  8,  1,  1,  1,  1,  1,  1,  1,  1  },
                { 1,  1,  7,  1,  1,  1,  1,  5,  1,  1, 19, 20, 19, 20,  1,  1, 10,  1,  6, 99,  3,  1,  1,  1  },
                { 1,  1,  7,  1,  3,  1,  1,  1,  1,  1, 17, 18, 17, 18,  1,  1,  1,  1,  7, 99,  1,  2,  1,  1  },
                { 1,  1,  7,  1,  1,  1,  3,  3,  1,  1, 15, 16, 15, 16,  1,  1,  1,  1,  7,  1,  1,  5,  1,  1  },
                { 1,  1,  7,  1, 10,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 10,  1,  1,  7,  1,  1,  1,  1,  1  },
                { 1,  1,  7,  1,  1,  1,  1,  1,  1,  2,  1,  5,  1,  1,  1,  1,  6,  1, 99,  1,  1,  1,  1,  1  },
                { 1,  1,  7,  1,  1,  1,  1,  9,  1,  1,  4,  1,  1,  3,  1,  2,  7,  2,  9,  1,  1,  1,  1,  1  },
                { 1,  1,  7,  1,  4,  1,  1,  9,  9,  3,  1,  1,  2,  1,  1,  4,  7,  1, 10,  1,  1,  1,  1,  1  },
                { 1,  8,  7,  1,  1,  1, 10,  4,  5,  1,  2,  1,  1, 10,  1,  4,  7,  4,  1,  1,  8,  1,  1,  1  },
                { 1,  1,  7,  1,  5,  1,  1,  1,  2,  1,  1,  3,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1  },
            }, 128, "Tiles/Town");
            // Get generated enemies from map
            enemies_ = map_.GetEnemies();
            camera_ = new Camera(map_.Width, map_.Height);
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

        private void EndGame()
        {
            end_sound_.Play();
            System.Threading.Thread.Sleep(1000);

            // Exit to menu when game ends
            game_.ChangeStateMenu();
        }

        public override void Update(GameTime gameTime)
        {
            // Sprites
            foreach (var sprite in player_sprite_list_)
                sprite.Update(gameTime);

            foreach (Skeleton enemy in enemies_)
                enemy.Update(gameTime);

            // Camera
            camera_.Update(player_knight_);

            // Physics
            foreach (CollisionTile tile in map_.CollisionTiles)
                if (tile.Id >= 8) // Temp
                    player_knight_.Collision(tile, map_.Width, map_.Height);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Camera
            sprite_batch_.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera_.ViewMatrix);

            // Tilemap
            map_.Draw(sprite_batch_);

            // Sprites
            foreach (var sprite in player_sprite_list_)
                sprite.Draw(gameTime, sprite_batch_);

            foreach (var enemy in enemies_)
                enemy.Draw(gameTime, sprite_batch_);

            sprite_batch_.End();

            // Scoreboard (in the old spriteBatch)
            spriteBatch.DrawString(Font, destroyed_.ToString(), new Vector2(10, 10), Color.White);
        }
    }
}
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
using Project1.Components;
using Microsoft.Xna.Framework.Input;

namespace Project1
{
    internal class Level1
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
        private DialogBox dialog_box_;
        private SpriteBatch sprite_batch_;
        private List<Sprite> player_sprite_list_;
        private List<Skeleton> enemies_ = new List<Skeleton>();

        private Map map_;

        public Level1(Game1 game, ContentManager content)
        {
            // Can get rid of this private
            game_ = game;

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

            map_ = new Map(debug ,skeleton_textures_for_animations, healthbar_texture, player_knight_);
            map_.Generate(new int[,]
            {
                { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 8,  7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 10, 10, 10, 10, 10, 10, 10, 9,  9,  10, 10, 8,  7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 10, 10, 10, 10, 10, 10, 10, 8,  7,  10, 10, 8,  7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 9,  9,  9,  9,  9,  9,  9,  1,  1,  9,  9,  1,  2,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9  },
                { 1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1  },
                { 1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1  },
                { 1,  1,  5,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  4,  1,  1,  1,  1  },
                { 1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  3,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1  },
                { 1,  12, 1,  1,  6,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1  },
                { 1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 14,  1,  1,  1,  1,  1,  1,  1,  1  },
                { 1,  1,  1,  1,  1,  1,  1,  1, 11, 11, 11,  4,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1  },
                { 11, 11, 11, 11, 11, 1,  1,  7, 10, 10, 10,  8,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1  },
                { 10, 10, 10, 10, 10, 8,  14, 7, 10, 10, 10,  11, 1,  1, 13,  1,  1,  1,  1,  1,  1,  1,  1,  1  },
                { 10, 10, 10, 10, 10, 8,  4, 7,  10, 10, 10,  10, 8, 13,  1,  1,  1,  1,  1,  1, 99,  1,  1,  1  },
                { 10, 10, 10, 10, 10, 8,  13, 7, 10, 10, 10,  10, 8,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1  },

            }, 64, "Tiles/Forest");
            // Get generated enemies from map
            enemies_ = map_.GetEnemies();
            camera_ = new Camera(map_.Width, map_.Height);

            dialog_box_ = new DialogBox(game_, Font)
            {
                Text = "Hello World! Press Enter or Button A to proceed.\n" +
                       "I will be on the next pane! " +
                       "And wordwrap will occur, especially if there are some longer words!\n" +
                       "Monospace fonts work best but you might not want Courier New.\n" +
                       "In this code sample, after this dialog box finishes, you can press the O key to open a new one."
            };
            dialog_box_.Initialize();
        }

        public void Save(PlayerStats stats)
        {
            // This can be list so that we can have more Players/Enemies
            string serializedText = JsonSerializer.Serialize<PlayerStats>(stats);

            // This doesn't append (need to use "append" something)
            File.WriteAllText(stats_path_, serializedText);
        }

        // Called in create
        public void Load()
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

        internal void Update(GameTime gameTime)
        {
            dialog_box_.Update();

            // Debug dialog box
            if (Keyboard.GetState().IsKeyDown(Keys.O))
            {
                if (!dialog_box_.Active)
                {
                    dialog_box_ = new DialogBox(game_, Font) { Text = "New dialog box!" };
                    dialog_box_.Initialize();
                }
            }

            // Sprites
            foreach (var sprite in player_sprite_list_)
                sprite.Update(gameTime);

            foreach (Skeleton enemy in enemies_)
                enemy.Update(gameTime);

            // Camera
            camera_.Update(player_knight_);

            // Physics
            foreach (CollisionTile tile in map_.CollisionTiles)
                if (tile.Id >= 10) // Temp
                    player_knight_.Collision(tile, map_.Width, map_.Height);
        }

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
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
            dialog_box_.Draw(spriteBatch);
        }
    }
}
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
using Project1.Components;

namespace Project1
{
    public class Level2 : Level
    {
        private Game1 game_;

        private Knight player_knight_;
        private Wolf wolf_dog_;

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

        private bool first_dialog_box_opened_ = false;

        Texture2D light_mask_;

        RenderTarget2D lights_target_;
        RenderTarget2D main_target_;

        Effect lighting_effect_;
        private bool dog_found_ = false;

        private List<Vector2> light_sources_ = new List<Vector2>()
        {
            new Vector2(302, 150), new Vector2(187, 150), new Vector2(50, 150), new Vector2(1591, 150), new Vector2(1713, 150)
        };

        public Level2(Game1 game, ContentManager content)
        {
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
                Position = new Vector2(2100, 160)
            };
            player_sprite_list_ = new List<Sprite>()
            {
                wolf_dog_,
                player_knight_
            };
            wolf_dog_.Stay = true;

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
                { 9, 9, 9,99,99,99, 9, 9, 9, 9, 4, 1, 9, 9,11,13,10,13 },
                { 2, 2, 2, 1, 1, 1, 9, 9, 9, 9, 4, 1, 2, 2,99,99, 1, 7 },
                {13,10,13, 1, 1, 5,12,12,12, 6, 1, 1, 1, 8, 9, 9, 9, 9 },
                { 1, 1, 1, 1, 1, 3,14,14,14, 4, 1,13,10,13, 9, 9, 9, 9 },
                { 9, 9, 4, 7, 7, 3,14,14,14, 4, 1,99, 7, 1,13,10,13,15 },
                {13,13, 4, 1, 1, 3,14,14,14, 4,99, 1, 1, 1, 1,99, 1, 1 },
                { 8, 1, 1,99,99, 1, 1, 1, 1, 1, 1, 1, 1, 1,99,99, 1, 1 },
            }, 128, "Tiles/Dungeon", 200);
            // Get generated enemies from map
            enemies_ = map_.GetEnemies();
            camera_ = new Camera(map_.Width, map_.Height);

            dialog_box_ = new DialogBox(game_, Font)
            {
                Text = "This is a dungeon, you need to be quick and find your dog.\n" +
                        "Hope he isn't a goner, search around!\n" +
                        "The dungeon is swarmed with enemies, so you gotta be careful!"
            };
            dialog_box_.Initialize();

            // Load the lighting effect
            light_mask_ = content.Load<Texture2D>("lightmask-2");
            lighting_effect_ = content.Load<Effect>("Effect1");

            lights_target_ = new RenderTarget2D(
                game.GraphicsDevice, Game1.screen_width, Game1.screen_height);
            main_target_ = new RenderTarget2D(
                game.GraphicsDevice, Game1.screen_width, Game1.screen_height);
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

        public override void Update(GameTime gameTime)
        {
            Console.WriteLine(player_knight_.Position);

            dialog_box_.Update();

            // Sprites
            foreach (var sprite in player_sprite_list_)
                sprite.Update(gameTime);

            foreach (Skeleton enemy in enemies_)
                enemy.Update(gameTime);

            // Camera
            camera_.Update(player_knight_);

            // Physics
            foreach (CollisionTile tile in map_.CollisionTiles) { 
                if (tile.Id > 6 && tile.Id != 15) // Temp
                    player_knight_.Collision(tile, map_.Width, map_.Height);

                if (tile.Id == 15 && player_knight_.IsTouching(tile.Rectangle))
                { 
                    if (dog_found_)
                        game_.NextLevelState();
                    else
                    {
                        if (!dialog_box_.Active)
                        {
                            dialog_box_ = new DialogBox(game_, Font) { Text = "You can't leave without your dog:(." };
                            dialog_box_.Initialize();
                        }
                    }
                }
                //foreach (var enemy in enemies_)
                //    enemy.Collision(tile, map_.Width, map_.Height);
            }

            if (player_knight_.Rectangle.Intersects(wolf_dog_.Rectangle))
            {
                wolf_dog_.Stay = false;
                if (!dialog_box_.Active && !first_dialog_box_opened_)
                {
                    first_dialog_box_opened_ = true;
                    dialog_box_ = new DialogBox(game_, Font) { Text = "You got your dog! Now get outta here!" };
                    dialog_box_.Initialize();
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Create a Light Mask to pass to the pixel shader
            game_.GraphicsDevice.SetRenderTarget(lights_target_);
            game_.GraphicsDevice.Clear(Color.Black);
            sprite_batch_.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, camera_.ViewMatrix);
            sprite_batch_.Draw(light_mask_, new Vector2(player_knight_.X - 200, player_knight_.Y - 200), Color.White);
            sprite_batch_.End();

            // Draw the main scene to the Render Target
            game_.GraphicsDevice.SetRenderTarget(main_target_);
            game_.GraphicsDevice.Clear(Color.CornflowerBlue);
            sprite_batch_.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera_.ViewMatrix);

            map_.Draw(sprite_batch_);

            foreach (var sprite in player_sprite_list_)
                sprite.Draw(gameTime, sprite_batch_);

            foreach (var enemy in enemies_)
                enemy.Draw(gameTime, sprite_batch_);

            sprite_batch_.End();

            // Draw the main scene with a pixel
            game_.GraphicsDevice.SetRenderTarget(null);
            game_.GraphicsDevice.Clear(Color.CornflowerBlue);
            sprite_batch_.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            lighting_effect_.Parameters["lightMask"].SetValue(lights_target_);
            lighting_effect_.CurrentTechnique.Passes[0].Apply();
            sprite_batch_.Draw(main_target_, Vector2.Zero, Color.White);
            sprite_batch_.End();

            // Scoreboard and dialog box
            spriteBatch.DrawString(Font, destroyed_.ToString(), new Vector2(10, 10), Color.White);
            dialog_box_.Draw(spriteBatch);
        }
    }
}
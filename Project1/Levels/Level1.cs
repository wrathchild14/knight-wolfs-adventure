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
using Project1.Levels;

namespace Project1
{
    public class Level1 : Level
    {
        public Level1(Game1 game, ContentManager content) : base(game, content)
        {
            light_mask = content.Load<Texture2D>("lightmask-1");

            map_.Generate(new int[,]
            {
                { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 8,  7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 10, 10, 10, 10, 10, 10, 10, 9,  9,  10, 10, 8,  7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 10, 10, 10, 10, 10, 10, 10, 8,  7,  10, 10, 8,  7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 9,  9,  9,  9,  9,  9,  9,  1,  1,  9,  9,  4,  2,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9,  9 },
                { 1,  1,  1, 13,  1,  3,  1,  4,  1,  1,  1, 13,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1 },
                { 2,  3,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 12,  1,  1,  1,  1,  1, 15 },
                { 1,  1,  5,  1,  3,  1,  4,  1,  1,  1,  4,  1,  1,  1,  1,  1,  1,  1,  1,  4,  1,  1,  1,  1 },
                { 1,  1,  1,  3,  1,  1,  1,  1,  1,  1,  1,  3,  1,  3, 13,  1,  1,  4,  1,  1,  5,  1,  1,  1 },
                { 1,  12, 1,  4,  6,  1,  2,  1,  5,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  2,  1,  6,  1,  1 },
                { 1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1, 14,  1,  1, 14,  1,  1,  1,  1,  1 },
                { 1,  1,  1,  1,  1,  1,  1,  1, 11, 11, 11,  4,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1 },
                { 11, 11, 11, 11, 11, 1,  1,  7, 10, 10, 10,  8,  1,  1,  1,  1,  1,  3,  1,  1, 99,  4,  1,  1 },
                { 10, 10, 10, 10, 10, 8,  14, 7, 10, 10, 10,  11, 1,  1, 13,  1,  1,  1,  4,  6,  1,  1, 99,  1 },
                { 10, 10, 10, 10, 10, 8,  4, 7,  10, 10, 10,  10, 8, 13,  1,  1,  1,  4,  1,  3, 99,  1,  2, 99 },
                { 10, 10, 10, 10, 10, 8,  13, 7, 10, 10, 10,  10, 8,  1,  5,  1, 99,  1, 99,  1,  4,  1, 99,  1 },
            }, 128, "Tiles/Forest", 400);
            // Get generated enemies from map
            enemies_ = map_.GetEnemies();
            camera_ = new Camera(map_.Width, map_.Height);

            dialog_box_ = new DialogBox(game_, Font)
            {
                Text = "Hello Traveller! I will be your guide for this adventure! Press Enter to proceed. You can skip the whole dialog with X\n" +
                       "You walk with WASD, and sprint with SHIFT! Attack with holding J, or don't hold it \n" +
                       "For now, explore a little"
            };
            dialog_box_.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            int enemies_dead = 0;
            foreach (Skeleton enemy in enemies_)
            {
                if (enemy.Dead)
                    enemies_dead++;

                enemy.Update(gameTime);

                float distance = Vector2.Distance(player_knight_.Position, enemy.Position);
                if (distance < 550 && !dialog_box_.Active && !dialog_box_opened_)
                {
                    dialog_box_opened_ = true;
                    dialog_box_ = new DialogBox(game_, Font) { Text = "You can see some enemies ahead, attack them!" };
                    dialog_box_.Initialize();
                }
            }
            destroyed_ = enemies_dead;

            if (destroyed_ == enemies_.Count)
            {
                wolf_dog_.GoTo(new Vector2(3100, 700));

                if (!dialog_box_.Active && !second_dialog_box_opened_)
                {
                    second_dialog_box_opened_ = true;
                    dialog_box_ = new DialogBox(game_, Font) { Text = "Your dog got scared and ran away, follow him!" };
                    dialog_box_.Initialize();
                }
            }

           
            // Physics
            foreach (CollisionTile tile in map_.CollisionTiles)
            {
                if (tile.Id >= 10 && tile.Id != 15) // Temp
                    player_knight_.Collision(tile, map_.Width, map_.Height);
                if (tile.Id == 15 && player_knight_.IsTouching(tile.Rectangle) && enemies_dead == enemies_.Count)
                    game_.NextLevelState();

                foreach(var enemy in enemies_)
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
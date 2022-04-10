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
using Microsoft.Xna.Framework.Input;

namespace Project1
{
    public class Level2 : Level
    {
        private bool dog_found_ = false;

        public Level2(Game1 game, ContentManager content) : base(game, content)
        {
            light_mask = content.Load<Texture2D>("lightmask-2");

            player_knight_.Position = new Vector2(50, 400);
            wolf_dog_.Stay = true;
            wolf_dog_.Position = new Vector2(2100, 160);

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
            base.Update(gameTime);

            int enemies_dead = 0;
            foreach (Skeleton enemy in enemies_)
            {
                enemy.Update(gameTime);
                if (enemy.Dead)
                    enemies_dead++;
            }
            destroyed_ = enemies_dead;

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
                
                foreach (var enemy in enemies_)
                    if (tile.Id > 6 && tile.Id != 15) // Temp
                        enemy.Collision(tile, map_.Width, map_.Height);
            }

            if (player_knight_.Rectangle.Intersects(wolf_dog_.Rectangle))
            {
                wolf_dog_.Stay = false;
                dog_found_ = true;
                if (!dialog_box_.Active && !dialog_box_opened_)
                {
                    dialog_box_opened_ = true;
                    dialog_box_ = new DialogBox(game_, Font) { Text = "You got your dog! Now get outta here!" };
                    dialog_box_.Initialize();
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }
    }
}
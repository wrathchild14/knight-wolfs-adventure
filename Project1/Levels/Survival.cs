using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project1.Components;
using Project1.Models;
using Project1.Sprites;

namespace Project1.Levels
{
    internal class Survival : Level
    {
        private int maxEnemies_ = 20;
        private readonly Random random = new Random();

        public Survival(Game1 game, ContentManager content) : base(game, content)
        {
            lightMask_ = content.Load<Texture2D>("lightmask-1");

            playerKnight_.Position = new Vector2(1540, 970);
            wolfDog_.Position = playerKnight_.Position;

            map_.Generate(new[,]
            {
                { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 8, 7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 10, 10, 10, 10, 10, 10, 10, 9, 9, 10, 10, 8, 7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 10, 10, 10, 10, 10, 10, 10, 8, 7, 10, 10, 8, 7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 9, 9, 9, 9, 9, 9, 9, 1, 1, 9, 9, 4, 2, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                { 1, 1, 1, 13, 1, 3, 1, 4, 1, 1, 1, 13, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 2, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 12, 1, 1, 1, 1, 1, 4 },
                { 1, 1, 5, 1, 3, 1, 4, 1, 1, 1, 4, 1, 1, 1, 1, 1, 1, 1, 1, 4, 1, 1, 1, 1 },
                { 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 3, 1, 3, 13, 1, 1, 4, 1, 1, 5, 1, 1, 1 },
                { 1, 12, 1, 4, 6, 1, 2, 1, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 6, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 14, 1, 1, 14, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 11, 11, 11, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 11, 11, 11, 11, 11, 1, 1, 7, 10, 10, 10, 8, 1, 1, 1, 1, 1, 3, 1, 1, 99, 4, 1, 1 },
                { 10, 10, 10, 10, 10, 8, 11, 7, 10, 10, 10, 11, 1, 1, 13, 1, 1, 1, 4, 6, 1, 1, 99, 1 },
                { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 8, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11, 11 },
                { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }
            }, 128, "Tiles/Forest", 1200);
            // Get generated enemies from map
            enemies_ = map_.GetEnemies();
            camera = new Camera(map_.Width, map_.Height);

            dialogBox = new DialogBox(game_, font)
            {
                Text = "This is survival mode, fight as many enemies as you can, they won't stop coming"
            };
            dialogBox.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (destroyed_ > 10)
                maxEnemies_ = 30;
            else if (destroyed_ > 30)
                maxEnemies_ = 50;
            else if (destroyed_ > 50)
                maxEnemies_ = 100;

            if (enemies_.Count < maxEnemies_)
                enemies_.Add(new Skeleton(game_.Content.Load<Texture2D>("DebugRectangle"),
                    game_.Content.Load<Texture2D>("Sprites/Healthbar"),
                    playerKnight_,
                    1200,
                    new Dictionary<string, Animation>
                    {
                        {
                            "Attack", new Animation(game_.Content.Load<Texture2D>("Sprites/Skeleton/SkeletonAttack"), 8)
                        },
                        { "Dead", new Animation(game_.Content.Load<Texture2D>("Sprites/Skeleton/SkeletonDeath"), 4) },
                        { "Idle", new Animation(game_.Content.Load<Texture2D>("Sprites/Skeleton/SkeletonIdle"), 4) },
                        {
                            "Running",
                            new Animation(game_.Content.Load<Texture2D>("Sprites/Skeleton/SkeletonRunning"), 4)
                        },
                        {
                            "Attacked",
                            new Animation(game_.Content.Load<Texture2D>("Sprites/Skeleton/SkeletonAttacked"), 4)
                        }
                    })
                {
                    Position = new Vector2(random.Next(120, 3000), random.Next(450, 1500))
                });


            for (var i = 0; i < enemies_.Count; i++)
            {
                enemies_[i].Update(gameTime);
                if (enemies_[i].Dead)
                {
                    destroyed_++;
                    enemies_.RemoveAt(i);
                }
            }

            // Physics
            foreach (var tile in map_.CollisionTiles)
            {
                if (tile.Id >= 10 && tile.Id != 15) // Temp
                    playerKnight_.Collision(tile, map_.Width, map_.Height);

                foreach (var enemy in enemies_)
                    if (tile.Id >= 10 && tile.Id != 15) // Temp
                        enemy.Collision(tile, map_.Width, map_.Height);
            }
        }

        public void Save(PlayerStats stats)
        {
            // This can be list so that we can have more Players/Enemies
            var serializedText = JsonSerializer.Serialize(stats);

            // This doesn't append (need to use "append" something)
            File.WriteAllText(statsPath, serializedText);
        }

        // Called in create
        public override void Load()
        {
            var fileContent = File.ReadAllText(statsPath);
            playerStats = JsonSerializer.Deserialize<PlayerStats>(fileContent);
            destroyed_ = playerStats.Score;
        }
    }
}
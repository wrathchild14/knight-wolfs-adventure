using System.IO;
using System.Linq;
using System.Text.Json;
using Components;
using KWA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Levels
{
    public class Level2 : Level
    {
        private bool dog_found_;

        public Level2(KWAGame game, ContentManager content) : base(game, content)
        {
            lightMask_ = content.Load<Texture2D>("lightmask-2");

            playerKnight_.Position = new Vector2(50, 400);
            wolfDog_.Stay = true;
            wolfDog_.Position = new Vector2(2100, 160);

            map_.Generate(new[,]
            {
                { 9, 9, 9, 99, 99, 99, 9, 9, 9, 9, 4, 1, 9, 9, 11, 13, 10, 13 },
                { 2, 2, 2, 1, 1, 1, 9, 9, 9, 9, 4, 1, 2, 2, 99, 99, 1, 7 },
                { 13, 10, 13, 1, 1, 5, 12, 12, 12, 6, 1, 1, 1, 8, 9, 9, 9, 9 },
                { 1, 1, 1, 1, 1, 3, 14, 14, 14, 4, 1, 13, 10, 13, 9, 9, 9, 9 },
                { 9, 9, 4, 7, 7, 3, 14, 14, 14, 4, 1, 99, 7, 1, 13, 10, 13, 15 },
                { 13, 13, 4, 1, 1, 3, 14, 14, 14, 4, 99, 1, 1, 1, 1, 99, 1, 1 },
                { 8, 1, 1, 99, 99, 1, 1, 1, 1, 1, 1, 1, 1, 1, 99, 99, 1, 1 }
            }, 128, "Tiles/Dungeon", 200);
            // Get generated enemies from map
            enemies_ = map_.GetEnemies();
            camera = new Camera(map_.Width, map_.Height);

            dialogBox = new DialogBox(game_, font)
            {
                Text = "This is a dungeon, you need to be quick and find your dog.\n" +
                       "Hope he isn't a goner, search around!\n" +
                       "The dungeon is swarmed with enemies, so you gotta be careful!"
            };
            dialogBox.Initialize();
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var enemiesDead = 0;
            foreach (var enemy in enemies_)
            {
                enemy.Update(gameTime);
                if (enemy.Dead)
                    enemiesDead++;
            }

            destroyed_ = enemiesDead;

            // Physics
            foreach (var tile in map_.CollisionTiles)
            {
                if (tile.Id > 6 && tile.Id != 15) // Temp
                    playerKnight_.Collision(tile, map_.Width, map_.Height);

                if (tile.Id == 15 && playerKnight_.IsTouching(tile.Rectangle))
                {
                    if (dog_found_)
                    {
                        game_.NextLevelState();
                    }
                    else
                    {
                        if (!dialogBox.Active)
                        {
                            dialogBox = new DialogBox(game_, font) { Text = "You can't leave without your dog:(." };
                            dialogBox.Initialize();
                        }
                    }
                }

                foreach (var enemy in enemies_.Where(enemy => tile.Id > 6 && tile.Id != 15))
                    enemy.Collision(tile, map_.Width, map_.Height);
            }

            if (playerKnight_.Rectangle.Intersects(wolfDog_.Rectangle))
            {
                wolfDog_.Stay = false;
                dog_found_ = true;
                if (!dialogBox.Active && !dialogBoxOpened_)
                {
                    dialogBoxOpened_ = true;
                    dialogBox = new DialogBox(game_, font) { Text = "You got your dog! Now get outta here!" };
                    dialogBox.Initialize();
                }
            }
        }
    }
}
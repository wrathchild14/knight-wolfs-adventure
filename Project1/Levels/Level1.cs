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
    public class Level1 : Level
    {
        public Level1(KWAGame game, ContentManager content) : base(game, content)
        {
            lightMask_ = content.Load<Texture2D>("lightmask-1");

            map_.Generate(new[,]
            {
                { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 8, 7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 10, 10, 10, 10, 10, 10, 10, 9, 9, 10, 10, 8, 7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 10, 10, 10, 10, 10, 10, 10, 8, 7, 10, 10, 8, 7, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 },
                { 9, 9, 9, 9, 9, 9, 9, 1, 1, 9, 9, 4, 2, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                { 1, 1, 1, 13, 1, 3, 1, 4, 1, 1, 1, 13, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 2, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 12, 1, 1, 1, 1, 1, 15 },
                { 1, 1, 5, 1, 3, 1, 4, 1, 1, 1, 4, 1, 1, 1, 1, 1, 1, 1, 1, 4, 1, 1, 1, 1 },
                { 1, 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 3, 1, 3, 13, 1, 1, 4, 1, 1, 5, 1, 1, 1 },
                { 1, 12, 1, 4, 6, 1, 2, 1, 5, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 6, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 14, 1, 1, 14, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 11, 11, 11, 4, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 11, 11, 11, 11, 11, 1, 1, 7, 10, 10, 10, 8, 1, 1, 1, 1, 1, 3, 1, 1, 99, 4, 1, 1 },
                { 10, 10, 10, 10, 10, 8, 14, 7, 10, 10, 10, 11, 1, 1, 13, 1, 1, 1, 4, 6, 1, 1, 99, 1 },
                { 10, 10, 10, 10, 10, 8, 4, 7, 10, 10, 10, 10, 8, 13, 1, 1, 1, 4, 1, 3, 99, 1, 2, 99 },
                { 10, 10, 10, 10, 10, 8, 13, 7, 10, 10, 10, 10, 8, 1, 5, 1, 99, 1, 99, 1, 4, 1, 99, 1 }
            }, 128, "Tiles/Forest", 400);
            // Get generated enemies from map
            enemies_ = map_.GetEnemies();
            camera = new Camera(map_.Width, map_.Height);

            dialogBox = new DialogBox(game_, font)
            {
                Text =
                    "Hello Traveler! I will be your guide for this adventure! Press Enter to proceed. You can skip the whole dialog with X\n" +
                    "You walk with WASD, and sprint with SHIFT! Attack with holding J, or don't hold it \n" +
                    "For now, explore a little"
            };
            dialogBox.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var enemiesDead = 0;
            foreach (var enemy in enemies_)
            {
                if (enemy.Dead)
                    enemiesDead++;

                enemy.Update(gameTime);

                var distance = Vector2.Distance(playerKnight_.Position, enemy.Position);
                if (distance < 550 && !dialogBox.Active && !dialogBoxOpened_)
                {
                    dialogBoxOpened_ = true;
                    dialogBox = new DialogBox(game_, font) { Text = "You can see some enemies ahead, attack them!" };
                    dialogBox.Initialize();
                }
            }

            destroyed_ = enemiesDead;

            if (destroyed_ == enemies_.Count)
            {
                wolfDog_.GoTo(new Vector2(3100, 700));

                if (!dialogBox.Active && !secondDialogBoxOpened_)
                {
                    secondDialogBoxOpened_ = true;
                    dialogBox = new DialogBox(game_, font) { Text = "Your dog got scared and ran away, follow him!" };
                    dialogBox.Initialize();
                }
            }


            // Physics
            foreach (var tile in map_.CollisionTiles)
            {
                if (tile.Id >= 10 && tile.Id != 15) // Temp
                    playerKnight_.Collision(tile, map_.Width, map_.Height);
                if (tile.Id == 15 && playerKnight_.IsTouching(tile.Rectangle) && enemiesDead == enemies_.Count)
                    game_.NextLevelState();

                foreach (var enemy in enemies_.Where(enemy => tile.Id >= 10 && tile.Id != 15))
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
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Project1.Components;
using Project1.Levels;

namespace Project1
{
    internal class Level3 : Level
    {
        public Level3(KWAGame game, ContentManager content) : base(game, content)
        {
            lightMask_ = content.Load<Texture2D>("lightmask-3");

            map_.Generate(new[,]
            {
                { 14, 14, 14, 12, 11, 13, 14, 14, 14, 14, 14, 14, 12, 99, 2, 99 },
                { 14, 14, 14, 12, 19, 99, 11, 11, 11, 11, 11, 11, 1, 19, 99, 3 },
                { 11, 11, 11, 2, 2, 1, 1, 10, 10, 10, 10, 10, 10, 6, 1, 5 },
                { 16, 16, 16, 15, 18, 26, 1, 6, 19, 23, 24, 23, 24, 8, 23, 24 },
                { 5, 4, 2, 1, 2, 2, 4, 8, 5, 21, 22, 21, 22, 99, 21, 22 },
                { 3, 2, 4, 26, 5, 99, 2, 8, 26, 20, 25, 20, 25, 99, 20, 25 },
                { 26, 2, 4, 1, 1, 99, 4, 9, 10, 10, 10, 10, 10, 6, 3, 1 },
                { 1, 3, 1, 2, 3, 2, 26, 5, 3, 23, 24, 23, 24, 8, 26, 99 },
                { 1, 5, 19, 3, 1, 4, 1, 1, 99, 21, 22, 21, 22, 8, 5, 99 },
                { 16, 16, 16, 16, 15, 18, 17, 19, 4, 20, 25, 20, 25, 8, 4, 19 }
            }, 128, "Tiles/Town", 400);
            // Get generated enemies from map
            enemies_ = map_.GetEnemies();
            camera = new Camera(map_.Width, map_.Height);

            dialogBox = new DialogBox(game_, font)
            {
                Text = "You made it! Congratulations, but it seems like some enemies came up with you as well\n" +
                       "You will need to save and clear your hometown from the enemies"
            };
            dialogBox.Initialize();
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
            if (destroyed_ == enemies_.Count)
                if (!dialogBox.Active && !dialogBoxOpened_)
                {
                    dialogBoxOpened_ = true;

                    game_.Instance?.Stop(); // shut down the music
                    game_.Content.Load<SoundEffect>("Sounds/Win");

                    dialogBox = new DialogBox(game_, font)
                    {
                        Text = "Congratulations, you saved your village \n" +
                               "Esc to end the game and get to main menu (you can explore a little bit)"
                    };
                    dialogBox.Initialize();
                }

            // Physics
            foreach (var tile in map_.CollisionTiles)
            {
                if (tile.Id >= 14) // Temp
                    playerKnight_.Collision(tile, map_.Width, map_.Height);
                //if (tile.Id == 15 && player_knight_.IsTouching(tile.Rectangle) && enemies_dead == enemies_.Count)
                //game_.NextLevelState();

                foreach (var enemy in enemies_.Where(enemy => tile.Id >= 14))
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
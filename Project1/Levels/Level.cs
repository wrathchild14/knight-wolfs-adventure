using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1.Components;
using Project1.Models;
using Project1.Sprites;
using Project1.TileMap;

namespace Project1.Levels
{
    public abstract class Level
    {
        protected Camera camera;

        protected int destroyed_ = 0;
        protected DialogBox dialogBox;
        protected bool dialogBoxOpened_ = false;
        protected SoundEffect endSound;
        protected List<Skeleton> enemies_ = new List<Skeleton>();
        protected SpriteFont font;
        protected Game1 game_;

        protected Effect lightingEffect_;

        protected Texture2D lightMask_;

        protected RenderTarget2D lightsTarget_;
        protected RenderTarget2D mainTarget_;

        protected Map map_;

        protected Knight playerKnight_;
        protected List<Sprite> playerSpriteList_;
        protected PlayerStats playerStats;

        protected SoundEffect punchSound;
        protected bool secondDialogBoxOpened_ = false;
        protected SpriteBatch spriteBatch_;
        protected string statsPath = "Stats.json";
        protected Wolf wolfDog_;

        public Level(Game1 game, ContentManager content)
        {
            // Can get rid of this private
            game_ = game;

            var debug = content.Load<Texture2D>("DebugRectangle");
            spriteBatch_ = new SpriteBatch(game_.GraphicsDevice);
            // Content loaders
            var healthbarTexture = content.Load<Texture2D>("Sprites/Healthbar");
            // Enemy (which is generated in Map.cs)
            var skeletonTexturesForAnimations = new Dictionary<string, Texture2D>
            {
                { "Attack", content.Load<Texture2D>("Sprites/Skeleton/SkeletonAttack") },
                { "Dead", content.Load<Texture2D>("Sprites/Skeleton/SkeletonDeath") },
                { "Idle", content.Load<Texture2D>("Sprites/Skeleton/SkeletonIdle") },
                { "Running", content.Load<Texture2D>("Sprites/Skeleton/SkeletonRunning") },
                { "Attacked", content.Load<Texture2D>("Sprites/Skeleton/SkeletonAttacked") }
            };
            // Player
            playerKnight_ = new Knight(debug, healthbarTexture, new Dictionary<string, Animation>
                {
                    { "Dead", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightDeath"), 4) },
                    { "Attack", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightAttack"), 15) },
                    { "Pray", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightPray"), 12) },
                    { "Idle", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightIdle"), 8) },
                    { "Running", new Animation(content.Load<Texture2D>("Sprites/Knight/KnightRunning"), 8) }
                })
                { Position = new Vector2(50, 600) };
            // Dog
            wolfDog_ = new Wolf(new Dictionary<string, Animation>
            {
                { "Idle", new Animation(content.Load<Texture2D>("Sprites/Wolf/WolfIdle"), 4) },
                { "Running", new Animation(content.Load<Texture2D>("Sprites/Wolf/WolfRunning"), 4) }
            }, playerKnight_)
            {
                Position = new Vector2(playerKnight_.Position.X - 40, playerKnight_.Position.Y + 15)
            };
            playerSpriteList_ = new List<Sprite>
            {
                wolfDog_, playerKnight_
            };

            playerStats = new PlayerStats
            {
                Name = "Jovan",
                Score = 0
            };

            punchSound = content.Load<SoundEffect>("Sounds/punch");
            endSound = content.Load<SoundEffect>("Sounds/end");
            font = content.Load<SpriteFont>("defaultFont");

            Tile.Content = content;

            map_ = new Map(debug, skeletonTexturesForAnimations, healthbarTexture, playerKnight_);

            // Load the lighting effect
            //light_mask = content.Load<Texture2D>("lightmask-1");
            lightingEffect_ = content.Load<Effect>("Effect1");

            lightsTarget_ = new RenderTarget2D(
                game_.GraphicsDevice, Game1.ScreenWidth, Game1.ScreenHeight);
            mainTarget_ = new RenderTarget2D(
                game_.GraphicsDevice, Game1.ScreenWidth, Game1.ScreenHeight);
        }

        public virtual void Update(GameTime gameTime)
        {
            dialogBox.Update();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                game_.ChangeStateMenu();

            if (playerKnight_.Dead)
                game_.ChangeStateEnd();

            // Sprites
            foreach (var sprite in playerSpriteList_)
                sprite.Update(gameTime);

            // Camera
            camera.Update(playerKnight_);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Create a Light Mask to pass to the pixel shader
            game_.GraphicsDevice.SetRenderTarget(lightsTarget_);
            game_.GraphicsDevice.Clear(Color.Black);
            spriteBatch_.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null,
                camera.ViewMatrix);
            var offset = lightMask_.Width / 2;
            spriteBatch_.Draw(lightMask_, new Vector2(playerKnight_.X - offset, playerKnight_.Y - offset), Color.White);
            spriteBatch_.End();

            // Draw the main scene to the Render Target
            game_.GraphicsDevice.SetRenderTarget(mainTarget_);
            game_.GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch_.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null,
                camera.ViewMatrix);

            map_.Draw(spriteBatch_);

            foreach (var sprite in playerSpriteList_)
                sprite.Draw(gameTime, spriteBatch_);

            foreach (var enemy in enemies_)
                enemy.Draw(gameTime, spriteBatch_);

            spriteBatch_.End();

            // Draw the main scene with a pixel
            game_.GraphicsDevice.SetRenderTarget(null);
            game_.GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch_.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            lightingEffect_.Parameters["lightMask"].SetValue(lightsTarget_);
            lightingEffect_.CurrentTechnique.Passes[0].Apply();
            spriteBatch_.Draw(mainTarget_, Vector2.Zero, Color.White);
            spriteBatch_.End();

            // Scoreboard and dialog box
            spriteBatch.DrawString(font, destroyed_.ToString(), new Vector2(10, 10), Color.White);
            dialogBox.Draw(spriteBatch);
        }

        public abstract void Load();
    }
}
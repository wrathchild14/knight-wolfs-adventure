using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1.Components;
using Project1.Models;
using Project1.Sprites;
using Project1.TileMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1.Levels
{
    public abstract class Level
    {
        protected Game1 game_;

        protected Knight player_knight_;
        protected Wolf wolf_dog_;

        protected int destroyed_ = 0;
        protected PlayerStats player_stats_;
        protected string stats_path_ = "Stats.json";

        protected SoundEffect punch_sound_;
        protected SoundEffect end_sound_;
        protected SpriteFont Font;

        protected Camera camera_;
        protected DialogBox dialog_box_;
        protected SpriteBatch sprite_batch_;
        protected List<Sprite> player_sprite_list_;
        protected List<Skeleton> enemies_ = new List<Skeleton>();

        protected Map map_;
        protected bool dialog_box_opened_ = false;
        protected bool second_dialog_box_opened_ = false;

        protected Texture2D light_mask;

        protected RenderTarget2D lights_target_;
        protected RenderTarget2D main_target_;

        protected Effect lighting_effect_;

        public Level(Game1 game, ContentManager content)
        {
            // Can get rid of this private
            game_ = game;

            Texture2D debug = content.Load<Texture2D>("DebugRectangle");
            sprite_batch_ = new SpriteBatch(game_.GraphicsDevice);
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
            { Position = new Vector2(50, 600) };
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

            // Load the lighting effect
            //light_mask = content.Load<Texture2D>("lightmask-1");
            lighting_effect_ = content.Load<Effect>("Effect1");

            lights_target_ = new RenderTarget2D(
                game_.GraphicsDevice, Game1.screen_width, Game1.screen_height);
            main_target_ = new RenderTarget2D(
                game_.GraphicsDevice, Game1.screen_width, Game1.screen_height);

        }

        public virtual void Update(GameTime gameTime)
        {
            dialog_box_.Update();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                game_.ChangeStateMenu();

            if (player_knight_.Dead)
                game_.ChangeStateEnd();

            // Sprites
            foreach (var sprite in player_sprite_list_)
                sprite.Update(gameTime);

            // Camera
            camera_.Update(player_knight_);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Create a Light Mask to pass to the pixel shader
            game_.GraphicsDevice.SetRenderTarget(lights_target_);
            game_.GraphicsDevice.Clear(Color.Black);
            sprite_batch_.Begin(SpriteSortMode.Immediate, BlendState.Additive, null, null, null, null, camera_.ViewMatrix);
            var offset = light_mask.Width / 2;
            sprite_batch_.Draw(light_mask, new Vector2(player_knight_.X - offset, player_knight_.Y - offset), Color.White);
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

        public abstract void Load();

    }
}

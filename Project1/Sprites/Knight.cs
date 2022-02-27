using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project1.Models;
using Project1.TileMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project1.Sprites
{
    public class Knight : Sprite
    {
        public bool IsAttacking;
        public Rectangle AttackRectangle;

        private Vector2 velocity_;

        private float speed_x_ = 3.6f;
        private float speed_y_ = 2.5f;
        private bool _pray;

        private Texture2D debug_attack_rectangle_;
        private bool debug_attack_rectangle_bool_ = false;

        public Knight(Texture2D texture2D, Dictionary<string, Animation> animations) : base(animations)
        {
            debug_attack_rectangle_ = texture2D;
            animations_["Attack"].FrameSpeed = 0.1f;
            animations_["Running"].FrameSpeed = 0.1f;
        }

        public override void Update(GameTime game_time)
        {
            TakeInput();
            SetAnimation();
            animation_manager_.Update(game_time);
            velocity_.X = 0;
        }
        internal void Collision(CollisionTile tile, int x_offset, int y_offset)
        {
            Rectangle tile_rectangle = tile.Rectangle;
            if (Rectangle.TouchTopOf(tile_rectangle))
            {
                Y = tile_rectangle.Y - Rectangle.Height;
            }

            if (Rectangle.TouchLeftOf(tile_rectangle))
            {
                X = tile_rectangle.X - Rectangle.Width - 2;
            }

            if (Rectangle.TouchRightOf(tile_rectangle))
            {
                X = tile_rectangle.X + tile_rectangle.Width + 4;
            }

            if (Rectangle.TouchBottonOf(tile_rectangle))
            {
                Console.WriteLine(Rectangle);
                Console.WriteLine(tile_rectangle);
                Y = tile_rectangle.Y + tile_rectangle.Height + 14;
                Console.WriteLine(Rectangle);
                Console.WriteLine(tile_rectangle);
            }

            if (X < 0) X = 0;
            if (X > x_offset - Rectangle.Width) X = x_offset - Rectangle.Width;
            if (Y < 0) Y = 0;
            if (Y > y_offset - Rectangle.Height) Y = y_offset - Rectangle.Height;
        }

        private void Attack()
        {
            IsAttacking = true;

            // Rectangle for attack zone
            if (animation_manager_.Right)
            {
                AttackRectangle = new Rectangle((int)(Position.X + Rectangle.Width), (int)(Position.Y + 10), 5, 5);
            }
            else if (!animation_manager_.Right)
            {
                AttackRectangle = new Rectangle((int)(Position.X), (int)(Position.Y + 10), 5, 5);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (IsAttacking && debug_attack_rectangle_bool_)
                spriteBatch.Draw(debug_attack_rectangle_, AttackRectangle, Color.Red);

            base.Draw(gameTime, spriteBatch);
        }

        private void SetAnimation()
        {
            if (IsAttacking)
            {
                animation_manager_.Play(animations_["Attack"]);
            }
            else if (velocity_.X < 0)
            {
                animation_manager_.Right = false;
                animation_manager_.Play(animations_["Running"]);
            }
            else if (velocity_.X > 0)
            {
                animation_manager_.Right = true;
                animation_manager_.Play(animations_["Running"]);
            }
            else if (_pray)
                animation_manager_.Play(animations_["Pray"]);
            else if (velocity_.X == 0)
                animation_manager_.Play(animations_["Idle"]);
        }

        private void TakeInput()
        {
            // Attack
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                Attack();
            else
                IsAttacking = false;

            // Movement
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Y -= speed_y_;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Y += speed_y_;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                X -= speed_x_;
                velocity_.X -= speed_x_;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                X += speed_x_;
                velocity_.X += speed_x_;
            }

            // Sprint
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                speed_x_ = 5f;
                animations_["Running"].FrameSpeed = 0.075f;
            }
            else
            {
                speed_x_ = 3.6f;
                animations_["Running"].FrameSpeed = 0.1f;
            }

            // Pray
            if (Keyboard.GetState().IsKeyDown(Keys.P))
                _pray = true;
            else
                _pray = false;
        }
    }
}
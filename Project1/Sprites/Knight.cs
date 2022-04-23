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
        public bool Attacking;
        public Rectangle AttackRectangle;

        private Vector2 velocity_;

        private float speedX_;
        private float speedY_;
        private float default_speedX_ = 3.2f;
        private float default_speedY_ = 2f;
        private float sprint_speedX_ = 3.6f;
        private float sprint_speedY_ = 2.4f;
        private bool pray_;

        private Texture2D debug_attack_rectangle_;
        private bool debug_attack_rectangle_bool_ = false;

        private Healthbar healthbar_;

        public Knight(Texture2D debug_rectangle, Texture2D health_bar_texture, Dictionary<string, Animation> animations) : base(animations)
        {
            healthbar_ = new Healthbar(health_bar_texture, this);
            debug_attack_rectangle_ = debug_rectangle;
            animations_["Attack"].FrameSpeed = 0.1f;
            animations_["Running"].FrameSpeed = 0.1f;
        }

        public override void Update(GameTime gameTime)
        {
            if (Dead)
                animationManager.UpdateTillEnd(gameTime);
            else
            {
                TakeInput();
                animationManager.Update(gameTime);
                healthbar_.Update(gameTime);
            }
            SetAnimation();
            velocity_.X = 0;
            velocity_.Y = 0;
        }

        private void Attack()
        {
            Attacking = true;

            // Rectangle for attack zone
            if (animationManager.Right)
            {
                AttackRectangle = new Rectangle((int)(Position.X + Rectangle.Width), (int)(Position.Y + 10), 5, 5);
            }
            else if (!animationManager.Right)
            {
                AttackRectangle = new Rectangle((int)(Position.X), (int)(Position.Y + 10), 5, 5);
            }
        }

        public void TakeDamage(int damage)
        {
            healthbar_.TakeDamage(damage);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Attacking && debug_attack_rectangle_bool_)
                spriteBatch.Draw(debug_attack_rectangle_, AttackRectangle, Color.Red);

            if (debug_attack_rectangle_bool_)
                spriteBatch.Draw(debug_attack_rectangle_, Rectangle, Color.Red);

            healthbar_?.Draw(gameTime, spriteBatch);

            base.Draw(gameTime, spriteBatch);
        }

        private void SetAnimation()
        {
            if (Dead)
                animationManager.Play(animations_["Dead"]);
            else
            {
                if (Attacking)
                    animationManager.Play(animations_["Attack"]);
                else if (velocity_.X < 0)
                {
                    animationManager.Right = false;
                    animationManager.Play(animations_["Running"]);
                }
                else if (velocity_.X > 0)
                {
                    animationManager.Right = true;
                    animationManager.Play(animations_["Running"]);
                }
                else if (velocity_.Y > 0)
                    animationManager.Play(animations_["Running"]);
                else if (pray_)
                    animationManager.Play(animations_["Pray"]);
                else if (velocity_.X == 0)
                    animationManager.Play(animations_["Idle"]);
            }
        }

        private void TakeInput()
        {
            // Attack
            if (Keyboard.GetState().IsKeyDown(Keys.J))
                Attack();
            else
            {
                Attacking = false;

                // Movement
                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    Y -= speedY_;
                    velocity_.Y += speedY_;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    Y += speedY_;
                    velocity_.Y += speedY_;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    X -= speedX_;
                    velocity_.X -= speedX_;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    X += speedX_;
                    velocity_.X += speedX_;
                }

                // Sprint
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    speedX_ = sprint_speedX_;
                    speedY_ = sprint_speedY_;
                    animations_["Running"].FrameSpeed = 0.075f;
                }
                else
                {
                    speedX_ = default_speedX_;
                    speedY_ = default_speedY_;
                    animations_["Running"].FrameSpeed = 0.1f;
                }

                // Pray
                if (Keyboard.GetState().IsKeyDown(Keys.P))
                    pray_ = true;
                else
                    pray_ = false;
            }
        }

        // meh, dont look at this
        public bool IsTouching(Rectangle rectangle)
        {
            return Rectangle.Right >= rectangle.Left &&
               Rectangle.Left <= rectangle.Right &&
               Rectangle.Bottom >= rectangle.Top &&
               Rectangle.Top <= rectangle.Bottom;
        }
    }
}
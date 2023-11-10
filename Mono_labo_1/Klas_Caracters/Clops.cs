using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mono_labo_1.interfaces;
using Mono_labo_1.Klas_Frames;
using Mono_labo_1.Klas_Interfaces;
using Mono_labo_1.Klas_Move;
using System;
using System.Collections.Generic;

namespace Mono_labo_1.Caracters
{
    class Clops : IGameObject, IMovable
    {
        // PROPERTIES
        private Texture2D texture;
        private Dictionary<string, Animation> animations;
        private MovementManager movementManager;
        private Animation currentAnimation;
        private bool isMovingLeft;
        private int frameWidth;
        private int frameHeight;
        private float gravity = 0.5f;
        private float verticalVelocity = 0f;
        private bool isJumping;
        private float jumpForce = 10f;

        public Vector2 Position { get; set; }
        public Vector2 Speed { get; set; }
        public IInputReader InputReader { get; set; }
        private GraphicsDevice graphicsDevice;

        // CONSTRUCTOR
        public Clops(Texture2D texture, IInputReader inputReader, GraphicsDevice graphicsDevice)
        {
            this.texture = texture;
            this.InputReader = inputReader;
            animations = new Dictionary<string, Animation>();
            movementManager = new MovementManager();
            Position = new Vector2(1, graphicsDevice.Viewport.Height - frameHeight); 
            Speed = new Vector2(3, 3);
            this.graphicsDevice = graphicsDevice;

            SetupAnimations();
            SetCurrentAnimation("idleleft");

            frameWidth = currentAnimation.CurrentFrame.SourceRectangle.Width;
            frameHeight = currentAnimation.CurrentFrame.SourceRectangle.Height;
        }

        private void SetCurrentAnimation(string animationName)
        {
            if (animations.ContainsKey(animationName))
            {
                currentAnimation = animations[animationName];
                frameWidth = currentAnimation.CurrentFrame.SourceRectangle.Width;
                frameHeight = currentAnimation.CurrentFrame.SourceRectangle.Height;
            }
            else
            {
                currentAnimation = animations.ContainsKey("idleleft") ? animations["idleleft"] : null;

                if (currentAnimation == null)
                {
                    throw new ArgumentException("Animation with the specified name does not exist: " + animationName);
                }
            }
        }

        private void SetupAnimations()
        {
            Animation idleLeftAnimation = new Animation();
            idleLeftAnimation.GetAnimationFromTextureRow(11, 15, 64, 64, 0);
            animations.Add("idleleft", idleLeftAnimation);

            Animation idleRightAnimation = new Animation();
            idleRightAnimation.GetAnimationFromTextureRow(1, 15, 64, 64, 0);
            animations.Add("idleright", idleRightAnimation);

            Animation walkLeftAnimation = new Animation();
            walkLeftAnimation.GetAnimationFromTextureRow(12, 12, 64, 64, 2);
            animations.Add("walkleft", walkLeftAnimation);

            Animation walkRightAnimation = new Animation();
            walkRightAnimation.GetAnimationFromTextureRow(2, 12, 64, 64, 2);
            animations.Add("walkright", walkRightAnimation);

            Animation jumpLeftAnimation = new Animation();
            jumpLeftAnimation.GetAnimationFromTextureRow(13, 7, 64, 64, 2);
            animations.Add("jumpleft", jumpLeftAnimation);

            Animation jumpRightAnimation = new Animation();
            jumpRightAnimation.GetAnimationFromTextureRow(3, 7, 64, 64, 2);
            animations.Add("jumpright", jumpRightAnimation);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float scale = 2f;
            Vector2 drawPosition = Position - (currentAnimation.CurrentFrame.SourceRectangle.Size.ToVector2() * scale - currentAnimation.CurrentFrame.SourceRectangle.Size.ToVector2()) / 2f;
            spriteBatch.Draw(texture, drawPosition, currentAnimation.CurrentFrame.SourceRectangle, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        public void Update(GameTime gameTime)
        {
            Move();
            ApplyGravity();
            if (InputReader.IsKeyPressed(Keys.Up) && !isJumping)
            {
                Jump();
            }
            CheckCollisionWithConsoleBounds();
            currentAnimation.Update(gameTime);
        }


        private void Jump()
        {
            // Start de sprong alleen als het personage op de grond staat
            if (Position.Y + frameHeight >= graphicsDevice.Viewport.Height)
            {
                // Pas de verticale snelheid toe om het personage omhoog te laten springen
                verticalVelocity = -jumpForce;
                isJumping = true;

                if (isMovingLeft)
                {
                    SetCurrentAnimation("jumpleft");
                }
                else
                {
                    SetCurrentAnimation("jumpright");
                }
            }
        }

        private void ApplyGravity()
        {
            // Voeg zwaartekracht toe aan de verticale snelheid
            verticalVelocity += gravity;

            // Pas de verticale positie aan op basis van de verticale snelheid
            Position += new Vector2(0, verticalVelocity);

            // Controleer of het personage de vloer raakt
            if (Position.Y + frameHeight >= graphicsDevice.Viewport.Height)
            {
                // Zet de verticale snelheid op nul en plaats het personage op de vloerpositie
                verticalVelocity = 0f;
                Position = new Vector2(Position.X, graphicsDevice.Viewport.Height - frameHeight);
                isJumping = false;
            }

        }

        private void Move()
        {
            Vector2 movement = InputReader.ReadInput();

            if (movement.X < 0)
            {
                isMovingLeft = true;
                SetCurrentAnimation("walkleft");
            }
            else if (movement.X > 0)
            {
                isMovingLeft = false;
                SetCurrentAnimation("walkright");
            }
            else if (isMovingLeft)
            {
                SetCurrentAnimation("idleleft");
            }
            else
            {
                SetCurrentAnimation("idleright");
            }

            movementManager.Move(this);
        }

        private void CheckCollisionWithConsoleBounds()
        {
            int consoleWidth = graphicsDevice.Viewport.Width;
            int consoleHeight = graphicsDevice.Viewport.Height;
            int adjustedFrameWidth = frameWidth;
            int adjustedFrameHeight = frameHeight;

            Vector2 currentPosition = Position;

            if (currentPosition.X < -10)
            {
                currentPosition.X = -10;
            }
            else if (currentPosition.X + adjustedFrameWidth > consoleWidth + 10)
            {
                currentPosition.X = consoleWidth - adjustedFrameWidth + 10;
            }

            if (currentPosition.Y < -25)
            {
                currentPosition.Y = -25;
            }
            else if (currentPosition.Y + adjustedFrameHeight > consoleHeight - 30)
            {
                currentPosition.Y = consoleHeight - adjustedFrameHeight - 30;
            }

            Position = currentPosition;
        }
    }
}

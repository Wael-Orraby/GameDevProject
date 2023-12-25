using System;
using GameDevProject.Game.AnimationsFolder;
using GameDevProject.Game.Klas_Interfaces;
using GameDevProject.Game.LevelDesign;
using GameDevProject.Game.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDevProject.Game.Character
{
    /// <summary>
    /// Richting waarin het gezicht zich bevindt langs de X-as.
    /// </summary>
    enum FaceDirection
    {
        Left = -1,
        Right = 1,
    }

    /// <summary>
    /// Een monster dat de voortgang van onze onverschrokken avonturier belemmert.
    /// </summary>
    public class Enemy : IEnemy
    {
        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Positie in de wereldruimte van het onderste midden van deze vijand.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }
        Vector2 position;

        private Rectangle localBounds;
        /// <summary>
        /// Geeft een rechthoek die deze vijand begrenst in de wereldruimte.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        // Animations
        private Animation runAnimation;
        private Animation idleAnimation;
        private AnimationPlayer sprite;

        /// <summary>
        /// De richting waarin deze vijand zich bevindt en langs de X-as beweegt.
        /// </summary>
        private FaceDirection direction = FaceDirection.Left;

        /// <summary>
        /// Hoe lang deze vijand heeft gewacht voordat hij zich omdraait.
        /// </summary>
        private float waitTime;

        /// <summary>
        /// Hoe lang te wachten voordat hij zich omdraait.
        /// </summary>
        private const float MaxWaitTime = 0.5f;

        /// <summary>
        /// De snelheid waarmee deze vijand zich langs de X-as verplaatst.
        /// </summary>
        private const float MoveSpeed = 64.0f;

        /// <summary>
        /// Construeert een nieuwe vijand.
        /// </summary>
        public Enemy(Level level, Vector2 position, string spriteSet)
        {
            this.level = level;
            this.position = position;

            LoadContent(spriteSet);
        }

        /// <summary>
        /// Laadt een specifiek vijandelijke sprite-sheet en geluiden.
        /// </summary>
        public void LoadContent(string spriteSet)
        {
            // Laad animaties.
            spriteSet = "Sprites/" + spriteSet + "/";
            runAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Run"), 0.1f, true);
            idleAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Idle"), 0.15f, true);
            sprite.PlayAnimation(idleAnimation);

            // Bereken grenzen binnen texture grootte.
            int width = (int)(idleAnimation.FrameWidth * 0.35);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameHeight * 0.7);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
        }


        /// <summary>
        /// Loopt heen en weer langs een platform, wachtend aan beide uiteinden.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Bereken tegel positie op basis van de kant waar we naartoe lopen.
            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / Tile.Width) - (int)direction;
            int tileY = (int)Math.Floor(Position.Y / Tile.Height);

            if (waitTime > 0)
            {
                // Wacht gedurende enige tijd.
                waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (waitTime <= 0.0f)
                {
                    // Draai dan om.
                    direction = (FaceDirection)(-(int)direction);
                }
            }
            else
            {
                // Als we tegen een muur aanlopen of van een klif aflopen, begin met wachten.
                if (Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                    Level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable)
                {
                    waitTime = MaxWaitTime;
                }
                else
                {
                    // Beweeg in de huidige richting.
                    Vector2 velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                    position = position + velocity;
                }
            }
        }

        /// <summary>
        /// Tekent de geanimeerde vijand.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Stop met rennen wanneer het spel is gepauzeerd of voordat je je omdraait.
            if (!Level.Player.IsAlive ||
                Level.ReachedExit ||
                Level.TimeRemaining == TimeSpan.Zero ||
                waitTime > 0)
            {
                sprite.PlayAnimation(idleAnimation);
            }
            else
            {
                sprite.PlayAnimation(runAnimation);
            }


            // Tekent met het gezicht in de richting waarin de vijand beweegt.
            SpriteEffects flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }

      
    }
}
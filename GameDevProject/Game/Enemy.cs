using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameDevProject.Game
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
    public class Enemy
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
            // To Do

            // Bereken grenzen binnen texture grootte.
       	   // To Do
        }


        /// <summary>
        /// Loopt heen en weer langs een platform, wachtend aan beide uiteinden.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // To Do

            // Bereken tegel positie op basis van de kant waar we naartoe lopen.
          // To Do

            if (waitTime > 0)
            {
                // Wacht gedurende enige tijd.
                // To Do
                if (waitTime <= 0.0f)
                {
                    // Draai dan om.
                    // To Do
                }
            }
            else
            {
                // Als we tegen een muur aanlopen of van een klif aflopen, begin met wachten.
                if (Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                    Level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable)
                {
                    // To Do
                }
                else
                {
                    // Beweeg in de huidige richting.
                    // To Do
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
                // To Do
            }
            else
            {
                // To Do
            }


            // Tekent met het gezicht in de richting waarin de vijand beweegt.
            // To Do
        }
    }
}

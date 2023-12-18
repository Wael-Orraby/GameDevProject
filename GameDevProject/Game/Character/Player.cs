using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameDevProject.Game.AnimationsFolder;
using GameDevProject.Game.LevelDesign;
using GameDevProject.Game.Mechanics;

namespace GameDevProject.Game.Character
{
    public class Player : ICharacter
    {
        // Animaties
        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation jumpAnimation;
        private Animation celebrateAnimation;
        private Animation dieAnimation;
        private SpriteEffects flip = SpriteEffects.None;
        private AnimationPlayer sprite;

        // Geluiden
        private SoundEffect killedSound;
        private SoundEffect jumpSound;
        private SoundEffect fallSound;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        // Fysische toestand
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        private float previousBottom;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        // Constanten voor het controleren van horizontale beweging
        private const float MoveAcceleration = 13000.0f;
        private const float MaxMoveSpeed = 1750.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;

        // Constanten voor het controleren van verticale beweging
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -3500.0f;
        private const float GravityAcceleration = 3400.0f;
        private const float MaxFallSpeed = 550.0f;
        private const float JumpControlPower = 0.14f;

        // Inputconfiguratie
        private const float MoveStickScale = 1.0f;
        private const float AccelerometerScale = 1.5f;
        private const Buttons JumpButton = Buttons.A;

        /// <summary>
        /// Geeft aan of de voeten van de speler al dan niet op de grond staan.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        /// <summary>
        /// Huidige invoer van gebruikersbeweging.
        /// </summary>
        private float movement;

        // Springstatus
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        private Rectangle localBounds;
        /// <summary>
        /// Geeft een rechthoek die deze speler begrenst in wereldruimte.
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

        /// <summary>
        /// Cre ert een nieuwe speler.
        /// </summary>
        public Player(Level level, Vector2 position)
        {
            this.level = level;

            LoadContent();

            Reset(position);
        }

        /// <summary>
        /// Laadt het spritesheet en geluiden van de speler.
        /// </summary>
        public void LoadContent()
        {
            // Laad geanimeerde textures.
            idleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.1f, true);
            runAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Run"), 0.1f, true);
            jumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Jump"), 0.1f, false);
            celebrateAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Celebrate"), 0.1f, false);
            dieAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Die"), 0.1f, false);


            // Bereken begrenzingen binnen de grootte van de texture .           
            int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameHeight * 0.8);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            // Laad geluiden.            
            killedSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
            jumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
            fallSound = Level.Content.Load<SoundEffect>("Sounds/PlayerFall");
        }

        /// <summary>
        /// Reset de speler naar het leven.
        /// </summary>
        /// <param name="position">De positie waar de speler tot leven komt.</param>
        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            isAlive = true;
            sprite.PlayAnimation(idleAnimation);
        }

        /// <summary>
        /// Handelt invoer af, voert natuurkunde uit en animeert de speler sprite.
        /// </summary>
        public void Update(
            GameTime gameTime,
            KeyboardState keyboardState,
            GamePadState gamePadState,
            AccelerometerState accelState,
            DisplayOrientation orientation)
        {
            GetInput(keyboardState, gamePadState, accelState, orientation);

            ApplyPhysics(gameTime);

            if (IsAlive && IsOnGround)
            {
                if (Math.Abs(Velocity.X) - 0.02f > 0)
                {
                    sprite.PlayAnimation(runAnimation);
                }
                else
                {
                    sprite.PlayAnimation(idleAnimation);
                }
            }

            // Clear input.
            movement = 0.0f;
            isJumping = false;
        }

        /// <summary>
        /// Haalt horizontale bewegings- en springopdrachten van de speler op vanaf de invoer.
        /// </summary>
        private void GetInput(
            KeyboardState keyboardState,
            GamePadState gamePadState,
            AccelerometerState accelState,
            DisplayOrientation orientation)
        {
            // Haal analoge horizontale beweging op.
            movement = gamePadState.ThumbSticks.Left.X * MoveStickScale;

            // Negeer kleine bewegingen om rennen op dezelfde plek te voorkomen.
            if (Math.Abs(movement) < 0.5f)
                movement = 0.0f;

            // Beweeg de speler met de versnellingsmeter.
            if (Math.Abs(accelState.Acceleration.Y) > 0.10f)
            {
                // stel onze bewegingssnelheid in
                movement = MathHelper.Clamp(-accelState.Acceleration.Y * AccelerometerScale, -1f, 1f);

                // als we in de LandscapeLeft-oriëntatie zijn, moeten we onze beweging omkeren
                if (orientation == DisplayOrientation.LandscapeRight)
                    movement = -movement;
            }

            // Als er enige digitale horizontale bewegingsinvoer is gevonden, overschrijf dan de analoge beweging.
            if (gamePadState.IsButtonDown(Buttons.DPadLeft) ||
                keyboardState.IsKeyDown(Keys.Left) ||
                keyboardState.IsKeyDown(Keys.A))
            {
                movement = -1.0f;
            }
            else if (gamePadState.IsButtonDown(Buttons.DPadRight) ||
                     keyboardState.IsKeyDown(Keys.Right) ||
                     keyboardState.IsKeyDown(Keys.D))
            {
                movement = 1.0f;
            }

            // Controleer of de speler wil springen.
            isJumping =
                gamePadState.IsButtonDown(JumpButton) ||
                keyboardState.IsKeyDown(Keys.Space) ||
                keyboardState.IsKeyDown(Keys.Up) ||
                keyboardState.IsKeyDown(Keys.W);
        }

        /// <summary>
        /// Update de snelheid en positie van de speler op basis van invoer, zwaartekracht, enz.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            // Basissnelheid is een combinatie van horizontale bewegingsbesturing en
            // versnelling naar beneden als gevolg van zwaartekracht.
            velocity.X += movement * MoveAcceleration * elapsed;
            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            velocity.Y = DoJump(velocity.Y, gameTime);

            // Pas pseudo-drag horizontaal toe.
            if (IsOnGround)
                velocity.X *= GroundDragFactor;
            else
                velocity.X *= AirDragFactor;

            // Voorkom dat de speler sneller rent dan zijn topsnelheid.
            velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Pas snelheid toe.
            Position += velocity * elapsed;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // Als de speler nu botst met het level, scheid ze dan.
            HandleCollisions();

            // Als de botsing ons heeft gestopt met bewegen, zet de snelheid dan terug op nul.
            if (Position.X == previousPosition.X)
                velocity.X = 0;

            if (Position.Y == previousPosition.Y)
                velocity.Y = 0;
        }

        /// <summary>
        /// Berekent de Y-snelheid rekening houdend met springen en
        /// animeert dienovereenkomstig.
        /// </summary>
        /// <remarks>
        /// Tijdens de opkomst van een sprong wordt de Y-snelheid volledig
        /// overschreven door een krachtcurve. Tijdens de daling neemt de zwaartekracht over.
        /// De springsnelheid wordt gecontroleerd door het jumpTime-veld
        /// dat de tijd in de opkomst van de huidige sprong meet.
        /// </remarks>
        /// <param name="velocityY">
        /// De huidige snelheid van de speler langs de Y-as.
        /// </param>
        /// <returns>
        /// Een nieuwe Y-snelheid als begin of voortzetting van een sprong.
        /// Anders de bestaande Y-snelheid.
        /// </returns>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // Als de speler wil springen
            if (isJumping)
            {
                // Begin of zet een sprong voort
                if (!wasJumping && IsOnGround || jumpTime > 0.0f)
                {
                    if (jumpTime == 0.0f)
                        jumpSound.Play();

                    jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    sprite.PlayAnimation(jumpAnimation);
                }

                // Als we in de opkomst van de sprong zitten
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Overschrijf de verticale snelheid volledig met een krachtcurve die spelers meer controle geeft over de top van de sprong
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Bereikte het hoogtepunt van de sprong
                    jumpTime = 0.0f;
                }
            }
            else
            {
                // Gaat niet springen of annuleert een lopende sprong
                jumpTime = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }

        /// <summary>
        /// Detecteert en lost alle botsingen op tussen de speler en zijn omliggende
        /// tegels. Wanneer een botsing wordt gedetecteerd, wordt de speler weggeduwd langs één
        /// as om overlapping te voorkomen. Er is enige speciale logica voor de Y-as om
        /// platforms af te handelen die zich anders gedragen afhankelijk van de bewegingsrichting.
        /// </summary>
        private void HandleCollisions()
        {
            // Haal de begrenzingsrechthoek van de speler op en zoek omliggende tegels.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling((float)bounds.Right / Tile.Width) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling((float)bounds.Bottom / Tile.Height) - 1;

            // Reset vlag om te zoeken naar bodembotsingen.
            isOnGround = false;

            // Voor elke potentieel botsende tegel,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // Als deze tegel botsbaar is,
                    TileCollision collision = Level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Bepaal de botsingsdiepte (met richting) en magnitude.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        Vector2 depth = bounds.GetIntersectionDepth(tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Los de botsing op langs de ondiepe as.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // Als we de bovenkant van een tegel hebben overschreden, staan we op de grond.
                                if (previousBottom <= tileBounds.Top)
                                    isOnGround = true;

                                // Negeer platforms, tenzij we op de grond staan.
                                if (collision == TileCollision.Impassable || IsOnGround)
                                {
                                    // Los de botsing op langs de Y-as.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Voer verdere botsingen uit met de nieuwe begrenzingen.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Negeer platforms.
                            {
                                // Los de botsing op langs de X-as.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Voer verdere botsingen uit met de nieuwe begrenzingen.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            // Sla de nieuwe onderkant van de begrenzingen op.
            previousBottom = bounds.Bottom;
        }

        /// <summary>
        /// Aangeroepen wanneer de speler is gedood.
        /// </summary>
        /// <param name="killedBy">
        /// De vijand die de speler heeft gedood. Deze parameter is null als de speler niet is
        /// gedood door een vijand (in een gat is gevallen).
        /// </param>
        public void OnKilled(Enemy killedBy)
        {
            isAlive = false;

            if (killedBy != null)
                killedSound.Play();
            else
                fallSound.Play();

            sprite.PlayAnimation(dieAnimation);
        }

        /// <summary>
        /// Aangeroepen wanneer deze speler de uitgang van het level bereikt.
        /// </summary>
        public void OnReachedExit()
        {
            sprite.PlayAnimation(celebrateAnimation);
        }


        /// <summary>
        /// Tekent de geanimeerde speler.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Keert de sprite om zodat deze de kant op kijkt waarheen we bewegen.
            if (Velocity.X > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X < 0)
                flip = SpriteEffects.None;

            // Teken die sprite.
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }
    }
}

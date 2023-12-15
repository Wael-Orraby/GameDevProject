using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Content;
using GameDevProject.Game;
using Microsoft.Xna.Framework.Media;
using System;
using GameDevProject.Game.Klas_Screens;
using SharpDX.Direct2D1;

namespace GameDevProject
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
// Hulpbronnen voor tekenen.
 private GraphicsDeviceManager graphics;
 private Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch;
 Vector2 baseScreenSize = new Vector2(800, 480);
 private Matrix globalTransformation;
 int backbufferWidth, backbufferHeight;

 // Global content.
 private SpriteFont hudFont;

 private Texture2D winOverlay;
 private Texture2D loseOverlay;
 private Texture2D diedOverlay;

 // Startknop
 private Texture2D startButtonTexture;
 private Rectangle startButtonRectangle;
 private StartScreen startScreen;
 private bool isGameStarted;

 //start backgroundImage
 private Texture2D backgroundImage;

 // Meta-level game state.
 private int levelIndex = -1;
 private Level level;
 private bool wasContinuePressed;

// Wanneer de resterende tijd korter is dan de waarschuwingstijd, knippert deze op de hud
 private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

// We slaan onze invoerstatussen op, zodat we slechts één keer per frame pollen,
// dan gebruiken we waar nodig dezelfde invoerstatus
 private GamePadState gamePadState;
 private KeyboardState keyboardState;
 private TouchCollection touchState;
 private AccelerometerState accelerometerState;

 private VirtualGamePad virtualGamePad;

  // Het aantal niveaus in de map Niveaus van onze inhoud. We gaan ervan uit dat
  // niveaus in onze inhoud zijn gebaseerd op 0 en dat alle getallen onder deze constante liggen
  // Zorg ervoor dat er een niveaubestand aanwezig is. Hierdoor hoeven we niet naar het bestand te zoeken
  // of uitzonderingen afhandelen, die beide onnodige tijd kunnen toevoegen aan het laden van niveaus.
 private const int numberOfLevels = 3;

 public Game1()
 {
     graphics = new GraphicsDeviceManager(this);


     graphics.IsFullScreen = false;

     //graphics.PreferredBackBufferWidth = 800;
     //graphics.PreferredBackBufferHeight = 480;
     graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

     Accelerometer.Initialize();
 }
  /// <summary>
 /// LoadContent wordt één keer per game aangeroepen en is de plek om te laden
  /// al uw inhoud.
 /// </summary>
 protected override void LoadContent()
 {
     this.Content.RootDirectory = "Content";

     // Maak een nieuwe SpriteBatch, die kan worden gebruikt om texturen te tekenen.
     spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);

     // laad fonts
     hudFont = Content.Load<SpriteFont>("Fonts/Hud");

     // laad overlay textures
     winOverlay = Content.Load<Texture2D>("Overlays/you_win");
     loseOverlay = Content.Load<Texture2D>("Overlays/you_lose");
     diedOverlay = Content.Load<Texture2D>("Overlays/you_died");

     ScalePresentationArea();

     virtualGamePad = new VirtualGamePad(baseScreenSize, globalTransformation, Content.Load<Texture2D>("Sprites/VirtualControlArrow"));
    
     
     // Startknop
     int buttonWidth = 250;
     int buttonHeight = 200;
     int buttonX = (GraphicsDevice.Viewport.Width - buttonWidth) / 2;
     int buttonY = (GraphicsDevice.Viewport.Height - buttonHeight) / 2;
     startButtonTexture = Content.Load<Texture2D>("Buttons/StartButtonImage");
     startButtonRectangle = new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight);
     startScreen = new StartScreen(startButtonTexture, startButtonRectangle, null);

     // Laad de achtergrondafbeelding
     backgroundImage = Content.Load<Texture2D>("Backgrounds/Layer1_2");

     // Laad music
     try
     {
         MediaPlayer.IsRepeating = true;
         MediaPlayer.Play(Content.Load<Song>("Sounds/Music"));
     }
     catch { }

     LoadNextLevel();

 }
 
        public void ScalePresentationArea()
        {
            //Bereken hoeveel we nodig hebben om onze afbeeldingen te schalen om het scherm te vullen
            backbufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            backbufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            float horScaling = backbufferWidth / baseScreenSize.X;
            float verScaling = backbufferHeight / baseScreenSize.Y;
            Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);
            System.Diagnostics.Debug.WriteLine("Screen Size - Width[" + GraphicsDevice.PresentationParameters.BackBufferWidth + "] Height [" + GraphicsDevice.PresentationParameters.BackBufferHeight + "]");
        }


        /// <summary>
        /// Hiermee kan het spel logica uitvoeren, zoals het updaten van de wereld,
         /// controleren op botsingen, input verzamelen en audio afspelen.
        /// </summary>
        /// <param name="gameTime">Biedt een momentopname van timingwaarden.</param>
        protected override void Update(GameTime gameTime)
        {
            //Bevestig dat het formaat van het scherm niet door de gebruiker is aangepast
            if (backbufferHeight != GraphicsDevice.PresentationParameters.BackBufferHeight ||
                backbufferWidth != GraphicsDevice.PresentationParameters.BackBufferWidth)
            {
                ScalePresentationArea();
            }
            // Behandel de polling voor onze input en zorg voor input op hoog niveau
            HandleInput(gameTime);

            // update ons niveau en geef de GameTime door, samen met al onze invoerstatussen
            level.Update(gameTime, keyboardState, gamePadState,
                         accelerometerState, Window.CurrentOrientation);

            if (level.Player.Velocity != Vector2.Zero)
                virtualGamePad.NotifyPlayerIsMoving();


            // update startknop
            MouseState mouseState = Mouse.GetState();
            if (!isGameStarted)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && startButtonRectangle.Contains(mouseState.Position))
                {
                    isGameStarted = true;
                }
                startScreen.Update(gameTime, mouseState);
            }

            base.Update(gameTime);
        }

        
        private void HandleInput(GameTime gameTime)
        {
            // haal al onze invoerstatussen op
            keyboardState = Keyboard.GetState();
            touchState = TouchPanel.GetState();
            gamePadState = virtualGamePad.GetState(touchState, GamePad.GetState(PlayerIndex.One));
            accelerometerState = Accelerometer.GetState();


            bool continuePressed =
                keyboardState.IsKeyDown(Keys.Space) ||
                gamePadState.IsButtonDown(Buttons.A) ||
                touchState.AnyTouch();

            // Voer de juiste actie uit om het spel vooruit te helpen en
            // om de speler weer aan het spelen te krijgen.
            if (!wasContinuePressed && continuePressed)
            {
                if (!level.Player.IsAlive)
                {
                    level.StartNewLife();
                }
                else if (level.TimeRemaining == TimeSpan.Zero)
                {
                    if (level.ReachedExit)
                        LoadNextLevel();
                    else
                        ReloadCurrentLevel();
                }
            }

            wasContinuePressed = continuePressed;

            virtualGamePad.Update(gameTime);
        }

        private void LoadNextLevel()
        {
            // ga naar het volgende niveau
            levelIndex = (levelIndex + 1) % numberOfLevels;

            // Verwijdert de inhoud van het huidige niveau voordat het volgende wordt geladen.
            if (level != null)
                level.Dispose();

            // Laad het niveau.
            string levelPath = string.Format("Content/Levels/{0}.txt", levelIndex);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                level = new Level(Services, fileStream, levelIndex);
        }

        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }
        /// <summary>
        /// Tekent het spel van achtergrond naar voorgrond.
        /// </summary>
        /// <param name="gameTime">Biedt een momentopname van timingwaarden.</param>
protected override void Draw(GameTime gameTime)
{
    graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

    spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, globalTransformation);

    // Startknop
    if (!isGameStarted)
    {
        // Teken de achtergrondafbeelding op het hele scherm
        spriteBatch.Draw(backgroundImage, GraphicsDevice.Viewport.Bounds, Color.White);
        startScreen.Draw(spriteBatch);
        this.IsMouseVisible = true;
    }
    else
    {
        level.Draw(gameTime, spriteBatch);
        this.IsMouseVisible = false;

        DrawHud();
    }



    spriteBatch.End();

    base.Draw(gameTime);
}

private void DrawHud()
{
    Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
    Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);

    Vector2 center = new Vector2(baseScreenSize.X / 2, baseScreenSize.Y / 2);

    // Teken de resterende tijd. Gebruikt modulo-deling om knipperen te veroorzaken wanneer de
     // speler heeft bijna geen tijd meer.
    string timeString = "TIME: " + level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00");
    Color timeColor;
    if (level.TimeRemaining > WarningTime ||
        level.ReachedExit ||
        (int)level.TimeRemaining.TotalSeconds % 2 == 0)
    {
        timeColor = Color.White;
    }
    else
    {
        timeColor = Color.Red;
    }
    DrawShadowedString(hudFont, timeString, hudLocation, timeColor);

    // Teken score
    float timeHeight = hudFont.MeasureString(timeString).Y;
    DrawShadowedString(hudFont, "SCORE: " + level.Score.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f), Color.White);

    // Bepaal welk statusoverlaybericht moet worden weergegeven.
    Texture2D status = null;
    if (level.TimeRemaining == TimeSpan.Zero)
    {
        if (level.ReachedExit)
        {
            status = winOverlay;
        }
        else
        {
            status = loseOverlay;
        }
    }
    else if (!level.Player.IsAlive)
    {
        status = diedOverlay;
    }

    if (status != null)
    {
        // Teken status bericht.
        Vector2 statusSize = new Vector2(status.Width, status.Height);
        spriteBatch.Draw(status, center - statusSize / 2, Color.White);
    }

    if (touchState.IsConnected)
        virtualGamePad.Draw(spriteBatch);
}

private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
{
    spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
    spriteBatch.DrawString(font, value, position, color);
}

    }
}

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
    }
}

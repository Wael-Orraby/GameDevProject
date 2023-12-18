using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Microsoft.Xna.Framework.Input;
using testGame.Game.Factory;

namespace GameDevProject.Game
{
    public class Level
    {
        // Fysieke structuur van het niveau.
        private Tile[,] tiles;
        private Texture2D[] layers;
        // De laag waarop entiteiten worden getekend.
        private const int EntityLayer = 2;

        // Entiteiten in het niveau.
        public Player Player
        {
            get { return player; }
        }
        Player player;

        private List<Gem> gems = new List<Gem>();
        private List<Enemy> enemies = new List<Enemy>();

        // Belangrijke locaties in het niveau.
        private Vector2 start;
        private Point exit = InvalidPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);

        // Spelstatus van het niveau.
        private Random random = new Random(354668); // Willekeurig, maar constant zaad

        public int Score
        {
            get { return score; }
        }
        int score;

        public bool ReachedExit
        {
            get { return reachedExit; }
        }
        bool reachedExit;

        public TimeSpan TimeRemaining
        {
            get { return timeRemaining; }
        }
        TimeSpan timeRemaining;

        private const int PointsPerSecond = 5;

        // Inhoud van het niveau.
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        private SoundEffect exitReachedSound;

        #region Laden

        /// <summary>
        /// Maakt een nieuw niveau.
        /// </summary>
        /// <param name="serviceProvider">
        /// De serviceprovider die zal worden gebruikt om een ContentManager te maken.
        /// </param>
        /// <param name="fileStream">
        /// Een stream met de tegelgegevens.
        /// </param>
        public Level(IServiceProvider serviceProvider, Stream fileStream, int levelIndex)
        {
            // Maak een nieuwe contentmanager om content te laden die alleen door dit niveau wordt gebruikt.
            content = new ContentManager(serviceProvider, "Content");

            timeRemaining = TimeSpan.FromMinutes(0.5);

            LoadTiles(fileStream);

            // Laad texturen voor achtergrondlagen. Voor nu moeten alle niveaus dezelfde achtergronden gebruiken
            // en alleen het meest linkse deel ervan gebruiken.
            layers = new Texture2D[3];
            for (int i = 0; i < layers.Length; ++i)
            {
                // Kies een willekeurig segment voor elke achtergrondlaag voor variatie in niveaus.
                int segmentIndex = levelIndex;
                layers[i] = Content.Load<Texture2D>("Backgrounds/Layer0_0");
            }

            // Laad geluiden.
            exitReachedSound = Content.Load<SoundEffect>("Sounds/ExitReached");
        }

        /// <summary>
        /// Doorloopt elke tegel in het structuurbestand en laadt het uiterlijk en gedrag.
        /// Deze methode controleert ook of het bestand goed is gevormd met een startpunt voor de speler, uitgang, enz.
        /// </summary>
        /// <param name="fileStream">
        /// Een stream met de tegelgegevens.
        /// </param>
        private void LoadTiles(Stream fileStream)
        {
            // Laad het niveau en zorg ervoor dat alle regels dezelfde lengte hebben.
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("De lengte van regel {0} verschilt van alle voorgaande regels.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Wijs het tegelrooster toe.
            tiles = new Tile[width, lines.Count];

            // Loop over elke tegelpositie,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // om elke tegel te laden.
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }

            // Controleer of het niveau een begin en een einde heeft.
            if (Player == null)
                throw new NotSupportedException("Een niveau moet een startpunt hebben.");
            if (exit == InvalidPosition)
                throw new NotSupportedException("Een niveau moet een uitgang hebben.");

        }

        /// <summary>
        /// Laadt het uiterlijk en gedrag van een individuele tegel.
        /// </summary>
        /// <param name="tileType">
        /// Het karakter geladen uit het structuurbestand dat aangeeft wat geladen moet worden.
        /// </param>
        /// <param name="x">
        /// De X-locatie van deze tegel in tegelruimte.
        /// </param>
        /// <param name="y">
        /// De Y-locatie van deze tegel in tegelruimte.
        /// </param>
        /// <returns>De geladen tegel.</returns>
        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // Lege ruimte
                case '.':
                    return new Tile(null, TileCollision.Passable);

                // Uitgang
                case 'X':
                    return LoadExitTile(x, y);

                // Edelsteen
                case 'G':
                    return LoadGemTile(x, y);

                // Zwevend platform
                case '-':
                    return LoadTile("Platform", TileCollision.Platform);

                // Diverse vijanden
                case 'A':
                    return LoadEnemyTile(x, y, "MonsterA");
                case 'B':
                    return LoadEnemyTile(x, y, "MonsterB");
                case 'C':
                    return LoadEnemyTile(x, y, "MonsterC");
                case 'D':
                    return LoadEnemyTile(x, y, "MonsterD");
                case 'S':
                    return LoadEnemyTile(x, y, "Spikes");
                case 'L':
                    return LoadEnemyTile(x, y, "AnimalEnemy");

                // Platformblok
                case '~':
                    return LoadVarietyTile("BlockB", 2, TileCollision.Platform);

                // Doorlaatbaar blok
                case ':':
                    return LoadVarietyTile("BlockB", 2, TileCollision.Passable);

                // Startpunt speler 1
                case '1':
                    return LoadStartTile(x, y);

                // Onbreekbaar blok
                case '#':
                    return LoadVarietyTile("BlockA", 7, TileCollision.Impassable);

                // Onbekend teken voor tegeltype
                default:
                    throw new NotSupportedException(String.Format("Niet-ondersteund teken voor tegeltype '{0}' op positie {1}, {2}.", tileType, x, y));
            }
        }

        /// <summary>
        /// Maakt een nieuwe tegel. De andere methoden voor het laden van tegels ketenen meestal naar deze methode
        /// nadat ze hun specifieke logica hebben uitgevoerd.
        /// </summary>
        /// <param name="name">
        /// Pad naar een tegeltextuur ten opzichte van de Content/Tiles-directory.
        /// </param>
        /// <param name="collision">
        /// Het type tegelbotsing voor de nieuwe tegel.
        /// </param>
        /// <returns>De nieuwe tegel.</returns>
        private Tile LoadTile(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
        }


        /// <summary>
        /// Laadt een tegel met een willekeurig uiterlijk.
        /// </summary>
        /// <param name="baseName">
        /// De inhoudsnaamprefix voor deze groep variaties van tegels. Tegelgroepen hebben namen zoals This0.png en This1.png en This2.png.
        /// </param>
        /// <param name="variationCount">
        /// Het aantal variaties in deze groep.
        /// </param>
        private Tile LoadVarietyTile(string baseName, int variationCount, TileCollision collision)
        {
            int index = random.Next(variationCount);
            return LoadTile(baseName + index, collision);
        }


        /// <summary>
        /// Instantieert een speler, plaatst hem in het niveau en onthoudt waar hij moet worden geplaatst wanneer hij weer tot leven komt.
        /// </summary>
        private Tile LoadStartTile(int x, int y)
        {
            if (Player != null)
                throw new NotSupportedException("Een niveau mag slechts één startpunt hebben.");

            start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            player = new Player(this, start);

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Onthoudt de locatie van de uitgang van het niveau.
        /// </summary>
        private Tile LoadExitTile(int x, int y)
        {
            if (exit != InvalidPosition)
                throw new NotSupportedException("Een niveau mag slechts één uitgang hebben.");

            exit = GetBounds(x, y).Center;

            return LoadTile("Exit", TileCollision.Passable);
        }

        /// <summary>
        /// Instantieert een vijand en plaatst hem in het niveau.
        /// </summary>
       private Tile LoadEnemyTile(int x, int y, string spriteSet)
 {
     EnemyFactory enemyFactory = new EnemyFactory(this);
        
     Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
     enemies.Add(enemyFactory.CreateEnemy(position, spriteSet)/*new Enemy(this, position, spriteSet)*/);

     return new Tile(null, TileCollision.Passable);
 }

        /// <summary>
        /// Instantieert een edelsteen en plaatst deze in het niveau.
        /// </summary>
        private Tile LoadGemTile(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            gems.Add(new Gem(this, new Vector2(position.X, position.Y)));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Ontlaadt de inhoud van het niveau.
        /// </summary>
        public void Dispose()
        {
            Content.Unload();
        }

        #endregion

        #region Grenzen en botsingen

        /// <summary>
        /// Geeft de botsingsmodus van de tegel op een bepaalde locatie.
        /// Deze methode behandelt tegels buiten de grenzen van het niveau door te voorkomen dat je voorbij de linkse of rechtse randen kunt ontsnappen,
        /// maar staat toe dat dingen voorbij de bovenkant van het niveau springen en van de onderkant vallen.
        /// </summary>
        public TileCollision GetCollision(int x, int y)
        {
            // Voorkom ontsnappen voorbij de niveaugrenzen.
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            // Sta toe om voorbij de bovenkant van het niveau te springen en door de onderkant te vallen.
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        /// <summary>
        /// Geeft het begrenzingsrechthoek van een tegel in wereldruimte.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        /// <summary>
        /// Breedte van het niveau gemeten in tegels.
        /// </summary>
        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        /// <summary>
        /// Hoogte van het niveau gemeten in tegels.
        /// </summary>
        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        #endregion

        #region Update

        /// <summary>
        /// Update alle objecten in de wereld, voert botsingen tussen hen uit
        /// en behandelt de tijdslimiet met scoren.
        /// </summary>
        public void Update(
            GameTime gameTime,
            KeyboardState keyboardState,
            GamePadState gamePadState,
            AccelerometerState accelState,
            DisplayOrientation orientation)
        {
            // Pauzeer wanneer de speler dood is of de tijd verstreken is.
            if (!Player.IsAlive || TimeRemaining == TimeSpan.Zero)
            {
                // Wil nog steeds natuurkunde op de speler uitvoeren.
                Player.ApplyPhysics(gameTime);
            }
            else if (ReachedExit)
            {
                // Animeer de tijd die wordt omgezet in punten.
                int seconds = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0f);
                seconds = Math.Min(seconds, (int)Math.Ceiling(TimeRemaining.TotalSeconds));
                timeRemaining -= TimeSpan.FromSeconds(seconds);
                score += seconds * PointsPerSecond;
            }
            else
            {
                timeRemaining -= gameTime.ElapsedGameTime;
                Player.Update(gameTime, keyboardState, gamePadState, accelState, orientation);
                UpdateGems(gameTime);

                // Van de onderkant van het niveau vallen doodt de speler.
                if (Player.BoundingRectangle.Top >= Height * Tile.Height)
                    OnPlayerKilled(null);

                UpdateEnemies(gameTime);

                // De speler heeft de uitgang bereikt als hij op de grond staat en
                // zijn begrenzingsrechthoek het midden van de uitgangstegel bevat. Ze kunnen alleen
                // vertrekken wanneer ze alle edelstenen hebben verzameld.
                if (Player.IsAlive &&
                    Player.IsOnGround &&
                    Player.BoundingRectangle.Contains(exit))
                {
                    OnExitReached();
                }
            }

            // Klem de resterende tijd op nul.
            if (timeRemaining < TimeSpan.Zero)
                timeRemaining = TimeSpan.Zero;
        }

        /// <summary>
        /// Animeert elke edelsteen en controleert of de speler ze kan verzamelen.
        /// </summary>
        private void UpdateGems(GameTime gameTime)
        {
            for (int i = 0; i < gems.Count; ++i)
            {
                Gem gem = gems[i];

                gem.Update(gameTime);

                if (gem.BoundingCircle.Intersects(Player.BoundingRectangle))
                {
                    gems.RemoveAt(i--);
                    OnGemCollected(gem, Player);
                }
            }
        }

        /// <summary>
        /// Animeert elke vijand en staat toe dat ze de speler doden.
        /// </summary>
        private void UpdateEnemies(GameTime gameTime)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime);

                // Het aanraken van een vijand doodt de speler direct.
                if (enemy.BoundingRectangle.Intersects(Player.BoundingRectangle))
                {
                    OnPlayerKilled(enemy);
                }
            }
        }


        /// <summary>
        /// Wordt aangeroepen als je een muntje pakt
        /// </summary>
        /// <param name="gem">muntje gepakt.</param>
        /// <param name="collectedBy">player pakt muntje</param>
        private void OnGemCollected(Gem gem, Player collectedBy)
        {
            score += gem.PointValue;

            gem.OnCollected(collectedBy);
        }

        /// <summary>
        /// aangeroepen als speler dood gaat
        /// </summary>
        /// <param name="killedBy">
        /// een enemy die een player killt
        /// of enemy die door een gat valt
        /// </param>
        private void OnPlayerKilled(Enemy killedBy)
        {
            Player.OnKilled(killedBy);
        }

        /// <summary>
        /// wanneer de speler het exit bordje raakt
        /// </summary>
        private void OnExitReached()
        {
            Player.OnReachedExit();
            exitReachedSound.Play();
            reachedExit = true;
        }

        /// <summary>
        /// wanneer die dood gaat start een nieuw leven
        /// </summary>
        public void StartNewLife()
        {
            Player.Reset(start);
        }

        #endregion

        #region Draw

        /// <summary>
        /// teken background en foreground
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int i = 0; i <= EntityLayer; ++i)
                spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);

            DrawTiles(spriteBatch);

            foreach (Gem gem in gems)
                gem.Draw(gameTime, spriteBatch);

            Player.Draw(gameTime, spriteBatch);

            foreach (Enemy enemy in enemies)
                enemy.Draw(gameTime, spriteBatch);

            for (int i = EntityLayer + 1; i < layers.Length; ++i)
                spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);
        }

        /// <summary>
        /// teken alle tegels
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch)
        {
            // For each tile position
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        // Draw it in screen space.
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }
        }

        #endregion

    }
}

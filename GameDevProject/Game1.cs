using GameDevProject.Game.Klas_Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDevProject
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Startknop
        private Texture2D startButtonTexture;
        private Rectangle startButtonRectangle;
        private StartScreen startScreen;
        private bool isGameStarted;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            isGameStarted = false;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Startknop
            int buttonWidth = 250;
            int buttonHeight = 200;
            int buttonX = (GraphicsDevice.Viewport.Width - buttonWidth) / 2;
            int buttonY = (GraphicsDevice.Viewport.Height - buttonHeight) / 2;
            startButtonTexture = Content.Load<Texture2D>("Buttons/StartButtonImage");
            startButtonRectangle = new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight);
            startScreen = new StartScreen(startButtonTexture, startButtonRectangle, null);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            // Startknop
            MouseState mouseState = Mouse.GetState();
            if (!isGameStarted)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && startButtonRectangle.Contains(mouseState.Position))
                {
                    isGameStarted = true;
                    GraphicsDevice.Clear(Color.Red);
                }
                startScreen.Update(gameTime, mouseState);
            }
            else
            {
                isGameStarted = false;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            // TODO: Add your drawing code here


            // Startknop
            if (!isGameStarted)
            {
                startScreen.Draw(_spriteBatch);
            }
            else
            {
                GraphicsDevice.Clear(Color.Yellow);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

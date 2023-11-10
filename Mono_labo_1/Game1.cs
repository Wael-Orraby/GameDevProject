using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Mono_labo_1.Klas_Interfaces;
using Mono_labo_1.Klas_Input;
using Mono_labo_1.Klas_Screens;
using Mono_labo_1.Caracters;
using Mono_labo_1.Klas_Keyboard;

namespace Mono_labo_1
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D clopsTexture;
        private Clops clops;

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
            _graphics.PreferredBackBufferWidth = 1900;
            _graphics.PreferredBackBufferHeight = 920;
            _graphics.ApplyChanges();

            base.Initialize();
            isGameStarted = false;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            clopsTexture = Content.Load<Texture2D>("Cyclops Sprite Sheet");
            clops = new Clops(clopsTexture, new KeyboardReader(), GraphicsDevice);

            startButtonTexture = Content.Load<Texture2D>("Lovepik_com-401228855-start-button");
            int buttonWidth = 250;
            int buttonHeight = 200;
            int buttonX = (GraphicsDevice.Viewport.Width - buttonWidth) / 2;
            int buttonY = (GraphicsDevice.Viewport.Height - buttonHeight) / 2;
            startButtonRectangle = new Rectangle(buttonX, buttonY, buttonWidth, buttonHeight);
            startScreen = new StartScreen(startButtonTexture, startButtonRectangle, null);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseState mouseState = Mouse.GetState();
            if (!isGameStarted)
            {
                if (mouseState.LeftButton == ButtonState.Pressed && startButtonRectangle.Contains(mouseState.Position))
                {
                    isGameStarted = true;
                }
                startScreen.Update(gameTime, mouseState);
            }
            else
            {
                clops.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            if (!isGameStarted)
            {
                startScreen.Draw(_spriteBatch);
            }
            else
            {
                clops.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

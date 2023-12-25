using System.Collections.Generic;
using GameDevProject.Game.Klas_Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace GameDevProject.Game.Klas_Screens
{
    //    public class StartScreen
    //    {
    //        private Texture2D startButtonTexture;
    //        private Rectangle startButtonRectangle;
    //        private bool isStartButtonClicked;
    //        private IInputReader inputReader;

    //        public bool IsStartButtonClicked => isStartButtonClicked;

    //        public StartScreen(Texture2D startButtonTexture, Rectangle startButtonRectangle, IInputReader inputReader)
    //        {
    //            this.startButtonTexture = startButtonTexture;
    //            this.startButtonRectangle = startButtonRectangle;
    //            this.inputReader = inputReader;
    //        }

    //        public void Update(GameTime gameTime, MouseState mouseState)
    //        {
    //            if (mouseState.LeftButton == ButtonState.Pressed && startButtonRectangle.Contains(mouseState.Position))
    //            {
    //                isStartButtonClicked = true;
    //            }
    //        }

    //        public void Draw(SpriteBatch spriteBatch)
    //        {
    //            spriteBatch.Draw(startButtonTexture, startButtonRectangle, Color.White);
    //        }
    //    }



    // StartScreen class
    public class StartScreen
{
    private Texture2D startButtonTexture;
    private Rectangle startButtonRectangle;
    private List<ICommand> commands;
    private IInputReader inputReader;

    public StartScreen(Texture2D startButtonTexture, Rectangle startButtonRectangle, IInputReader inputReader)
    {
        this.startButtonTexture = startButtonTexture;
        this.startButtonRectangle = startButtonRectangle;
        this.inputReader = inputReader;
        this.commands = new List<ICommand>();
    }

    public void AddCommand(ICommand command)
    {
        commands.Add(command);
    }

    public void Update(GameTime gameTime, MouseState mouseState)
    {
        if (mouseState.LeftButton == ButtonState.Pressed && startButtonRectangle.Contains(mouseState.Position))
        {
            foreach (var command in commands)
            {
                command.Execute();
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(startButtonTexture, startButtonRectangle, Color.White);
    }
}

}

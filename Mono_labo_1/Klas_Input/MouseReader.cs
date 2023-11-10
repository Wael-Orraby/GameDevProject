using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Mono_labo_1.Klas_Interfaces;
using System;

namespace Mono_labo_1.Klas_Input
{
    internal class MouseReader : IInputReader
    {
        private MouseState previousMouseState;
        private MouseState currentMouseState;

        public bool IsDestinationInput => true;

        public bool IsKeyPressed(Keys key)
        {
            return false; // Muis leest geen toetsenbordinvoer
        }

        public Vector2 ReadInput()
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            return new Vector2(currentMouseState.X, currentMouseState.Y);
        }

        public bool IsMouseClicked()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Mono_labo_1.Klas_Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mono_labo_1.Klas_Keyboard
{
    internal class KeyboardReader : IInputReader
    {
        public bool IsDestinationInput => false;


        public Vector2 ReadInput()
        {
            KeyboardState state = Keyboard.GetState();
            Vector2 direction = Vector2.Zero;
            if (state.IsKeyDown(Keys.Left))
            {
                direction.X -= 1;
            }
            if (state.IsKeyDown(Keys.Right))
            {
                direction.X += 1;
            }
            if (state.IsKeyDown(Keys.Up))
            {
                direction.Y -= 1;
            }
            if (state.IsKeyDown(Keys.Down))
            {
                direction.Y += 1;
            }
            
            return direction;   
        }
        public bool IsKeyPressed(Keys key)
        {
            KeyboardState state = Keyboard.GetState();
            return state.IsKeyDown(key);
        }
    }
}

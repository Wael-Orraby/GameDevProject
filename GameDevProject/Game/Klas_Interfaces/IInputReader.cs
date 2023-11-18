using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameDevProject.Game.Klas_Interfaces
{
    public interface IInputReader
    {
        Vector2 ReadInput();
        bool IsKeyPressed(Keys key);
        public bool IsDestinationInput { get; }
    }
}

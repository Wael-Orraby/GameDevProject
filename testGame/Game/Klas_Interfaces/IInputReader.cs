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

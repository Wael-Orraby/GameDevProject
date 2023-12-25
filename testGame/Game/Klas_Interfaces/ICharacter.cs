using Microsoft.Xna.Framework;

namespace GameDevProject.Game.Klas_Interfaces
{

    public interface ICharacter
    {
        Level Level { get; }

        Vector2 Position { get; }

        Rectangle BoundingRectangle { get; }
    }

}
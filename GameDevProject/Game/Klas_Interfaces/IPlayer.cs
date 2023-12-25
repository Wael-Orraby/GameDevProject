using GameDevProject.Game.Mechanics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameDevProject.Game.Character;

namespace GameDevProject.Game.Klas_Interfaces
{
    // Interface voor speler
    public interface IPlayer : ICharacter
    {
        bool IsAlive { get; }
        bool IsOnGround { get; }
        void Reset(Vector2 position);
        void OnKilled(Enemy killedBy);
        void OnReachedExit();
        void Update(GameTime gameTime, KeyboardState keyboardState, GamePadState gamePadState, AccelerometerState accelState, DisplayOrientation orientation);
        void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }

}

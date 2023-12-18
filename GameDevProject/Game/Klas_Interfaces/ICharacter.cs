using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameDevProject.Game.Klas_Interfaces
{
      public interface ICharacter
   {
       Level Level { get; }

       Vector2 Position { get; }

       Rectangle BoundingRectangle { get; }

       void LoadContent();


       void Update(GameTime gameTime, KeyboardState keyboardState, GamePadState gamePadState, AccelerometerState accelState, DisplayOrientation orientation);

       void Draw(GameTime gameTime, SpriteBatch spriteBatch);
   }
}

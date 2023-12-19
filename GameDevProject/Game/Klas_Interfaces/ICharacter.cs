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
   }
}

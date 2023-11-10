using Microsoft.Xna.Framework;
using Mono_labo_1.Klas_Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Mono_labo_1.Klas_Move
{
    internal class MovementManager
    {
        public void Move(IMovable movable)
        {
            var direction = movable.InputReader.ReadInput();

            if (movable.InputReader.IsDestinationInput)
            {
                direction -= movable.Position;
                direction.Normalize();  
            }

            var afstand = direction * movable.Speed;
            var toekomstigePositie = movable.Position + afstand;

            movable.Position = toekomstigePositie;

        }
    }
}

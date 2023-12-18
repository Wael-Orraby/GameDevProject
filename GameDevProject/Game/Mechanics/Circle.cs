using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameDevProject.Game.Mechanics
{
    public struct Circle
    {
        /// <summary>
        /// Middelpuntpositie van de cirkel.
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// Straal van de cirkel.
        /// </summary>
        public float Radius;

        /// <summary>
        /// Maakt een nieuwe cirkel aan.
        /// </summary>
        public Circle(Vector2 position, float radius)
        {
            Center = position;
            Radius = radius;
        }

        /// <summary>
        /// Bepaalt of een cirkel een rechthoek doorsnijdt.
        /// </summary>
        /// <returns>Waar als de cirkel en de rechthoek overlappen. Anders onwaar.</returns>
        public bool Intersects(Rectangle rectangle)
        {
            Vector2 v = new Vector2(MathHelper.Clamp(Center.X, rectangle.Left, rectangle.Right),
                                    MathHelper.Clamp(Center.Y, rectangle.Top, rectangle.Bottom));

            Vector2 direction = Center - v;
            float distanceSquared = direction.LengthSquared();

            return distanceSquared > 0 && distanceSquared < Radius * Radius;
        }
    }
}

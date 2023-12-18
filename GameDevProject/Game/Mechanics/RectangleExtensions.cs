using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameDevProject.Game.Mechanics
{
    public static class RectangleExtensions
    {
        /// <summary> 
        /// Berekent de ondertekende diepte van de intersectie tussen twee rechthoeken. 
        /// </summary>
        /// <returns> 
        /// De hoeveelheid overlap tussen twee elkaar snijdende rechthoeken. Deze 
        /// dieptewaarden kunnen negatief zijn afhankelijk van welke zijden de rechthoeken 
        /// snijden. Dit stelt de bellers in staat om de juiste richting te bepalen 
        /// om objecten te duwen om botsingen op te lossen. 
        /// Als de rechthoeken niet snijden, wordt Vector2.Zero geretourneerd. 
        /// </returns> 
        public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            // Bereken half sizes.
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            // Bereken centers.
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            // Bereken huidige en minimum-niet-kruisende afstanden tussen centers.
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            // Als we elkaar helemaal niet kruisen, return (0, 0).
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            // Bereken en retourneer de snijdieptes.
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }

        /// <summary>
        /// Haalt de positie op van het midden van de onderrand van de rechthoek.
        /// </summary>
        public static Vector2 GetBottomCenter(this Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Bottom);
        }
    }
}

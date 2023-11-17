using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer2D
{

    /// deze enum bepaald of een blokje collision moet hebben ofniet
    enum TileCollision
    {
        /// dit is een doorzichtige blokje waar je door heen kunt lopen
        Passable = 0,

        /// door deze tegel kan je niet heen bewegen
        Impassable = 1,

        /// <summary>
        /// je kan hier op staan of via onder doorheen komen
        /// </summary>
        Platform = 2,
    }

    /// <summary>
    /// deze klas bepaald het gedrag van een blokje het gedrag van een blokje
    /// </summary>
    struct Tile
    {
        public Texture2D Texture;
        public TileCollision Collision;

        public const int Width = 40;
        public const int Height = 32;

        public static readonly Vector2 Size = new Vector2(Width, Height);

        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
        }
    }
}

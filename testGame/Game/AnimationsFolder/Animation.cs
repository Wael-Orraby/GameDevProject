using Microsoft.Xna.Framework.Graphics;

namespace GameDevProject.Game.AnimationsFolder
{
    /// <summary>
    /// Stelt een geanimeerde textuur voor.
    /// </summary>
    /// <remarks>
    /// Momenteel gaat deze klasse ervan uit dat elk frame van de animatie
    /// even breed is als dat de animatie hoog is. Het aantal frames in de
    /// animatie wordt hiervan afgeleid.
    /// </remarks>
    public class Animation
    {
        /// <summary>
        /// Alle frames in de animatie gerangschikt horizontaal.
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
        }
        Texture2D texture;

        /// <summary>
        /// Duur van tijd om elk frame te tonen.
        /// </summary>
        public float FrameTime
        {
            get { return frameTime; }
        }
        float frameTime;

        /// <summary>
        /// Wanneer het einde van de animatie is bereikt, moet het
        /// doorgaan met afspelen vanaf het begin?
        /// </summary>
        public bool IsLooping
        {
            get { return isLooping; }
        }
        bool isLooping;

        /// <summary>
        /// Geeft het aantal frames in de animatie weer.
        /// </summary>
        public int FrameCount
        {
            // Assume square frames.
            get { return Texture.Width / FrameHeight; }
        }


        /// <summary>
        /// Geeft de breedte van een frame in de animatie weer.
        /// </summary>
        public int FrameWidth
        {
            // Assume square frames.
            get { return Texture.Height; }
        }

        /// <summary>
        /// Geeft de hoogte van een frame in de animatie weer.
        /// </summary>
        public int FrameHeight
        {
            get { return Texture.Height; }
        }


        /// <summary>
        /// Maakt een nieuwe animatie.
        /// </summary>        
        public Animation(Texture2D texture, float frameTime, bool isLooping)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
        }
    }

}
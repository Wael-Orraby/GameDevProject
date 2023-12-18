using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameDevProject.Game.AnimationsFolder
{
    /// <summary>
    /// Bestuurt de weergave van een animatie.
    /// </summary>
    public struct AnimationPlayer
    {
        /// <summary>
        /// Geeft de animatie weer die momenteel wordt afgespeeld.
        /// </summary>
        public Animation Animation
        {
            get { return animation; }
        }
        Animation animation;

        /// <summary>
        /// Geeft de index van het huidige frame in de animatie weer.
        /// </summary>
        public int FrameIndex
        {
            get { return frameIndex; }
        }
        int frameIndex;

        /// <summary>
        /// De hoeveelheid tijd in seconden dat het huidige frame is getoond.
        /// </summary>
        private float time;

        /// <summary>
        /// Geeft een textuur-oorsprong aan de onderkant van elk frame.
        /// </summary>
        public Vector2 Origin
        {
            get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight); }
        }


        /// <summary>
        /// Start of blijft de weergave van een animatie voortzetten.
        /// </summary>
        public void PlayAnimation(Animation animation)
        {
            // Als deze animatie al wordt afgespeeld, start deze niet opnieuw.
            if (Animation == animation)
                return;

            // Start de nieuwe animatie.
            this.animation = animation;
            frameIndex = 0;
            time = 0.0f;
        }

        /// <summary>
        /// Verplaatst de tijdpositie en tekent het huidige frame van de animatie.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            // Verwerk de verstrijkende tijd.
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (time > Animation.FrameTime)
            {
                time -= Animation.FrameTime;

                // Verplaats de frame-index; looping of inklemmen indien nodig.
                if (Animation.IsLooping)
                {
                    frameIndex = (frameIndex + 1) % Animation.FrameCount;
                }
                else
                {
                    frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
                }
            }

            // Bereken de bron rechthoek van het huidige frame.
            Rectangle source = new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);
            // Teken het huidige frame.
            spriteBatch.Draw(Animation.Texture, position, source, Color.White, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
        }
    }
}

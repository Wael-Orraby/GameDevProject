using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Mono_labo_1.Klas_Frames
{
    internal class Animation
    {
        // FIELDS
        private List<AnimationFrame> frames;
        private int counter;
        private double secondCounter = 0;

        // PROPERTIES
        public AnimationFrame CurrentFrame { get; private set; }

        // CONSTRUCTOR
        public Animation()
        {
            frames = new List<AnimationFrame>();
            counter = 0;
        }

        // METHODS
        public void Update(GameTime gameTime)
        {
            if (frames.Count == 0)
                return;

            CurrentFrame = frames[counter];

            secondCounter += gameTime.ElapsedGameTime.TotalSeconds;
            int fps = 15;

            if (secondCounter >= 1d / fps)
            {
                counter++;
                secondCounter = 0;
            }

            if (counter >= frames.Count)
            {
                counter = 0;
            }
        }

        public void SetAnimationFrames(List<Rectangle> frameRectangles)
        {
            frames.Clear(); // Clear all frames
            foreach (var rectangle in frameRectangles)
            {
                frames.Add(new AnimationFrame(rectangle));
            }
            CurrentFrame = frames.Count > 0 ? frames[0] : null; // Set the current frame to the first frame
        }

        public void GetAnimationFromTextureRow(int row, int rowSize, int frameWidth, int frameHeight, int yOffset)
        {
            frames.Clear();

            for (int i = 0; i < rowSize; i++)
            {
                Rectangle frameRectangle = new Rectangle(frameWidth * i, frameHeight * (row - 1) + yOffset, frameWidth, frameHeight - yOffset);
                frames.Add(new AnimationFrame(frameRectangle));
            }

            CurrentFrame = frames.Count > 0 ? frames[0] : null; // Set the current frame to the first frame
        }


        public void GetFramesFromTextureProperties(int width, int height, int numberOfWidthSprites, int numberOfHeightSprites)
        {
            frames.Clear();

            int widthOfFrame = width / numberOfWidthSprites;
            int heightOfFrame = height / numberOfHeightSprites;

            for (int y = 0; y <= height - heightOfFrame; y += heightOfFrame)
            {
                for (int x = 0; x <= width - widthOfFrame; x += widthOfFrame)
                {
                    frames.Add(new AnimationFrame(new Rectangle(x, y, widthOfFrame, heightOfFrame)));
                }
            }

            CurrentFrame = frames.Count > 0 ? frames[0] : null; // Set the current frame to the first frame
        }

        public void AddFrame(AnimationFrame frame)
        {
            frames.Add(frame);
            CurrentFrame = frames.Count > 0 ? frames[0] : null; // Set the current frame to the first frame
        }

        public void SetIdleAnimation(int row, int rowSize)
        {
            List<Rectangle> idleFrames = new List<Rectangle>();

            for (int i = 0; i < rowSize; i++)
            {
                idleFrames.Add(new Rectangle(64 * i, 64 * (row - 1), 64, 64));
            }

            SetAnimationFrames(idleFrames); // Set the frames for the idle animation
        }
    }
}

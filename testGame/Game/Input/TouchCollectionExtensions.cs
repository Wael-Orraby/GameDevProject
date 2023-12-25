using Microsoft.Xna.Framework.Input.Touch;

namespace GameDevProject.Game.Input
{
    public static class TouchCollectionExtensions
    {
        /// bepaald of er contact is met het scherm
        /// returnt TRUE als er contact is tijdens het bewegen en returnt false als er geen contact is
        public static bool AnyTouch(this TouchCollection touchState)
        {
            foreach (TouchLocation location in touchState)
            {
                if (location.State == TouchLocationState.Pressed || location.State == TouchLocationState.Moved)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
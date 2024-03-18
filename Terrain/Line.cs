using Microsoft.Xna.Framework;

namespace LunarLander.Terrain
{
    internal class Line
    {
        public Vector2 startingPosition;
        public Vector2 endingPosition;
        public bool is_safe_zone;

        public Line(Vector2 startingPosition, Vector2 endingPosition, bool is_safe_zone)
        {
            this.startingPosition = startingPosition;
            this.endingPosition = endingPosition;
            this.is_safe_zone = is_safe_zone;
        }
    }
}

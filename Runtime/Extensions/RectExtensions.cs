namespace Craiel.UnityEssentials.Runtime.Extensions
{
    using UnityEngine;

    public static class RectExtensions
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Rect Grow(this Rect source, float by)
        {
            return source.Grow(new Vector2(by, by));
        }
        
        public static Rect Grow(this Rect source, Vector2 by)
        {
            Vector2 min = source.min - by;
            Vector2 max = source.max + by;
            return MinMaxRect(min, max);
        }

        public static Rect MinMaxRect(Vector2 min, Vector2 max)
        {
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        public static Rect Shrink(this Rect source, float by)
        {
            return source.Shrink(new Vector2(by, by));
        }

        public static Rect Shrink(this Rect source, Vector2 by)
        {
            Vector2 min = source.min + by;
            Vector2 max = source.max - by;

            if (max.x < min.x)
            {
                max.x = min.x;
            }

            if (max.y < min.y)
            {
                max.y = min.y;
            }
            
            return MinMaxRect(min, max);
        }
    }
}
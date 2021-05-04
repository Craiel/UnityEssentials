// ReSharper disable UnusedMember.Global
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
        
        public static Rect Grow(this Rect source, Rect by)
        {
            if (by.xMin < source.xMin)
            {
                source.xMin = by.xMin;
            }

            if (by.xMax > source.xMax)
            {
                source.xMax = by.xMax;
            }

            if (by.yMin < source.yMin)
            {
                source.yMin = by.yMin;
            }

            if (by.yMax > source.yMax)
            {
                source.yMax = by.yMax;
            }
            
            return source;
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
        
        public static Rect Fit(this Rect source, float width, float height, bool shrinkOnly = true)
        {
            if (shrinkOnly && source.width <= width && source.height <= height)
            {
                return source;
            }
            
            float widthPercent = width / source.width;
            float heightPercent = height / source.height;
            float fitPercent = widthPercent < heightPercent ? widthPercent : heightPercent;
            source.width *= fitPercent;
            source.height *= fitPercent;
            
            return source;
        }
        
        public static bool Includes(this Rect source, Rect target, bool includesFully = true)
        {
            if (includesFully)
            {
                return source.xMin <= target.xMin 
                       && source.xMax > target.xMax 
                       && source.yMin <= target.yMin 
                       && source.yMax >= target.yMax;
            }
            
            return target.xMax > source.xMin 
                   && target.xMin < source.xMax 
                   && target.yMax > source.yMin 
                   && target.yMin < source.yMax;
        }
        
        public static Rect ResetXY(this Rect source)
        {
            source.x = source.y = 0;
            return source;
        }
        
        public static Rect Shift(this Rect source, float x, float y, float width, float height)
        {
            return new Rect(source.x + x, source.y + y, source.width + width, source.height + height);
        }
        
        public static Rect SetX(this Rect source, float value)
        {
            source.x = value;
            return source;
        }

        public static Rect SetY(this Rect source, float value)
        {
            source.y = value;
            return source;
        }
        
        public static Rect SetHeight(this Rect source, float value)
        {
            source.height = value;
            return source;
        }
        
        public static Rect SetWidth(this Rect source, float value)
        {
            source.width = value;
            return source;
        }
    }
}
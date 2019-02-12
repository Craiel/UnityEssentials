namespace Craiel.UnityEssentials.Runtime.Extensions
{
    using UnityEngine;
    using Utils;

    public static class ColorExtensions
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static float[] ToArray(this Color source)
        {
            return new[] {source.r, source.g, source.b, source.a };
        }

        public static Color FromArray(float[] source)
        {
            return new Color(source[0], source[1], source[2], source[3]);
        }

        public static Color Brighten(this Color color, float factor)
        {
            return new Color((color.r * factor).Clamp(0, 1f),
                (color.g * factor).Clamp(0, 1f),
                (color.b * factor).Clamp(0, 1f));
        }
        
        public static Color Darken(this Color color, float by)
        {
            return new Color((color.r / by).Clamp(0, 1f), 
                (color.g / by).Clamp(0, 1f), 
                (color.b / by).Clamp(0, 1f));
        }

        // Note: this is not a proper way to de-saturate, look at HSV implementations for proper ways
        public static Color DesaturateSimple(this Color color, float by)
        {
            float l = (float)(0.3 * color.r + 0.6 * color.g + 0.1 * color.b);
            float r = color.r + by * (l - color.r);
            float g = color.g + by * (l - color.g);
            float b = color.b + by * (l - color.b);
            return new Color(r, g, b, color.a);
        }
    }
}

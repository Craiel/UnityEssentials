namespace Craiel.UnityEssentials.Extensions
{
    using UnityEngine;

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
    }
}

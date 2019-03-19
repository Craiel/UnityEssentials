// ReSharper disable UnusedMember.Global
namespace Craiel.UnityEssentials.Runtime.Extensions
{
    using System;
    using System.Globalization;
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

        public static Color FromRGB(byte r, byte g, byte b)
        {
            float maxValue = byte.MaxValue;
            return new Color(r / maxValue, g / maxValue, b / maxValue);
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
        
        public static Color SetAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
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
        
        public static string ToHexString(this Color32 color)
        {
            return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
        }

        public static Color32 ToColor(this string colorHexString)
        {
            int number = int.Parse(colorHexString, NumberStyles.HexNumber);

            Color32 result;

            switch (colorHexString.Length)
            {
                case 8:
                {
                    return new Color32((byte)(number >> 24 & 255), (byte)(number >> 16 & 255), (byte)(number >> 8 & 255), (byte)(number & 255));
                }

                case 6:
                {
                    return new Color32((byte)(number >> 16 & 255), (byte)(number >> 8 & 255), (byte)(number & 255), 255);
                }

                case 4:
                {
                    return new Color32((byte)((number >> 12 & 15) * 17), (byte)((number >> 8 & 15) * 17), (byte)((number >> 4 & 15) * 17), (byte)((number & 15) * 17));
                }

                case 3:
                {
                    return new Color32((byte)((number >> 8 & 15) * 17), (byte)((number >> 4 & 15) * 17), (byte)((number & 15) * 17), 255);
                }

                default:
                {
                    throw new FormatException("Support only RRGGBBAA, RRGGBB, RGBA, RGB formats");
                }
            }
        }
    }
}

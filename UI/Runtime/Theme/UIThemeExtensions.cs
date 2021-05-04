namespace Craiel.UnityEssentialsUI.Runtime.Theme
{
    using Enums;
    using UnityEngine;

    public static class UIThemeExtensions
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Color GetColorValue(this UITheme theme, ThemeColorType colorType)
        {
            switch (colorType)
            {
                case ThemeColorType.Primary:
                {
                    return theme.PrimaryColor;
                }
                
                case ThemeColorType.Secondary:
                {
                    return  theme.SecondaryColor;
                }
                
                case ThemeColorType.Accent1:
                {
                    return  theme.Accent1Color;
                }
                
                case ThemeColorType.Accent2:
                {
                    return  theme.Accent2Color;
                }
                
                case ThemeColorType.FontDefault:
                {
                    return  theme.FontColorDefault;
                }

                default:
                {
                    return Color.white;
                }
            }
        }
    }
}
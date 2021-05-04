namespace Craiel.UnityEssentials.Runtime.I18N
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Enums;
    using Resource;

    public static class LocaleConstants
    {
        private static readonly IDictionary<CultureInfo, ResourceKey> LocalizationMasterFiles = new Dictionary<CultureInfo, ResourceKey>();

        private static readonly ResourceKey LocalizationFallbackMasterFile;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static readonly CultureInfo LocaleEnglish = new CultureInfo("en-US");
        public static readonly CultureInfo LocaleFrench = new CultureInfo("fr-FR");
        public static readonly CultureInfo LocaleGerman = new CultureInfo("de-DE");
        public static readonly CultureInfo LocaleSpanish = new CultureInfo("es-ES");
        public static readonly CultureInfo LocalePortuguese = new CultureInfo("pt-PT");
        public static readonly CultureInfo LocaleRussian = new CultureInfo("ru-RU");
        
        public static ResourceKey GetLocalizationMasterFile(CultureInfo culture)
        {
            ResourceKey key;
            if(LocalizationMasterFiles.TryGetValue(culture, out key))
            {
                return key;
            }

            if (LocalizationFallbackMasterFile == ResourceKey.Invalid)
            {
                throw new InvalidOperationException();
            }

            return LocalizationFallbackMasterFile;
        }
        
        public static readonly CultureInfo[] AllCultures = {
            LocaleEnglish,
            LocaleFrench, 
            LocaleGerman, 
            LocaleSpanish,
            LocalePortuguese,
            LocaleRussian
        };
        
        public static CultureInfo GetCulture(GameLanguage language)
        {
            switch (language)
            {
                case GameLanguage.Eng:
                {
                    return LocaleEnglish;
                }

                case GameLanguage.Fra:
                {
                    return LocaleFrench;
                }

                case GameLanguage.Deu:
                {
                    return LocaleGerman;
                }

                case GameLanguage.Esp:
                {
                    return LocaleSpanish;
                }

                case GameLanguage.Por:
                {
                    return LocalePortuguese;
                }

                case GameLanguage.Rus:
                {
                    return LocaleRussian;
                }

                default:
                {
                    return LocaleEnglish;
                }
            }
        }
    }
}

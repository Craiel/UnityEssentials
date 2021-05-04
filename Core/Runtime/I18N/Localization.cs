namespace Craiel.UnityEssentials.Runtime.I18N
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using IO;
    using UnityEngine;

    public static class Localization
    {
        private static readonly HashSet<LocalizationToken> ActiveTokens = new HashSet<LocalizationToken>();

        private static CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

        private static ILocalizationProvider provider;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static CultureInfo CurrentCulture
        {
            get
            {
                return currentCulture;
            }

            set
            {
                currentCulture = value;
                if (!Thread.CurrentThread.CurrentCulture.Equals(currentCulture))
                {
                    Thread.CurrentThread.CurrentCulture = currentCulture;
                    Thread.CurrentThread.CurrentUICulture = currentCulture;
                }
            }
        }

        public static void Initialize(ILocalizationProvider newProvider)
        {
            provider = newProvider;
        }

        public static LocalizationToken CreateToken(string key)
        {
            var result = new LocalizationToken(key);
            if (ActiveTokens.Contains(result))
            {
                EssentialsCore.Logger.Warn("Duplicate Localization token: {0}", key);
            }
            else
            {
                ActiveTokens.Add(result);
            }

            return result;
        }

        public static string Get(this LocalizationToken token)
        {
            if (provider == null || string.IsNullOrEmpty(token.Key))
            {
                return token.Key;
            }

            return provider.GetString(token.Key);
        }

        public static string GetCaps(this LocalizationToken token)
        {
            if (provider == null || string.IsNullOrEmpty(token.Key))
            {
                return token.Key?.ToUpper();
            }

            return provider.GetString(token.Key).ToUpper(currentCulture);
        }

        public static string GetFormatted(this LocalizationToken token, params object[] formatParams)
        {
            if (provider == null)
            {
                return string.Format(token.Key, formatParams);
            }

            return string.Format(provider.GetString(token.Key), formatParams);
        }

        public static string GetDirect(string source)
        {
            if (provider == null || string.IsNullOrEmpty(source))
            {
                return source;
            }

            return provider.GetString(source);
        }

        public static string GetEnumTranslation<T>(T value)
            where T : struct, IComparable
        {
            // TODO: handle enums properly
            return GetDirect(value.ToString());
        }
    }
}

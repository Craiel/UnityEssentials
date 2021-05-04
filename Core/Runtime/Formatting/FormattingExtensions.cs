namespace Craiel.UnityEssentials.Runtime.Formatting
{
    using System;
    using System.Globalization;
    using Enums;
    using UnityEngine;

    public static class FormattingExtensions
    {
        // ReSharper disable StringLiteralTypo
        private static readonly NumberDefinition[] NumberDefinitions =
        {
            new NumberDefinition("Million", "M", 6),
            new NumberDefinition("Billion", "B", 9),
            new NumberDefinition("Trillion", "T", 12),
            new NumberDefinition("Quadrillion", "Qa", 15),
            new NumberDefinition("Quintillion", "Qi", 18),
            new NumberDefinition("Sextillion", "Sx", 21),
            new NumberDefinition("Septillion", "Sp", 24),
            new NumberDefinition("Octillion", "Oc", 27),
            new NumberDefinition("Nonillion", "No", 30),
            new NumberDefinition("Decillion", "Dc", 33),
            new NumberDefinition("Undecillion", "Ud", 36),
            new NumberDefinition("Duodecillion", "Dd", 39),
            new NumberDefinition("Tredecillion", "Td", 42),
            new NumberDefinition("Quattuordecillion", "Qad", 45),
            new NumberDefinition("Quinquadecillion", "Qid", 48),
            new NumberDefinition("Sedecillion", "Sxd", 51),
            new NumberDefinition("Septendecillion", "Spd", 54),
            new NumberDefinition("Octodecillion", "Ocd", 57),
            new NumberDefinition("Novendecillion", "Nod", 60),
            new NumberDefinition("Vigintillion", "Vg", 63),
            new NumberDefinition("Unvigintillion", "Uvg", 66),
            new NumberDefinition("Duovigintillion", "Dvg", 69),
            new NumberDefinition("Tresvigintillion", "Tvg", 72),
            new NumberDefinition("Quattuorvigintillion", "Qavg", 75),
            new NumberDefinition("Quinquavigintillion", "Qivg", 78),
            new NumberDefinition("Sesvigintillion", "Sxvg", 81),
            new NumberDefinition("Septemvigintillion", "Spvg", 84),
            new NumberDefinition("Octovigintillion", "Ocvg", 87),
            new NumberDefinition("Novemvigintillion", "Novg", 90),
            new NumberDefinition("Trigintillion", "Tg", 93),
            new NumberDefinition("Untrigintillion", "Utg", 96),
            new NumberDefinition("Duotrigintillion", "Dtg", 99),
            new NumberDefinition("Trestrigintillion", "Ttg", 102),
            new NumberDefinition("Quattuortrigintillion", "Qatg", 105),
            new NumberDefinition("Quinquatrigintillion", "Qitg", 108),
            new NumberDefinition("Sestrigintillion", "Sxtg", 111),
            new NumberDefinition("Septentrigintillion", "Sptg", 114),
            new NumberDefinition("Octotrigintillion", "Octg", 117),
            new NumberDefinition("Noventrigintillion", "Notg", 120),
            new NumberDefinition("Quadragintillion", "G", 123),
            new NumberDefinition("Unquadragintillion", "Ug", 126),
            new NumberDefinition("Duoquadragintillion", "Dg", 129),
            new NumberDefinition("Tresquadragintillion", "Tg", 132),
            new NumberDefinition("Quattorquadragintillion", "Qag", 135),
            new NumberDefinition("Quinquaquadragintillion", "Qig", 138),
            new NumberDefinition("Quinquaquadragintillion", "Qig", 141)
        };
        // ReSharper enable StringLiteralTypo
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static string DefaultFormatPrefix;
        
        public static string Format(this float value, byte decimalDigits = 0, NumberFormattingType type = NumberFormattingType.ShortName, string customPrefix = null)
        {
            return ((double) value).Format(decimalDigits, type);
        }
        
        public static string Format(this double value, byte decimalDigits = 0, NumberFormattingType type = NumberFormattingType.ShortName, string customPrefix = null)
        {
            string prefix = customPrefix ?? DefaultFormatPrefix ?? string.Empty;
            
            NumberDefinition? definition = null;
            if (type != NumberFormattingType.Raw)
            {
                definition = GetNumberDefintion(value);
            }

            if (decimalDigits != byte.MaxValue)
            {
                value = Math.Round(value, decimalDigits);
            }

            if (value < NumberDefinitions[0].value)
            {
                return string.Format("{0}{1:#,##.##}", prefix, value);
            }

            if (definition != null)
            {
                value = value / definition.Value.value;
                if (value >= 100)
                {
                    value = Math.Round(value, 1);
                }
                else if (value >= 10)
                {
                    value = Math.Round(value, 2);
                }
                else
                {
                    value = Math.Round(value, 3);
                }
            }

            if (definition == null)
            {
                return string.Format("{0}{1}", prefix, value);
            }

            switch (type)
            {
                case NumberFormattingType.ShortName:
                {
                    return string.Format("{0}{1} {2}", prefix, value, definition.Value.ShortName);
                }

                case NumberFormattingType.FullName:
                {
                    return string.Format("{0}{1} {2}", prefix, value, definition.Value.FullName);
                }

                default:
                {
                    return prefix + value.ToString(CultureInfo.InvariantCulture);
                }
            }
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static NumberDefinition? GetNumberDefintion(double value)
        {
            uint exponents = (uint)Math.Floor(Math.Log10(value));
            if (exponents >= 6)
            {
                uint formatIndex = (uint)Mathf.Floor((exponents - 6) / 3f);
                if (formatIndex >= NumberDefinitions.Length)
                {
                    return NumberDefinitions[NumberDefinitions.Length - 1];
                }
                
                return NumberDefinitions[formatIndex];
            }

            return null;
        }
        
        private struct NumberDefinition
        {
            public NumberDefinition(string name, string shortName, ushort exponent)
            {
                this.FullName = name;
                this.ShortName = shortName;
                this.value = Math.Pow(10, exponent);
            }
            
            public readonly string FullName;
            public readonly string ShortName;
            public readonly double value;
        }
    }
}

namespace Craiel.UnityEssentials.Runtime.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>
    /// Math utilities
    /// </summary>
    public static class EssentialMathUtils
    {
        // Largest and lowest safe numbers to do comparisons on, c# gets inaccurate after
        public const float MinFloat = 1 - (1 << 24);
        public const float MaxFloat = (1 << 24) - 1;
        public const double MaxDouble = 9007199254740991;
        public const double MinDouble = 1 - MaxDouble;

        public const double GoldenRatio = 1.6180339887498948482;
        public const double PlasticConstant = 1.32471795724474602596;
        public const double Tau = 6.283185307179586;
        
        public const double PlanckLength = 1.61622938 * (10 * -35);
        public const double PlanckTime = 5.3911613 * (10 ^ -44);
        public const double PlanckTemperature = 1.41680833 * (10 ^ 32);
        public const double PlanckMass = 4.341 * (10 ^ -9);
        public const double PlancksConstant = 6.62 * (10 ^ -34);

        public const double GravitationalConstant = 6.6740831 * (10 ^ -11);
        
        public const int SpeedOfLight = 299792458;
        
        public const long LightYearInMeters = 9460730472580800;
        public const double LightYearInKilometers = LightYearInMeters / 1000f;

        public static readonly double PiOver2 = Math.PI / 2;
        public static readonly double TwoPi = Math.PI * 2;

        public static float Epsilon;

        public static double DoubleEpsilon;

        private static double nextNextGaussian;
        private static bool haveNextGaussian;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static EssentialMathUtils()
        {
            UpdateEpsilon();
            UpdateDoubleEpsilon();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        /// <summary>
        /// Recalculates the epsilon value for floats
        /// See https://www.johndcook.com/blog/2010/06/08/c-math-gotchas/ for the specific reason why this exists
        /// </summary>
        /// <param name="center">the center to calculate for, higher = less precision</param>
        public static void UpdateEpsilon(float center = 1.0f)
        {
            Epsilon = 1f;
            while (Epsilon + center > center)
            {
                Epsilon /= 2f;
            }
        }
        
        /// <summary>
        /// Recalculates the epsilon value for floats
        /// See https://www.johndcook.com/blog/2010/06/08/c-math-gotchas/ for the specific reason why this exists
        /// </summary>
        /// <param name="center">the center to calculate for, higher = less precision</param>
        public static void UpdateDoubleEpsilon(double center = 1.0d)
        {
            DoubleEpsilon = 1d;
            while (DoubleEpsilon + center > center)
            {
                DoubleEpsilon /= 2d;
            }
        }

        /// <summary>
        /// <para>Returns the next pseudorandom, Gaussian ("normally") distributed double value with mean 0.0 and standard deviation 1.0 from this random number generator's sequence.</para>
        /// <para>The general contract of <see cref="NextGaussian"/> is that one double value, chosen from(approximately) the usual
        /// normal distribution with mean 0.0 and standard deviation 1.0, is pseudorandomly generated and returned.</para>
        /// <para>The method <see cref="NextGaussian"/> is implemented by class <see cref="System.Random"/> as if by a threadsafe version of the following:
        /// <code>
        /// public double nextGaussian()
        /// {
        ///     if (haveNextNextGaussian)
        ///     {
        ///         haveNextNextGaussian = false;
        ///         return nextNextGaussian;
        ///     }
        /// 
        ///     double v1, v2, s;
        ///     do
        ///     {
        ///         v1 = 2 * nextDouble() - 1;   // between -1.0 and 1.0
        ///         v2 = 2 * nextDouble() - 1;   // between -1.0 and 1.0
        ///         s = v1 * v1 + v2 * v2;
        ///      } while (s >= 1 || s == 0);
        /// 
        ///      double multiplier = StrictMath.sqrt(-2 * StrictMath.log(s) / s);
        ///      nextNextGaussian = v2 * multiplier;
        ///      haveNextNextGaussian = true;
        ///      return v1 * multiplier;
        /// }
        /// </code></para>
        ///  <para>This uses the<i>polar method</i> of G. E.P.Box, M.E.Muller, and
        ///  G. Marsaglia, as described by Donald E. Knuth in <i>The Art of
        ///  Computer Programming</i>, Volume 3: <i>Seminumerical Algorithms</i>,
        ///  section 3.4.1, subsection C, algorithm P. Note that it generates two
        ///  independent values at the cost of only one call to <see cref="Math.Log"/>
        ///  and one call to <see cref="Math.Sqrt"/>.</para>
        /// </summary>
        /// <returns>the next pseudorandom, Gaussian("normally") distributed double value with mean 0.0 and standard deviation 1.0 from this random number generator's sequence</returns>
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static double NextGaussian()
        {
            // See Knuth, ACP, Section 3.4.1 Algorithm C.
            if (haveNextGaussian)
            {
                haveNextGaussian = false;
                return nextNextGaussian;
            }

            double v1, v2, s;
            do
            {
                v1 = 2 * UnityEngine.Random.value - 1; // between -1 and 1
                v2 = 2 * UnityEngine.Random.value - 1; // between -1 and 1
                s = v1 * v1 + v2 * v2;
            }
            while (s >= 1 || Math.Abs(s) < DoubleEpsilon);

            double multiplier = Math.Sqrt(-2 * Math.Log(s) / s);
            nextNextGaussian = v2 * multiplier;
            haveNextGaussian = true;
            return v1 * multiplier;
        }

        public static double RandomTriangular(double high)
        {
            return (UnityEngine.Random.value - UnityEngine.Random.value) * high;
        }

        public static double RandomTriangular(double low, double high, double mode)
        {
            double u = UnityEngine.Random.value;
            double d = high - low;
            if (u <= (mode - low) / d)
            {
                return low + Math.Sqrt(u * d * (mode - low));
            }

            return high - Math.Sqrt((1 - u) * d * (high - mode));
        }

        public static float RandomTriangular(float high)
        {
            return (UnityEngine.Random.value - UnityEngine.Random.value) * high;
        }

        public static float RandomTriangular(float low, float high, float mode)
        {
            float u = UnityEngine.Random.value;
            float d = high - low;
            if (u <= (mode - low) / d)
            {
                return low + (float)Math.Sqrt(u * d * (mode - low));
            }

            return high - (float)Math.Sqrt((1 - u) * d * (high - mode));
        }

        /// <summary>
        /// Returns the specified value if the value is already a power of two
        /// </summary>
        /// <param name="value">the value to start at</param>
        /// <returns>the next power of two</returns>
        public static int NextPowerOfTwo(int value)
        {
            if (value == 0)
            {
                return 1;
            }

            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }

        public static int NumberOfTrailingZeros(this int i)
        {
            // HD, Figure 5-14
            int y;
            if (i == 0)
            {
                return 32;
            }

            int n = 31;
            y = i << 16; if (y != 0) { n = n - 16; i = y; }
            y = i << 8; if (y != 0) { n = n - 8; i = y; }
            y = i << 4; if (y != 0) { n = n - 4; i = y; }
            y = i << 2; if (y != 0) { n = n - 2; i = y; }
            return n - ((i << 1) >> 31);
        }

        public static Vector3 WithMaxPrecision(this Vector3 source, int precision)
        {
            return new Vector3((float)Math.Round(source.x, precision), (float)Math.Round(source.y, precision), (float)Math.Round(source.z, precision));
        }

        public static float Max(params float[] values)
        {
            float result = float.MinValue;
            for (var i = 0; i < values.Length; i++)
            {
                if (result < values[i])
                {
                    result = values[i];
                }
            }

            return result;
        }

        public static float DegreesToRadians(float degree)
        {
            return (float)(degree * (Math.PI / 180.0f));
        }

        public static float RadiansToDegrees(float radian)
        {
            return (float)(radian * (180.0f / Math.PI));
        }

        public static int Clamp(this int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static float Clamp(this float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static double Clamp(this double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static float Floor(this float value)
        {
            return (float)Math.Floor(value);
        }

        public static int FloorToInt(this float value)
        {
            return (int)Math.Floor(value);
        }

        public static int FloorToInt(this double value)
        {
            return (int)Math.Floor(value);
        }

        public static long FloorToLong(this double value)
        {
            return (long)Math.Floor(value);
        }

        public static double Floor(this double value)
        {
            return Math.Floor(value);
        }

        public static IList<int> ComputePrimes(int max)
        {
            var primes = new bool[max + 1];
            var sqrt = (int)Math.Sqrt(max);
            for (int x = 1; x < sqrt; x++)
            {
                var squareX = x * x;
                for (int y = 1; y <= sqrt; y++)
                {
                    var squareY = y * y;
                    var n = (4 * squareX) + squareY;
                    if (n <= max && (n % 12 == 1 || n % 12 == 5))
                    {
                        primes[n] ^= true;
                    }

                    n = (3 * squareX) + squareY;
                    if (n <= max && n % 12 == 7)
                    {
                        primes[n] ^= true;
                    }

                    n = (3 * squareX) - squareY;
                    if (x > y && n <= max && n % 12 == 11)
                    {
                        primes[n] ^= true;
                    }
                }
            }

            var primeList = new List<int> { 2, 3 };
            for (int i = 5; i <= sqrt; i++)
            {
                if (primes[i])
                {
                    primeList.Add(i);
                    int square = i * i;
                    for (int k = square; k < max; k += square)
                    {
                        primes[k] = false;
                    }
                }
            }

            for (int i = sqrt + 1; i <= max; i++)
            {
                if (primes[i])
                {
                    primeList.Add(i);
                }
            }

            return primeList;
        }

        public static bool IsMultipleOf(this int x, int n)
        {
            return (x % n) == 0;
        }

        public static bool IsMultipleOf(this uint x, uint n)
        {
            return (x % n) == 0;
        }

        public static double GetExponentialResultWithin(float currentAmount, float cost, float exponent, uint currentLevel)
        {
            double t1 = currentAmount * (exponent - 1);
            double t2 = (float)(cost * Math.Pow(exponent, currentLevel));
            double newBase = (t1 / t2) + 1;
            return Math.Floor(Math.Log(newBase, exponent));
        }

        public static double GetExponentialCost(uint currentValue, uint desiredAddition, float cost, float exponent)
        {
            double t1 = Math.Pow(exponent, currentValue) *
                        (Math.Pow(exponent, desiredAddition) - 1);
            double t2 = exponent - 1;
            return cost * (t1 / t2);
        }
        
        public static bool Approximately(this Vector3 v1, Vector3 v2)
        {
            return Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y) && Mathf.Approximately(v1.z, v2.z);
        }
    }
}

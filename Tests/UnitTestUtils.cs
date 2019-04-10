namespace Craiel.UnityEssentials.Tests
{
    using System;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using NUnit.Framework;

    public static class UnitTestUtils
    {
        private const int PerformanceBaselineTests = 10000;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [StringFormatMethod("text")]
        public static void Log(string text, params object[] formatParams)
        {
            if (formatParams == null || formatParams.Length == 0)
            {
                TestContext.WriteLine(text);
                return;
            }
            
            TestContext.WriteLine(string.Format(text, formatParams));
        }

        public static float GetPerformanceBaseline()
        {
            var rnd = new Random();
            double test = 0;
            var timer = new Stopwatch();
            timer.Reset();
            timer.Start();
            for (var i = 0; i < PerformanceBaselineTests; i++)
            {
                unchecked
                {
                    test = test * rnd.NextDouble() + rnd.Next() % rnd.NextDouble() / 2;                    
                }
            }
            
            timer.Stop();
            return 1 / ((float)timer.ElapsedTicks / PerformanceBaselineTests);
        }
    }
}
namespace Craiel.UnityEssentials.Tests
{
    using JetBrains.Annotations;
    using NUnit.Framework;

    public static class UnitTestUtils
    {        
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
    }
}
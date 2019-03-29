namespace Craiel.UnityEssentials.Tests
{
    using System;
    using NUnit.Framework;
    using Runtime;

    public static class FNVTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public static void Run()
        {
            var provider = FNV.Create();

            const int value1 = 9912355;
            const int value2 = 12351692;
            
            byte[] data = BitConverter.GetBytes(value1);
            byte[] data2 = BitConverter.GetBytes(value2);

            int hash1 = provider.Compute(data);
            Assert.AreEqual(hash1, provider.Compute(data));
            
            UnitTestUtils.Log("Hash1: {0} -> {1}", value1, hash1);
            
            int hash2 = provider.Compute(data2);
            Assert.AreEqual(hash2, provider.Compute(data2));
            
            UnitTestUtils.Log("Hash2: {0} -> {1}", value2, hash2);
            
            Assert.AreNotEqual(hash1, hash2);
        }
    }
}
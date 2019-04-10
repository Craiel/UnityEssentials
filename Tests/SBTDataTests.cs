namespace Craiel.UnityEssentials.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using NUnit.Framework;
    using Runtime.Data.SBT;
    using Runtime.Data.SBT.Nodes;
    using Runtime.Enums;

    public static class SBTDataTests
    {
        private const string TestString = "TestStr123";
        private const byte TestByte = 25;
        private const short TestShort = -50;
        private const ushort TestUShort = 99;
        private const int TestInt = -523;
        private const uint TestUInt = 9125;
        private const long TestLong = -3151613516616161263;
        private const ulong TestULong = 9315125161642661862;
        private const float TestFloat = 5124.5152f;
        private const double TestDouble = 4141259683476126434361948561251345665.145f;
        
        private static readonly int[] TestArray = new[] {5, 15, 23, 591, int.MaxValue};

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        
        [Test]
        public static void SimpleTypeListTest()
        {
            var lData = new SBTList();
            FillTestList(lData);
            
            byte[] lbData = lData.Serialize();
            byte[] lbcData = lData.SerializeCompressed();
            
            // Note: Compression with so little data actually produces overhead
            Assert.AreEqual(87, lbData.Length);
            Assert.AreEqual(106, lbcData.Length);
            
            // Deserialize
            var lDataOut = SBTList.Deserialize(lbData);
            var lcDataOut = SBTList.DeserializeCompressed(lbcData);
            
            Assert.AreEqual(lDataOut.Count, lcDataOut.Count);
            
            // Read each type
            Assert.AreEqual(10, lDataOut.Count);
            Assert.AreEqual(TestString, lDataOut.ReadString(0));
            Assert.AreEqual(TestByte, lDataOut.ReadByte(1));
            Assert.AreEqual(TestShort, lDataOut.ReadShort(2));
            Assert.AreEqual(TestUShort, lDataOut.ReadUShort(3));
            Assert.AreEqual(TestInt, lDataOut.ReadInt(4));
            Assert.AreEqual(TestUInt, lDataOut.ReadUInt(5));
            Assert.AreEqual(TestLong, lDataOut.ReadLong(6));
            Assert.AreEqual(TestULong, lDataOut.ReadULong(7));
            Assert.AreEqual(TestFloat, lDataOut.ReadSingle(8));
            Assert.AreEqual(TestDouble, lDataOut.ReadDouble(9));
            
            Assert.AreEqual(SBTFlags.Debug, lDataOut.ReadFlags(0));
            Assert.AreEqual(SBTFlags.None, lDataOut.ReadFlags(1));
            Assert.AreEqual(SBTFlags.None, lDataOut.ReadFlags(5));
            Assert.AreEqual(SBTFlags.Debug, lDataOut.ReadFlags(7));
        }

        [Test]
        public static void SimpleTypeDictionaryTest()
        {
            var lData = new SBTDictionary();
            FillTestDictionary(lData);
            
            byte[] lbData = lData.Serialize();
            byte[] lbcData = lData.SerializeCompressed();
            
            // Note: Compression with so little data actually produces overhead
            Assert.AreEqual(139, lbData.Length);
            Assert.AreEqual(148, lbcData.Length);
            
            // Deserialize
            var lDataOut = SBTDictionary.Deserialize(lbData);
            var lcDataOut = SBTDictionary.DeserializeCompressed(lbcData);
            
            Assert.AreEqual(lDataOut.Count, lcDataOut.Count);
            
            // Read each type
            Assert.AreEqual(10, lDataOut.Count);
            Assert.AreEqual(TestString, lDataOut.ReadString("Str"));
            Assert.AreEqual(TestByte, lDataOut.ReadByte("Byt"));
            Assert.AreEqual(TestShort, lDataOut.ReadShort("Short"));
            Assert.AreEqual(TestUShort, lDataOut.ReadUShort("UShort"));
            Assert.AreEqual(TestInt, lDataOut.ReadInt("I"));
            Assert.AreEqual(TestUInt, lDataOut.ReadUInt("UI"));
            Assert.AreEqual(TestLong, lDataOut.ReadLong("VeryVeryLongKey"));
            Assert.AreEqual(TestULong, lDataOut.ReadULong("ULong"));
            Assert.AreEqual(TestFloat, lDataOut.ReadSingle("S"));
            Assert.AreEqual(TestDouble, lDataOut.ReadDouble("D"));
            
            Assert.AreEqual(SBTFlags.Debug, lDataOut.ReadFlags("VeryVeryLongKey"));
            Assert.AreEqual(SBTFlags.None, lDataOut.ReadFlags("ULong"));
            
            // Test with case mis-match
            Assert.Throws<KeyNotFoundException>(() => lDataOut.ReadDouble("d"));

            // Try read
            Assert.AreEqual(TestULong, lDataOut.TryRead<SBTNodeULong>("ULong").Data);
            
            // Contains
            Assert.IsTrue(lDataOut.Contains("VeryVeryLongKey"));
            Assert.IsFalse(lDataOut.Contains("VeryveryLongKey"));
        }

        [Test]
        public static void MassDataTest()
        {
            var lData = new SBTList();
            var dData = new SBTDictionary();
            for (var i = 0; i < 1000; i++)
            {
                lData.Add(TestInt);
                lData.Add(TestDouble);
                
                dData.Add("IntValue" + i, TestShort);
                dData.Add("Double" + i, TestULong);
                dData.Add("Key" + i, "ValueString");
            }

            byte[] lbData = lData.Serialize();
            byte[] lbcData = lData.SerializeCompressed();
            byte[] dbData = dData.Serialize();
            byte[] dbcData = dData.SerializeCompressed();
            
            Assert.AreEqual(18005, lbData.Length);
            Assert.AreEqual(106, lbcData.Length);
            Assert.AreEqual(59675, dbData.Length);
            Assert.AreEqual(7738, dbcData.Length);
            
            var lDataOut = SBTList.Deserialize(lbData);
            var lcDataOut = SBTList.DeserializeCompressed(lbcData);
            var dDataOut = SBTDictionary.Deserialize(dbData);
            var dcDataOut = SBTDictionary.DeserializeCompressed(dbcData);
            
            Assert.AreEqual(lData.Count, lDataOut.Count);
            Assert.AreEqual(lData.Count, lcDataOut.Count);
            Assert.AreEqual(dData.Count, dDataOut.Count);
            Assert.AreEqual(dData.Count, dcDataOut.Count);
        }

        [Test]
        public static void NestingTest()
        {
            var lData = new SBTList();
            var dData = new SBTDictionary();

            var nestedList = lData.AddList();
            FillTestList(nestedList);
            nestedList = dData.AddList("NestedList");
            FillTestList(nestedList);

            var nestedDictionary = lData.AddDictionary();
            FillTestDictionary(nestedDictionary);
            
            nestedDictionary = dData.AddDictionary("NestedDict");
            FillTestDictionary(nestedDictionary);
            
            var nestedArray = lData.AddArray<int>();
            FillTestArray(nestedArray);
            
            nestedArray = dData.AddArray<int>("NestedIntArray");
            FillTestArray(nestedArray);
            
            byte[] lbData = lData.Serialize();
            byte[] lbcData = lData.SerializeCompressed();
            byte[] dbData = dData.Serialize();
            byte[] dbcData = dData.SerializeCompressed();
            
            Assert.AreEqual(20242, lbData.Length);
            Assert.AreEqual(7252, lbcData.Length);
            Assert.AreEqual(20279, dbData.Length);
            Assert.AreEqual(7285, dbcData.Length);
            
            var lDataOut = SBTList.Deserialize(lbData);
            var lcDataOut = SBTList.DeserializeCompressed(lbcData);
            var dDataOut = SBTDictionary.Deserialize(dbData);
            var dcDataOut = SBTDictionary.DeserializeCompressed(dbcData);
            
            Assert.AreEqual(lData.Count, lDataOut.Count);
            Assert.AreEqual(lData.Count, lcDataOut.Count);
            Assert.AreEqual(dData.Count, dDataOut.Count);
            Assert.AreEqual(dData.Count, dcDataOut.Count);

            var compareList = lDataOut.ReadList(0);
            Assert.AreEqual(nestedList.Count, compareList.Count);
            
            var compareDict = dDataOut.ReadDictionary("NestedDict");
            Assert.AreEqual(nestedDictionary.Count, compareDict.Count);
            
            var compareArray = dDataOut.ReadArray<int>("NestedIntArray");
            Assert.AreEqual(nestedArray.Length, compareArray.Length);
        }
        
        [Test]
        public static void ArrayTest()
        {
            var lData = new SBTList();
            
            var nestedArray = lData.AddArray<int>();
            Assert.AreEqual(2621440, nestedArray.Capacity);
            
            FillTestArray(nestedArray);
            nestedArray.Set(123, 999999);
            
            Assert.AreEqual(5000, nestedArray.Length);

            var nestedUShortArray = lData.AddArray<ushort>();
            Assert.AreEqual(5242880, nestedUShortArray.Capacity);
            
            nestedUShortArray.AddChecked(15);
            nestedUShortArray.AddChecked(515);
            
            byte[] lbData = lData.Serialize();
            byte[] lbcData = lData.SerializeCompressed();
            
            Assert.AreEqual(20031, lbData.Length);
            Assert.AreEqual(7023, lbcData.Length);
            
            var lDataOut = SBTList.Deserialize(lbData);
            var lcDataOut = SBTList.DeserializeCompressed(lbcData);
            
            Assert.AreEqual(lData.Count, lDataOut.Count);
            Assert.AreEqual(lData.Count, lcDataOut.Count);
            
            var compareArray = lDataOut.ReadArray<int>(0);
            Assert.AreEqual(5000, compareArray.Length);
            Assert.AreEqual(1052, compareArray.Read(1052));
            Assert.AreEqual(999999, compareArray.Read(123));
            for (var i = 0; i < 50; i++)
            {
                Assert.AreEqual(i, compareArray.Read(i));
            }
            
            var compareUShortArray = lDataOut.ReadArray<ushort>(1);
            Assert.AreEqual(2, compareUShortArray.Length);
            Assert.AreEqual(515, compareUShortArray.Read(1));
        }

        [Test]
        public static void PerformanceTest()
        {
            var baseline = UnitTestUtils.GetPerformanceBaseline();
            UnitTestUtils.Log("Baseline: {0:N4}", baseline);
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < 100; i++)
            {
                NestingTest();
            }
            
            stopWatch.Stop();
            double elapsed = stopWatch.Elapsed.TotalSeconds;
            UnitTestUtils.Log("Elapsed: {0:N3}, BS: {1:N3}", elapsed, elapsed * baseline);
            Assert.LessOrEqual(elapsed * baseline, 0.6f);
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void FillTestDictionary(SBTNodeDictionary target)
        {
            target.Add("Str", TestString)
                .Add("Byt", TestByte)
                .Add("Short", TestShort)
                .Add("UShort", TestUShort)
                .Add("I", TestInt)
                .Add("UI", TestUInt)
                .Add("VeryVeryLongKey", TestLong, SBTFlags.Debug)
                .Add("ULong", TestULong)
                .Add("S", TestFloat)
                .Add("D", TestDouble);
        }

        private static void FillTestList(SBTNodeList target)
        {
            target.Add(TestString, SBTFlags.Debug)
                .Add(TestByte)
                .Add(TestShort)
                .Add(TestUShort)
                .Add(TestInt)
                .Add(TestUInt)
                .Add(TestLong)
                .Add(TestULong, SBTFlags.Debug)
                .Add(TestFloat)
                .Add(TestDouble);
        }

        private static void FillTestArray(SBTNodeArray<int> target)
        {
            target.SetCapacity(5000);
            for (var i = 0; i < 5000; i++)
            {
                target.Add(i);
            }
        }
    }
}
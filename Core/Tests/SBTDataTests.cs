namespace Craiel.UnityEssentials.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http.Headers;
    using NUnit.Framework;
    using Runtime.Data.SBT;
    using Runtime.Data.SBT.Nodes;
    using Runtime.Enums;
    using Runtime.Extensions;
    using UnityEditor.VersionControl;
    using UnityEngine;

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

        private const string TestNote = "MyCustom Note Is Cool";
        
        private static readonly Vector2 TestVector2 = new Vector2(9.5f, 8.5f);
        private static readonly Vector3 TestVector3 = new Vector3(123.5f, 515.2f, 999991.5534f);
        private static readonly Quaternion TestQuaternion = Quaternion.LookRotation(Vector3.left, Vector3.up);
        private static readonly Color TestColor = new Color(0.2f, 0.6f, 0.1f, 0.2f);
        
        private static readonly DateTime TestTime = new DateTime(1995, 4, 3, 10, 25, 44, 123);
        private static readonly TimeSpan TestTimeSpan = TimeSpan.FromTicks(813524617346161);
        
        private const int StreamTestCount = 100;
        
        private static readonly int[] TestArray = {5, 15, 23, 591, int.MaxValue};
        private static readonly byte[] ByteArrayTestData = {25, 99, 100};

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        
        [Test]
        public static void SimpleTypeListTest()
        {
            var lData = new SBTList();
            FillTestList(lData);
            
            Assert.AreEqual(TestNote, lData.ReadNote(7));
            
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
            Assert.IsTrue(lDataOut.TryReadULong("ULong", out ulong uLongTryReadData));
            Assert.AreEqual(TestULong, uLongTryReadData);
            
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
        public static void StreamTest()
        {
            var lData = new SBTList();

            var nestedStream = lData.AddStream();
            FillTestStream(nestedStream);
            
            Assert.AreEqual(700, nestedStream.Length);

            byte[] lbData = lData.Serialize();
            byte[] lbcData = lData.SerializeCompressed();
            
            Assert.AreEqual(712, lbData.Length);
            Assert.AreEqual(197, lbcData.Length);
            
            var lDataOut = SBTList.Deserialize(lbData);
            var lcDataOut = SBTList.DeserializeCompressed(lbcData);
            
            Assert.AreEqual(lData.Count, lDataOut.Count);
            Assert.AreEqual(lData.Count, lcDataOut.Count);

            var compareStream = lDataOut.ReadStream(0);
            Assert.AreEqual(700, compareStream.Length);
            using (var reader = compareStream.BeginRead())
            {
                for (var i = 0; i < StreamTestCount; i++)
                {
                    int id = reader.ReadInt32();
                    Assert.AreEqual(i, id);

                    byte[] data = reader.ReadBytes(ByteArrayTestData.Length);
                    Assert.IsTrue(data.SequenceEqual(ByteArrayTestData));
                }
            }
        }

        [Test]
        public static void StringTest()
        {
            var lData = new SBTList();
            var dData = new SBTDictionary();
            
            FillTestList(lData);
            FillTestDictionary(dData);
            
            string lbData = lData.SerializeToString();
            string dbData = dData.SerializeToString();
            
            // Note: Compression with so little data actually produces overhead
            Assert.AreEqual(116, lbData.Length);
            Assert.AreEqual(188, dbData.Length);
            
            // Deserialize
            var lDataOut = SBTList.Deserialize(lbData);
            var dDataOut = SBTDictionary.Deserialize(dbData);
            
            Assert.AreEqual(lData.Count, lDataOut.Count);
            Assert.AreEqual(dData.Count, dDataOut.Count);
        }

        [Test]
        public static void UnityTest()
        {
            var lData = new SBTList();
            var dData = new SBTDictionary();

            lData.Add(TestVector2);
            lData.Add(TestVector3);
            lData.Add(TestQuaternion);
            lData.Add(TestColor);
            
            dData.Add("Vec2", TestVector2);
            dData.Add("Vec3A", TestVector3);
            dData.Add("Rotation", TestQuaternion);
            dData.Add("RotationCMP", TestQuaternion.Compress());
            dData.Add("MyColor", TestColor);
            
            string lbData = lData.SerializeToString();
            string dbData = dData.SerializeToString();
            
            // Note: Compression with so little data actually produces overhead
            Assert.AreEqual(92, lbData.Length);
            Assert.AreEqual(160, dbData.Length);
            
            // Deserialize
            var lDataOut = SBTList.Deserialize(lbData);
            var dDataOut = SBTDictionary.Deserialize(dbData);
            
            Assert.AreEqual(lData.Count, lDataOut.Count);
            Assert.AreEqual(dData.Count, dDataOut.Count);
            
            Assert.AreEqual(lData.ReadVector2(0), lData.ReadVector2(0));
            Assert.AreEqual(lData.ReadVector3(1), lData.ReadVector3(1));
            Assert.AreEqual(lData.ReadQuaternion(2), lData.ReadQuaternion(2));
            Assert.AreEqual(lData.ReadColor(3), lData.ReadColor(3));
            
            Assert.AreEqual(dData.ReadVector2("Vec2"), dDataOut.ReadVector2("Vec2"));
            Assert.AreEqual(dData.ReadVector3("Vec3A"), dDataOut.ReadVector3("Vec3A"));
            Assert.AreEqual(dData.ReadQuaternion("Rotation"), dDataOut.ReadQuaternion("Rotation"));
            Assert.AreEqual(dData.ReadLong("RotationCMP"), dDataOut.ReadLong("RotationCMP"));
            Assert.AreEqual(dData.ReadColor("MyColor"), dDataOut.ReadColor("MyColor"));
        }

        [Test]
        public static void DateTimeTest()
        {
            var lData = new SBTList();
            var dData = new SBTDictionary();

            lData.Add(TestTime);
            lData.Add(TestTimeSpan);

            dData.Add("Dat", TestTime);
            dData.Add("TS", TestTimeSpan);
            
            string lbData = lData.SerializeToString();
            string dbData = dData.SerializeToString();
            
            // Note: Compression with so little data actually produces overhead
            Assert.AreEqual(36, lbData.Length);
            Assert.AreEqual(48, dbData.Length);
            
            // Deserialize
            var lDataOut = SBTList.Deserialize(lbData);
            var dDataOut = SBTDictionary.Deserialize(dbData);
            
            Assert.AreEqual(lData.Count, lDataOut.Count);
            Assert.AreEqual(dData.Count, dDataOut.Count);
            
            Assert.AreEqual(TestTime, lData.ReadDateTime(0));
            Assert.AreEqual(TestTimeSpan, dData.ReadTimeSpan("TS"));
        }

        [Test]
        public static void TOMLTest()
        {
            var dData = new SBTDictionary();
            
            FillTestDictionary(dData);
            dData.AddArray("TestArray", TestArray);
            dData.Add("Date", TestTime);
            dData.Add("TimeSpan", TestTimeSpan);
            dData.Add("VEC", TestVector3);
            dData.Add("Color", TestColor);
            dData.Add("QRTN", TestQuaternion);

            var nested = dData.AddDictionary("NestedDict");
            FillTestDictionary(nested);
            
            nested = nested.AddDictionary("NestedDict2", note: "Testing Nested Dictionaries");
            FillTestDictionary(nested);
            nested.AddArray("ARR", TestArray, note: "This is the second nested array");

            var serializer = new SBTTOMLSerializer();
            dData.Serialize(serializer);
            string dbData = serializer.GetData();
            
            UnitTestUtils.Log(dbData);
            
            Assert.AreEqual(1129, dbData.Length);
            
            // TODO: Deserialize
            /*var dDataOut = SBTDictionary.Deserialize(dbData);
            
            Assert.AreEqual(dData.Count, dDataOut.Count);*/
        }
        
        /*[Test]
        public static void JSONTest()
        {
            var dData = new SBTDictionary();
            
            FillTestDictionary(dData);
            dData.AddArray("TestArray", TestArray);
            
            var nested = dData.AddDictionary("NestedDict");
            FillTestDictionary(nested);
            
            nested = dData.AddDictionary("NestedDict2");
            FillTestDictionary(nested);
            nested.AddArray("ARR", TestArray);
            
            string dbData = dData.SerializeToString();
            
            UnitTestUtils.Log(dbData);
            
            Assert.AreEqual(942, dbData.Length);
            
            // Deserialize
            var dDataOut = SBTDictionary.Deserialize(dbData);
            
            Assert.AreEqual(dData.Count, dDataOut.Count);
        }*/
        
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
                .Add("ULong", TestULong, note: "Some other note")
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
                .Add(TestULong, SBTFlags.Debug, TestNote)
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

        private static void FillTestStream(SBTNodeStream target)
        {
            target.Seek(0, SeekOrigin.Begin);
            using (var writer = target.BeginWrite())
            {
                for (var i = 0; i < StreamTestCount; i++)
                {
                    writer.Write(i);
                    writer.Write(ByteArrayTestData);
                }
            }
        }
    }
}
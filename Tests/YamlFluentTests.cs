namespace Craiel.UnityEssentials.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using Runtime.Enums;
    using Runtime.Utils;

    public static class YamlFluentTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public static void Serialize()
        {
            string data = GetSerializedTest(); 
                
            Assert.NotNull(data);
            Assert.AreEqual(93, data.Length);
            
            UnitTestUtils.Log(data);
        }

        [Test]
        public static void Deserialize()
        {
            string data = GetSerializedTest();

            var deserializer = new YamlFluentDeserializer(data);
            deserializer.BeginRead();
            deserializer.Read("TestKey", out int testInt);
            deserializer.Read("TestKey2", out string testStr);
            deserializer.Read(88, out int testInt2);
            
            Assert.AreEqual(25, testInt);
            Assert.AreEqual("TestValue", testStr);
            Assert.AreEqual(123, testInt2);
            
            deserializer.BeginRead("List");
            Assert.AreEqual(4, deserializer.GetElementCount());
            deserializer.Read(1, out ushort listEntry);
            Assert.AreEqual(5, listEntry);
            deserializer.ReadAll(out IList<int> values);
            Assert.AreEqual(4, values.Count);
            Assert.AreEqual(15, values[2]);
            
            deserializer.EndRead();
            
            deserializer.EndRead();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static string GetSerializedTest()
        {
            return new YamlFluentSerializer()
                .Begin(YamlContainerType.Dictionary)
                    .Add("TestKey", 25)
                    .Add("TestKey2", "TestValue")
                    .Add(88, 123)
                    .Begin("List", YamlContainerType.List)
                        .Add(1)
                        .Add(5)
                        .Add(15)
                        .Add(20)
                    .End()
                .End()
                .Serialize();
        }
    }
}
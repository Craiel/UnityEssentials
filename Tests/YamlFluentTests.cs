namespace Craiel.UnityEssentials.Tests
{
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
            Assert.AreEqual(100, data.Length);
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
            
            deserializer.BeginRead();
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
                    .Begin(YamlContainerType.List)
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
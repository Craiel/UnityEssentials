namespace Craiel.UnityEssentials.Tests
{
    using System.Collections;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Runtime.Data;
    using Runtime.Data.SBT;
    using Runtime.EngineCore.Modules;
    using Runtime.Enums;
    using Runtime.IO;
    using UnityEngine.TestTools;

    public static class ModuleSaveLoadTests
    {
        private const string TestSaveName1 = "My Custom Game";
        private const string TestSaveName2 = @"Some@Random Game.\1/2";
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [UnityTest]
        public static IEnumerator PlayerPrefs()
        {
            var module = new ModuleSaveLoad();
            module.SetMode(SaveLoadMode.PlayerPrefs);

            IList<SaveDataId> saves = module.GetAvailableSaves();
            foreach (SaveDataId id in saves)
            {
                module.Delete(id);
            }
            
            TestModule(module);
            
            yield return null;
        }
        
        [UnityTest]
        public static IEnumerator DataPath()
        {
            var module = new ModuleSaveLoad();
            module.SetMode(SaveLoadMode.PersistentDataPath);
            
            // Change the save path so we are in an isolated environment
            var testTargetDirectory = ManagedDirectory.GetTempDirectory();
            module.SetSavePath(testTargetDirectory);
            
            TestModule(module);
            
            testTargetDirectory.Delete(true);
            
            yield return null;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void TestModule(ModuleSaveLoad module)
        {
            // Test basic module calls and initial state
            IList<SaveDataId> saves = module.GetAvailableSaves();
            Assert.IsNotNull(saves);
            Assert.AreEqual(0, saves.Count);

            // Test the SaveDataId
            SaveDataId testId = SaveDataId.Create(TestSaveName1);
            Assert.AreNotEqual(SaveDataId.Invalid, testId);
            Assert.AreEqual("my_custom_game", testId.Id);

            // Create a new entry and save it
            var save = module.Create(testId);
            save.Add("Foo", "Hello World123");

            Assert.True(module.Save(testId, save));
            
            saves = module.GetAvailableSaves();
            Assert.AreEqual(1, saves.Count);

            if (module.Mode == SaveLoadMode.PersistentDataPath)
            {
                ManagedFile localFile = module.GetSavePath(testId);
                Assert.IsNotNull(localFile);
                Assert.IsTrue(localFile.Exists);
                Assert.AreEqual(30, localFile.Size);
            }
            
            // Try loading the save again
            save = module.Load(testId);
            Assert.AreEqual(1, save.Count);
            
            // Delete the save
            Assert.IsTrue(module.Delete(testId));
            saves = module.GetAvailableSaves();
            Assert.AreEqual(0, saves.Count);
            
            if (module.Mode == SaveLoadMode.PersistentDataPath)
            {
                ManagedFile localFile = module.GetSavePath(testId);
                Assert.IsNotNull(localFile);
                Assert.IsFalse(localFile.Exists);
            }
        }
    }
}
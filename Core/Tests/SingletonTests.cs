namespace Craiel.UnityEssentials.Tests
{
    using System.Collections;
    using NUnit.Framework;
    using Runtime.Singletons;
    using UnityEngine;
    using UnityEngine.TestTools;

    public static class SingletonTests
    {
        [Test]
        public static void Singleton()
        {
            Assert.IsFalse(NormalSingleton.IsInstanceActive);
            Assert.IsNull(NormalSingleton.Instance);
            
            NormalSingleton.Instantiate();
            Assert.IsTrue(NormalSingleton.IsInstanceActive);
            Assert.IsNotNull(NormalSingleton.Instance);
            Assert.IsFalse(NormalSingleton.Instance.WasInitialized);
            
            NormalSingleton.DestroyInstance();
            Assert.IsFalse(NormalSingleton.IsInstanceActive);
            Assert.IsNull(NormalSingleton.Instance);
            
            NormalSingleton.InstantiateAndInitialize();
            Assert.IsTrue(NormalSingleton.IsInstanceActive);
            Assert.IsNotNull(NormalSingleton.Instance);
            Assert.IsTrue(NormalSingleton.Instance.WasInitialized);
            
            NormalSingleton.DestroyInstance();
            Assert.IsFalse(NormalSingleton.IsInstanceActive);
            Assert.IsNull(NormalSingleton.Instance);
        }

        [UnityTest]
        public static IEnumerator SingletonBehavior()
        {
            Assert.IsFalse(NormalSingletonBehavior.IsInstanceActive);
            Assert.IsNull(NormalSingletonBehavior.Instance);
            
            NormalSingletonBehavior.Instantiate();
            Assert.IsTrue(NormalSingletonBehavior.IsInstanceActive);
            Assert.IsNotNull(NormalSingletonBehavior.Instance);
            Assert.IsFalse(NormalSingletonBehavior.Instance.WasInitialized);
            Assert.IsNotNull(Object.FindObjectOfType<NormalSingletonBehavior>());
            
            NormalSingletonBehavior.Destroy();
            Assert.IsFalse(NormalSingletonBehavior.IsInstanceActive);
            Assert.IsNull(NormalSingletonBehavior.Instance);
            Assert.IsNull(Object.FindObjectOfType<NormalSingletonBehavior>());
            
            NormalSingletonBehavior.InstantiateAndInitialize();
            Assert.IsTrue(NormalSingletonBehavior.IsInstanceActive);
            Assert.IsNotNull(NormalSingletonBehavior.Instance);
            Assert.IsTrue(NormalSingletonBehavior.Instance.WasInitialized);
            Assert.IsNotNull(Object.FindObjectOfType<NormalSingletonBehavior>());
            
            NormalSingletonBehavior.Destroy();
            Assert.IsFalse(NormalSingletonBehavior.IsInstanceActive);
            Assert.IsNull(NormalSingletonBehavior.Instance);
            Assert.IsNull(Object.FindObjectOfType<NormalSingletonBehavior>());
            
            var test = new GameObject("Test123");
            var behavior = test.AddComponent<NormalSingletonBehavior>();
            Assert.IsNotNull(behavior);
            Assert.IsFalse(NormalSingletonBehavior.IsInstanceActive);
            
            behavior.SetAutoInstantiate();
            
            Assert.IsTrue(NormalSingletonBehavior.IsInstanceActive);
            
            NormalSingletonBehavior.Destroy();
            
            yield return null;
        }
    }

    internal class NormalSingleton : UnitySingleton<NormalSingleton>
    {
        public bool WasInitialized { get; set; }
        
        public override void Initialize()
        {
            base.Initialize();

            this.WasInitialized = true;
        }
    }
    
    internal class NormalSingletonBehavior : UnitySingletonBehavior<NormalSingletonBehavior>
    {
        public bool WasInitialized { get; set; }
        
        public override void Initialize()
        {
            base.Initialize();

            this.WasInitialized = true;
        }
    }
}
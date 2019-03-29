namespace Craiel.UnityEssentials.Tests
{
    using System.Collections;
    using NUnit.Framework;
    using Runtime;
    using Runtime.Extensions;
    using UnityEngine;
    using UnityEngine.TestTools;

    public class BoxColliderHelperTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [UnityTest]
        public static IEnumerator Run()
        {
            var testObject = GameObjectExtensions.CreateEmptyGameObject<BoxCollider2D>();
            var collider = testObject.GetComponent<BoxCollider2D>();
            collider.size = new Vector3(5, 5);
            
            Assert.IsTrue(new Vector3(-2.5f, -2.5f, 0) == collider.bounds.min);
            Assert.IsTrue(new Vector3(2.5f, 2.5f, 0) == collider.bounds.max);
            
            var helper = testObject.AddComponent<BoxColliderHelper>();
            Assert.IsTrue(Vector2.zero == helper.ColliderRect.min);
            Assert.IsTrue(new Vector2(5, 5) == helper.ColliderRect.max);
            
            yield return null;
        }
    }
}
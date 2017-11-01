namespace Assets.Scripts.Craiel.Essentials.Scene
{
    using System.Collections.Generic;
    using Enums;
    using Essentials;
    using UnityEngine;
    
    public class SceneObjectController : UnitySingleton<SceneObjectController>
    {
        private const string RootName = "Scene";

        private readonly IDictionary<SceneRootCategory, SceneObjectContainer> containers;

        private SceneObjectContainer rootContainer;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneObjectController()
        {
            this.containers = new Dictionary<SceneRootCategory, SceneObjectContainer>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SceneObjectRoot AcquireRoot(SceneRootCategory category, string rootName, bool persistent = false)
        {
            this.EnsureContainer(category);

            return this.containers[category].AcquireRoot(rootName, persistent);
        }

        public SceneObjectRoot RegisterObjectAsRoot(SceneRootCategory category, GameObject gameObject, bool persistent)
        {
            this.EnsureContainer(category);
            SceneObjectRoot root = this.containers[category].RegisterAsRoot(gameObject);
            root.Persistent = persistent;
            return root;
        }

        public void Clear(bool clearPersistentObjects = false)
        {
            foreach (SceneObjectContainer container in this.containers.Values)
            {
                container.Clear(clearPersistentObjects);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void EnsureRootContainer()
        {
            if (this.rootContainer == null)
            {
                this.rootContainer = new SceneObjectContainer(null, RootName);
            }
        }
        
        private void EnsureContainer(SceneRootCategory category)
        {
            this.EnsureRootContainer();

            if (!this.containers.ContainsKey(category))
            {
                this.containers[category] = new SceneObjectContainer(this.rootContainer, category.ToString());
            }
        }
    }
}

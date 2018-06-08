namespace Craiel.UnityEssentials.Runtime.EngineCore
{
    using System;
    using System.Collections.Generic;
    using Scene;

    public abstract partial class EssentialEngineCore<T, TSceneEnum>
    {
        private readonly IDictionary<TSceneEnum, Type> sceneImplementations = new Dictionary<TSceneEnum, Type>();

        private readonly IDictionary<TSceneEnum, BaseScene<TSceneEnum>> scenes = new Dictionary<TSceneEnum, BaseScene<TSceneEnum>>();

        private BaseScene<TSceneEnum> activeScene;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public TSceneEnum? ActiveSceneType { get; private set; }

        public TS GetScene<TS>()
            where TS : BaseScene<TSceneEnum>
        {
            return (TS)this.activeScene;
        }

        protected void RegisterSceneImplementation<TImplementation>(TSceneEnum type)
            where TImplementation : BaseScene<TSceneEnum>
        {
            this.sceneImplementations.Add(type, typeof(TImplementation));
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DestroyScene(TSceneEnum type)
        {
            if (!this.scenes.ContainsKey(type))
            {
                Logger.Warn("Scene {0} is not loaded, skipping shutdown", type);
                return;
            }

            this.scenes.Remove(type);
        }

        private void LoadScene(TSceneEnum type)
        {
            if (!this.scenes.ContainsKey(type))
            {
                BaseScene<TSceneEnum> scene = this.InitializeScene(type);
                if (scene == null)
                {
                    return;
                }

                this.scenes.Add(type, scene);
            }
        }

        private BaseScene<TSceneEnum> InitializeScene(TSceneEnum type)
        {
            if (this.scenes.ContainsKey(type))
            {
                Logger.Warn("Scene {0} is already loaded, skipping", type);
                return null;
            }

            Type implementation;
            if (!this.sceneImplementations.TryGetValue(type, out implementation))
            {
                Logger.Error("Scene {0} has no implementation defined!", type);
                return null;
            }

            if (!typeof(BaseScene<TSceneEnum>).IsAssignableFrom(implementation))
            {
                Logger.Error("Scene implementation {0} is not of type IGameScene!", implementation);
                return null;
            }

            return (BaseScene<TSceneEnum>)Activator.CreateInstance(implementation);
        }
    }
}

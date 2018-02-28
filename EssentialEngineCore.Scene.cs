using System;
using System.Collections.Generic;
using Assets.Scripts.Core.Scenes;
using Assets.Scripts.Craiel.Essentials.Scene;

namespace Assets.Scripts.Craiel.UnityEssentials
{
    public abstract partial class EssentialEngineCore<T, TSceneEnum>
    {
        private readonly IDictionary<TSceneEnum, Type> sceneImplementations = new Dictionary<TSceneEnum, Type>();

        private readonly IDictionary<TSceneEnum, BaseScene> scenes = new Dictionary<TSceneEnum, BaseScene>();

        private BaseScene activeScene;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public TSceneEnum? ActiveSceneType { get; private set; }

        public TS GetScene<TS>()
            where TS : BaseScene
        {
            return (TS)this.activeScene;
        }

        protected void RegisterSceneImplementation<T>(TSceneEnum type)
            where T : BaseScene
        {
            sceneImplementations.Add(type, typeof(T));
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
                BaseScene scene = this.InitializeScene(type);
                if (scene == null)
                {
                    return;
                }

                this.scenes.Add(type, scene);
            }
        }

        private BaseScene InitializeScene(TSceneEnum type)
        {
            if (this.scenes.ContainsKey(type))
            {
                Logger.Warn("Scene {0} is already loaded, skipping", type);
                return null;
            }

            Type implementation;
            if (!sceneImplementations.TryGetValue(type, out implementation))
            {
                Logger.Error("Scene {0} has no implementation defined!", type);
                return null;
            }

            if (!typeof(BaseScene).IsAssignableFrom(implementation))
            {
                Logger.Error("Scene implementation {0} is not of type IGameScene!", implementation);
                return null;
            }

            return (BaseScene)Activator.CreateInstance(implementation);
        }
    }
}

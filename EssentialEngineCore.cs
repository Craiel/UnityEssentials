namespace Assets.Scripts.Craiel.UnityEssentials
{
    using System;
    using System.Collections.Generic;
    using Audio;
    using Essentials;
    using Essentials.Enums;
    using Essentials.Event;
    using Essentials.I18N;
    using Essentials.Input;
    using Essentials.Logging;
    using Essentials.Resource;
    using Essentials.Scene;
    using GameData;
    using JetBrains.Annotations;
    using NLog;

    public abstract partial class EssentialEngineCore<T, TSceneEnum> : UnitySingletonBehavior<T>
        where T : EssentialEngineCore<T, TSceneEnum>
        where TSceneEnum: struct, IConvertible
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Initialize()
        {
            base.Initialize();

            // System components first
            NLogUtils.InitializeDefaultConfig();

            ResourceProvider.InstantiateAndInitialize();
            BundleProvider.InstantiateAndInitialize();
            ResourceStreamProvider.InstantiateAndInitialize();

            // Do localization fairly early so everything can run through it
            LocalizationSystem.InstantiateAndInitialize();

            // Bring up the object controller and Register the root
            SceneObjectController.InstantiateAndInitialize();
            SceneObjectController.Instance.RegisterObjectAsRoot(SceneRootCategory.System, this.gameObject, true);

            // LoadFromProto resources all components will need
            this.LoadPermanentResources();

            // First some main systems
            GameEvents.InstantiateAndInitialize();
            GameRuntimeData.InstantiateAndInitialize();
            InputHandler.InstantiateAndInitialize();
            AudioSystem.InstantiateAndInitialize();
            AudioAreaSystem.InstantiateAndInitialize();

            try
            {
                // Initialize Game specific components
                this.InitializeGameComponents();

                UnityEngine.Debug.LogWarning("Essential Engine.Initialize() complete");
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Error in Initialize of Sub-systems: " + e);
                throw;
            }
        }

        [UsedImplicitly]
        public virtual void Update()
        {
            if (this.transitioning)
            {
                this.UpdateSceneTransition();
            }
        }

#if UNITY_EDITOR
        public IDictionary<ResourceKey, long> GetHistory()
        {
            return ResourceProvider.Instance.GetHistory();
        }
#endif

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void LoadPermanentResources()
        {
            // Todo: Add resources we will always need

            ResourceProvider.Instance.LoadImmediate();
        }

        protected abstract void InitializeGameComponents();
    }
}

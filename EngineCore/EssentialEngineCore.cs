using BundleProvider = Craiel.UnityEssentials.Resource.BundleProvider;
using GameEvents = Craiel.UnityEssentials.Event.GameEvents;
using InputHandler = Craiel.UnityEssentials.Input.InputHandler;
using LocalizationSystem = Craiel.UnityEssentials.I18N.LocalizationSystem;
using NLogUtils = Craiel.UnityEssentials.Logging.NLogUtils;
using ResourceKey = Craiel.UnityEssentials.Resource.ResourceKey;
using ResourceProvider = Craiel.UnityEssentials.Resource.ResourceProvider;
using ResourceStreamProvider = Craiel.UnityEssentials.Resource.ResourceStreamProvider;
using SceneObjectController = Craiel.UnityEssentials.Scene.SceneObjectController;
using SceneRootCategory = Craiel.UnityEssentials.Enums.SceneRootCategory;

namespace Craiel.UnityEssentials.EngineCore
{
    using System;
    using System.Collections.Generic;
    using JetBrains.Annotations;
    using NLog;
    using Singletons;

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
            InputHandler.InstantiateAndInitialize();

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

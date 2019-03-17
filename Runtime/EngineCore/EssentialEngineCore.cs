namespace Craiel.UnityEssentials.Runtime.EngineCore
{
    using System;
    using System.Collections.Generic;
    using Enums;
    using Event;
    using Events;
    using I18N;
    using Input;
    using JetBrains.Annotations;
    using Logging;
    using Resource;
    using Scene;
    using Singletons;
    using TweenLite;

    public abstract partial class EssentialEngineCore<T, TSceneEnum> : UnitySingletonBehavior<T>
        where T : EssentialEngineCore<T, TSceneEnum>
        where TSceneEnum: struct, IConvertible
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Initialize()
        {
            if (EssentialEngineState.IsInitialized)
            {
                throw new InvalidOperationException("Engine was already initialized!");
            }
            
            base.Initialize();

            // Logging first
            if (!NLogInterceptor.IsInstanceActive)
            {
                NLogInterceptor.InstantiateAndInitialize();
            }

            UnityNLogRelay.InstantiateAndInitialize();

            NLogUtils.ConfigureNLog(UnityEngine.Application.persistentDataPath, NLogInterceptor.Instance.Target);

            // System components next
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
            UIEvents.InstantiateAndInitialize();
            InputHandler.InstantiateAndInitialize();
            TweenLiteSystem.InstantiateAndInitialize();

            try
            {
                // Initialize Game specific components
                this.InitializeGameComponents();

                UnityEngine.Debug.LogWarning("Essential Engine.Initialize() complete");

                EssentialEngineState.IsInitialized = true;
                GameEvents.Send(new EventEngineInitialized());
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

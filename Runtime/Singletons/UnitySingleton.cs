namespace Craiel.UnityEssentials.Runtime.Singletons
{
    using Contracts;
    
    public abstract class UnitySingleton<T> : IUnitySingleton
        where T : class, IUnitySingleton, new()
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool IsInstanceActive
        {
            get
            {
                return Instance != null;
            }
        }

        public static T Instance { get; private set; }

        public bool IsInitialized { get; protected set; }

        public static void Instantiate()
        {
            // Prevent accidential duplicate initialization
            if (Instance != null)
            {
                return;
            }

            Instance = new T();
            
#if DEBUG
            EssentialsCore.Logger.Info("Singleton.Instantiate: {0}", typeof(T).Name);
#endif
        }
        
        public static void InstantiateAndInitialize()
        {
            if (Instance != null && Instance.IsInitialized)
            {
                // Instance is already there and initialized, skip
                return;
            }

            Instantiate();
            Instance.Initialize();
        }

        public virtual void Initialize()
        {
#if DEBUG
            EssentialsCore.Logger.Info("Singleton.Initialize: {0}", this.GetType().Name);
#endif

            this.IsInitialized = true;
        }

        public static void DestroyInstance()
        {
            if (IsInstanceActive)
            {
                Instance.DestroySingleton();
            }
        }

        public virtual void DestroySingleton()
        {
#if DEBUG
            EssentialsCore.Logger.Info("Singleton.Destroy: {0}", this.GetType().Name);
#endif

            Instance = null;
        }
    }
}

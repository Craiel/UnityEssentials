namespace Craiel.UnityEssentials.Runtime.Singletons
{
    using System;
    using Contracts;
    using Enums;
    using Scene;
    using UnityEngine;
    
    public abstract class UnitySingletonBehavior<T> : MonoBehaviour, IUnitySingletonBehavior
        where T : UnitySingletonBehavior<T>
    {
        private static T instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public bool AutoInstantiate;

        public static bool IsInstanceActive
        {
            get
            {
                return instance != default(T);
            }
        }

        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        public bool IsInitialized { get; protected set; }

        public static void Instantiate()
        {
            if (Instance != null)
            {
                return;
            }
            
            instance = FindObjectOfType<T>();
            
            if (instance == null)
            {
                GameObject gameObject = new GameObject(typeof(T).Name);

                try
                {
                    instance = gameObject.AddComponent<T>();
                }
                catch (Exception e)
                {
                    EssentialsCore.Logger.Error("Error trying to add Singleton Component {0}: {1}", typeof(T), e);
                }

                if (instance == null)
                {
                    EssentialsCore.Logger.Error("Adding Component of type {0} returned null", typeof(T));
                }
                
                // Only attempt Don't destroy if the object has no parent
                if (gameObject.transform.parent == null)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }

#if DEBUG
        public static void InstantiateAndInitializeForUnitTest()
        {
            if (Instance != null && Instance.IsInitialized)
            {
                return;
            }

            instance = Activator.CreateInstance<T>();
            instance.Initialize();
        }
#endif

        public static void InstantiateAndInitialize()
        {
            if (Instance != null && Instance.IsInitialized)
            {
                return;
            }

            Instantiate();

            instance.Initialize();
        }

        public static void Destroy()
        {
            instance.DestroySingleton();
        }

        public virtual void Awake()
        {
            if (instance == null && this.AutoInstantiate)
            {
                instance = (T)this;
            }

            if (instance != null && instance != this)
            {
                EssentialsCore.Logger.Error("Duplicate Instance of {0} found, destroying!", this.GetType());
                DestroyImmediate(this.gameObject);
            }
        }

        public virtual void Initialize()
        {
            this.IsInitialized = true;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected SceneObjectRoot RegisterInController(SceneObjectController parent, SceneRootCategory category, bool persistent = false)
        {
            SceneObjectRoot root = parent.RegisterObjectAsRoot(category, this.gameObject, persistent);

            root.OnDestroying += this.OnSingletonDestroying;

            return root;
        }
        
        protected void DestroySingleton()
        {
            EssentialsCore.Logger.Info("Destroying Singleton MonoBehavior {0}", this.name);

            this.OnSingletonDestroying();

            Destroy(this.gameObject);
        }

        protected virtual void OnSingletonDestroying()
        {
            // Clear out the instance since our parent got destroyed
            instance = null;
        }
    }
}

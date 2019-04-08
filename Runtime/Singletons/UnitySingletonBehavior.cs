namespace Craiel.UnityEssentials.Runtime.Singletons
{
    using System;
    using Contracts;
    using Enums;
    using Scene;
    using UnityEngine;
    
    [ExecuteInEditMode]
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

        public void SetAutoInstantiate()
        {
            if (!this.AutoInstantiate)
            {
                this.AutoInstantiate = true;
            }

            if (Instance == null)
            {
                InstantiateAndInitialize();
            }
        }

        public static void Instantiate()
        {
            if (Instance != null)
            {
                return;
            }
            
            instance = FindObjectOfType<T>();
            
            if (instance == null)
            {
#if DEBUG
                EssentialsCore.Logger.Info("SingletonBehavior.Instantiate: {0}", typeof(T).Name);
#endif
                
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
                if (gameObject.transform.parent == null && !RuntimeInfo.RunningFromNUnit)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }

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
#if DEBUG
            EssentialsCore.Logger.Info("SingletonBehavior.Awake: {0}", this.GetType().Name);
#endif
            
            if (instance == null && this.AutoInstantiate)
            {
#if DEBUG
                EssentialsCore.Logger.Info("SingletonBehavior.AutoInstantiate: {0}", typeof(T).Name);
#endif
                
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
#if DEBUG
            EssentialsCore.Logger.Info("SingletonBehavior.Initialize: {0}", this.GetType().Name);
#endif
            
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
#if DEBUG
            EssentialsCore.Logger.Info("SingletonBehavior.Destroy: {0}", this.GetType().Name);
#endif

            this.OnSingletonDestroying();

            if (RuntimeInfo.RunningFromNUnit)
            {
                DestroyImmediate(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual void OnSingletonDestroying()
        {
            // Clear out the instance since our parent got destroyed
            instance = null;
        }
    }
}

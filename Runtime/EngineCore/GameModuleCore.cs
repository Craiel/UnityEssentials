namespace Craiel.UnityEssentials.Runtime.EngineCore
{
    using Enums;
    using Scene;
    using Singletons;

    public abstract class GameModuleCore<T> : UnitySingletonBehavior<T>
        where T : GameModuleCore<T>
    {
        private IGameModule[] modules;

        private bool isInitialized;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Awake()
        {
            SceneObjectController.Instance.RegisterObjectAsRoot(SceneRootCategory.System, this.gameObject, true);
            
            base.Awake();
        }

        public override void Initialize()
        {
            base.Initialize();
            
            foreach (IGameModule module in this.modules)
            {
                module.Initialize();
            }
        }

        public void Update()
        {
            for (var i = 0; i < this.modules.Length; i++)
            {
                this.modules[i].Update();
            }
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < this.modules.Length; i++)
            {
                this.modules[i].FixedUpdate();
            }
        }

        public void OnDestroy()
        {
            for (var i = 0; i < this.modules.Length; i++)
            {
                this.modules[i].Destroy();
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void SetModules(params IGameModule[] newModules)
        {
            if (newModules == null || newModules.Length == 0)
            {
                this.modules = new IGameModule[0];
                return;
            }
            
            this.modules = newModules;
        }
    }
}
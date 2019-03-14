namespace Craiel.UnityEssentials.Runtime.EngineCore
{
    using System.Collections.Generic;
    using System.Linq;
    using Enums;
    using Modules;
    using Scene;
    using Singletons;

    public abstract class GameModuleCore<T> : UnitySingletonBehavior<T>
        where T : GameModuleCore<T>
    {
        private IGameModule[] modules;

        private bool isInitialized;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected GameModuleCore()
        {
            this.SaveLoad = new ModuleSaveLoad();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ModuleSaveLoad SaveLoad { get; private set; }
        
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
        protected void SetModules(bool includeDefaultModules = true, params IGameModule[] newModules)
        {
            IList<IGameModule> moduleList = new List<IGameModule>();

            if (includeDefaultModules)
            {
                moduleList.Add(this.SaveLoad);
            }
            
            if (newModules == null || newModules.Length == 0)
            {
                this.SetModules(moduleList);
                return;
            }
            
            foreach (IGameModule module in newModules)
            {
                if (moduleList.Contains(module))
                {
                    continue;
                }
                
                moduleList.Add(module);
            }
            
            this.SetModules(moduleList);
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void SetModules(IList<IGameModule> moduleList)
        {
            if (moduleList == null)
            {
                this.modules = new IGameModule[0];
                return;
            }
            
            this.modules = moduleList.ToArray();
        }
    }
}
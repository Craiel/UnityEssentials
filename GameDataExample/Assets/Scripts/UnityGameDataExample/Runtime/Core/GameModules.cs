using Craiel.UnityGameData.Runtime;
using UnityGameDataExample.Runtime.Core.Setup;

namespace UnityGameDataExample.Runtime.Core
{
    using Craiel.UnityEssentials.Runtime.EngineCore;
    using Modules;

    public class GameModules : GameModuleCore<GameModules>
    {
        private bool isInitialized;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GameModules()
        {
            this.GameState = new ModuleGameState(this);
            this.LogicCore = new ModuleLogicCore(this);
            this.Scaffolding = new ModuleGameScaffolding(this);

            this.SetModules(true,
                // Note: Order matters here
                this.LogicCore,
                this.Scaffolding,
                this.GameState);
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ModuleGameState GameState { get; private set; }

        public ModuleLogicCore LogicCore { get; private set; }

        public ModuleGameScaffolding Scaffolding { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            
            GameRuntimeData.Instance.Load(Constants.GameDataResourceKey);
        }
    }
}
namespace UnityGameDataExample.Runtime.Modules
{
    using Core;
    using Craiel.UnityEssentials.Runtime.EngineCore;
    using Enums;

    public class ModuleGameState : GameModuleBase<GameModules>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModuleGameState(GameModules parent)
            : base(parent)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GameMasterState MasterState { get; set; }
        
        public GameMode Mode { get; set; }
        
        public override void Initialize()
        {
            base.Initialize();
            
            this.Mode = GameMode.Default;
        }
    }
}

namespace UnityGameDataExample.Runtime.Modules
{
    using System.Collections.Generic;
    using Core;
    using Craiel.UnityEssentials.Runtime.EngineCore;
    using Craiel.UnityEssentials.Runtime.Extensions;
    using Enums;
    using Logic;

    public class ModuleLogicCore : GameModuleBase<GameModules>
    {
        private readonly IDictionary<GameEntityId, GameEntityBase> entityRegister;
        private readonly IList<GameEntityId> entityTickCache;

        private ulong nextGameEntityId = 100;

        private bool updateEntityCache;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModuleLogicCore(GameModules parent)
            : base(parent)
        {
            this.entityRegister = new Dictionary<GameEntityId, GameEntityBase>();
            this.entityTickCache = new List<GameEntityId>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public FactionProvider FactionProvider { get; private set; }
        
        public RaceProvider RaceProvider { get; private set; }
        
        public ClassProvider ClassProvider { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            this.FactionProvider = new FactionProvider();
            this.RaceProvider = new RaceProvider();
            this.ClassProvider = new ClassProvider();
        }

        public override void Destroy()
        {
            this.FactionProvider.Dispose();
            this.RaceProvider.Dispose();
            this.ClassProvider.Dispose();
            
            base.Destroy();
        }

        public override void Update()
        {
            base.Update();

            if (this.updateEntityCache)
            {
                this.updateEntityCache = false;
                this.entityTickCache.Clear();
                this.entityTickCache.AddRange(this.entityRegister.Keys);
            }

            foreach (GameEntityId id in this.entityTickCache)
            {
                GameEntityBase entity = this.entityRegister[id];
                if (entity.IsDead)
                {
                    continue;
                }

                entity.TickEntity();
            }
        }

        public void RegisterEntity(GameEntityBase data)
        {
            this.entityRegister.Add(data.Id, data);
            this.updateEntityCache = true;
        }

        public void UnregisterEntity(GameEntityId id)
        {
            this.entityRegister.Remove(id);
            this.updateEntityCache = true;
        }

        public T Get<T>(GameEntityId id)
            where T : GameEntityBase
        {
            GameEntityBase result = this.Get(id);
            return (T)result;
        }

        public GameEntityBase Get(GameEntityId id)
        {
            GameEntityBase result;
            if (this.entityRegister.TryGetValue(id, out result))
            {
                return result;
            }

            return null;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private GameEntityId GetNextGameEntityId(GameEntityType type)
        {
            return new GameEntityId(this.nextGameEntityId++, type);
        }
    }
}

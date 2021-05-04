namespace UnityGameDataExample.Runtime.Logic
{
    using System.Collections.Generic;
    using Craiel.UnityEssentials.Runtime.Resource;
    using Craiel.UnityGameData.Runtime;
    using Data.Runtime;
    using Enums;

    public class FactionProvider : GameDataProvider<RuntimeFactionData>
    {
        private readonly IDictionary<FactionType, GameDataId> typeLookup;
        private readonly IDictionary<FactionType, ResourceKey> iconLargeLookup;
        private readonly IDictionary<FactionType, ResourceKey> iconSmallLookup;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FactionProvider()
        {
            this.typeLookup = new Dictionary<FactionType, GameDataId>();
            this.iconLargeLookup = new Dictionary<FactionType, ResourceKey>();
            this.iconSmallLookup = new Dictionary<FactionType, ResourceKey>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GameDataId Get(FactionType type)
        {
            GameDataId result;
            if (this.typeLookup.TryGetValue(type, out result))
            {
                return result;
            }
            
            return GameDataId.Invalid;
        }

        public ResourceKey GetIcon(FactionType type, bool getLargeIcon = false)
        {
            ResourceKey result;
            if (getLargeIcon && this.iconLargeLookup.TryGetValue(type, out result))
            {
                return result;
            }

            if (this.iconSmallLookup.TryGetValue(type, out result))
            {
                return result;
            }

            return ResourceKey.Invalid;
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void PostLoad()
        {
            base.PostLoad();

            this.typeLookup.Clear();
            this.iconLargeLookup.Clear();
            this.iconSmallLookup.Clear();
            
            foreach (GameDataId dataId in this)
            {
                var data = GameRuntimeData.Instance.Get<RuntimeFactionData>(dataId);
                this.typeLookup.Add(data.InternalType, dataId);

                if (data.IconLarge != ResourceKey.Invalid)
                {
                    this.iconLargeLookup.Add(data.InternalType, data.IconLarge);
                }

                if (data.IconSmall != ResourceKey.Invalid)
                {
                    this.iconSmallLookup.Add(data.InternalType, data.IconLarge);
                }
            }
        }
    }
}
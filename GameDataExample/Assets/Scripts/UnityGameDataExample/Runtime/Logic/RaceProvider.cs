using System.Linq;
using Craiel.UnityEssentials.Runtime.Extensions;
using UnityGameDataExample.Runtime.Core;

namespace UnityGameDataExample.Runtime.Logic
{
    using System.Collections.Generic;
    using Craiel.UnityEssentials.Runtime.Resource;
    using Craiel.UnityGameData.Runtime;
    using Data.Runtime;
    using Enums;

    public class RaceProvider : GameDataProvider<RuntimeRaceData>
    {
        private readonly IDictionary<CharacterRaceType, GameDataId> typeLookup;
        private readonly IDictionary<CharacterRaceType, ResourceKey> iconLargeLookup;
        private readonly IDictionary<CharacterRaceType, ResourceKey> iconSmallLookup;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public RaceProvider()
        {
            this.typeLookup = new Dictionary<CharacterRaceType, GameDataId>();
            this.iconLargeLookup = new Dictionary<CharacterRaceType, ResourceKey>();
            this.iconSmallLookup = new Dictionary<CharacterRaceType, ResourceKey>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GameDataId Get(CharacterRaceType type)
        {
            GameDataId result;
            if (this.typeLookup.TryGetValue(type, out result))
            {
                return result;
            }
            
            return GameDataId.Invalid;
        }

        public ResourceKey GetIcon(CharacterRaceType type, bool getLargeIcon = false)
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

        public RaceProvider ByFaction(FactionType faction)
        {
            var dataId = GameModules.Instance.LogicCore.FactionProvider.Get(faction);
            if (dataId == GameDataId.Invalid)
            {
                this.FilteredList.Clear();
                return this;
            }
            
            IList<RuntimeRaceData> cache = this.FilteredList.Where(x => x.Factions.Contains(dataId)).ToList();
            this.FilteredList.Clear();
            this.FilteredList.AddRange(cache);
            return this;
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
                var data = GameRuntimeData.Instance.Get<RuntimeRaceData>(dataId);
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
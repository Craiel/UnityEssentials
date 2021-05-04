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

    public class ClassProvider : GameDataProvider<RuntimeClassData>
    {
        private readonly IDictionary<CharacterClassType, GameDataId> typeLookup;
        private readonly IDictionary<CharacterClassType, ResourceKey> iconLargeLookup;
        private readonly IDictionary<CharacterClassType, ResourceKey> iconSmallLookup;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ClassProvider()
        {
            this.typeLookup = new Dictionary<CharacterClassType, GameDataId>();
            this.iconLargeLookup = new Dictionary<CharacterClassType, ResourceKey>();
            this.iconSmallLookup = new Dictionary<CharacterClassType, ResourceKey>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public GameDataId Get(CharacterClassType type)
        {
            GameDataId result;
            if (this.typeLookup.TryGetValue(type, out result))
            {
                return result;
            }
            
            return GameDataId.Invalid;
        }

        public ResourceKey GetIcon(CharacterClassType type, bool getLargeIcon = false)
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
        
        public ClassProvider ByFaction(FactionType faction)
        {
            var dataId = GameModules.Instance.LogicCore.FactionProvider.Get(faction);
            if (dataId == GameDataId.Invalid)
            {
                this.FilteredList.Clear();
                return this;
            }
            
            IList<RuntimeClassData> cache = this.FilteredList.Where(x => x.Factions.Contains(dataId)).ToList();
            this.FilteredList.Clear();
            this.FilteredList.AddRange(cache);
            return this;
        }
        
        public ClassProvider ByRace(CharacterRaceType race)
        {
            var dataId = GameModules.Instance.LogicCore.RaceProvider.Get(race);
            if (dataId == GameDataId.Invalid)
            {
                this.FilteredList.Clear();
                return this;
            }
            
            IList<RuntimeClassData> cache = this.FilteredList.Where(x => x.Races.Contains(dataId)).ToList();
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
                var data = GameRuntimeData.Instance.Get<RuntimeClassData>(dataId);
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
using Craiel.UnityEssentials.Runtime.Resource;
using Craiel.UnityEssentialsUI.Runtime.GameControllers;
using UnityEngine;
using UnityEngine.UI;
using UnityGameDataExample.Runtime.Core;
using UnityGameDataExample.Runtime.Data.Runtime;
using UnityGameDataExample.Runtime.Logic;

namespace UnityGameDataExample.Runtime.UI
{
    public class UITestController : UIControllerBase
    {
        public Image Class;
        public Image Race;
        public Image Faction;

        private RuntimeFactionData selectedFaction;
        private RuntimeClassData selectedClass;
        private RuntimeRaceData selectedRace;

        public override void Awake()
        {
            base.Awake();
            
            this.Shuffle();
        }

        public void Shuffle()
        {
            GameModules.Instance.LogicCore.FactionProvider.Reset();
            this.selectedFaction = GameModules.Instance.LogicCore.FactionProvider.GetRandom();

            var icon = GameModules.Instance.LogicCore.FactionProvider.GetIcon(this.selectedFaction.InternalType);
            if (icon != ResourceKey.Invalid)
            {
                this.Faction.sprite = icon.LoadManaged<Sprite>();
            }

            GameModules.Instance.LogicCore.RaceProvider.Reset();
            GameModules.Instance.LogicCore.RaceProvider.ByFaction(this.selectedFaction.InternalType);
            this.selectedRace = GameModules.Instance.LogicCore.RaceProvider.GetRandom();
            icon = GameModules.Instance.LogicCore.RaceProvider.GetIcon(this.selectedRace.InternalType);
            if (icon != ResourceKey.Invalid)
            {
                this.Class.sprite = icon.LoadManaged<Sprite>();
            }

            GameModules.Instance.LogicCore.ClassProvider.Reset();
            GameModules.Instance.LogicCore.ClassProvider.ByFaction(this.selectedFaction.InternalType);
            GameModules.Instance.LogicCore.ClassProvider.ByRace(this.selectedRace.InternalType);
            this.selectedClass = GameModules.Instance.LogicCore.ClassProvider.GetRandom();
            icon = GameModules.Instance.LogicCore.ClassProvider.GetIcon(this.selectedClass.InternalType);
            if (icon != ResourceKey.Invalid)
            {
                this.Race.sprite = icon.LoadManaged<Sprite>();
            }
        }

        public void Start()
        {
            
        }
    }
}
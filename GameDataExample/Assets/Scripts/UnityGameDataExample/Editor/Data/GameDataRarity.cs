namespace UnityGameDataExample.Data.Editor
{
    using Craiel.UnityEssentials.Runtime.Extensions;
    using Craiel.UnityGameData.Editor.Builder;
    using Craiel.UnityGameData.Editor.Common;
    using Runtime.Data;
    using Runtime.Data.Runtime;
    using Runtime.Enums;
    using UnityEngine;

    public class GameDataRarity : GameDataObject
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public Color Color;
        
        [SerializeField]
        public uint MinLevel;

        [SerializeField]
        public Rarity Rarity;
        
        [SerializeField]
        public bool CanDrop = true;
        
        [SerializeField]
        public float DropRarityValue = 100;

        [SerializeField]
        public bool CanBuyFromVendor = true;

        [SerializeField]
        public float BuyRarityValue = 100;
        
        [SerializeField]
        public float CurrencyMultiplier = 1f;
       
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoValidate(GameDataBuildValidationContext context)
        {
            base.DoValidate(context);

            if (this.CanDrop && this.DropRarityValue <= 0)
            {
                context.Error(this, this, null, "Drop Rarity value is invalid");
            }
            
            if (this.CanBuyFromVendor && this.BuyRarityValue <= 0)
            {
                context.Error(this, this, null, "Buy Rarity value is invalid");
            }
        }

        protected override void DoBuild(GameDataBuildContext context)
        {
            base.DoBuild(context);
            
            var runtime = new RuntimeRarityData
            {
                ColorValues = this.Color.ToArray(),
                MinLevel = this.MinLevel,
                Rarity = this.Rarity,
                CanDrop = this.CanDrop,
                DropRarityValue = this.CanDrop ? this.DropRarityValue : 0,
                CanBuyFromVendor = this.CanBuyFromVendor,
                BuyRarityValue = this.CanBuyFromVendor ? this.BuyRarityValue : 0,
                CurrencyMultiplier = this.CurrencyMultiplier
            };

            this.BuildBase(context, runtime);

            context.AddBuildResult(runtime);
        }
    }
}
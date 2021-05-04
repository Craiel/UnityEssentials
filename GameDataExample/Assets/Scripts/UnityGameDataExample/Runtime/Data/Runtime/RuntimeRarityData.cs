namespace UnityGameDataExample.Runtime.Data.Runtime
{
    using System;
    using Craiel.UnityEssentials.Runtime.Extensions;
    using Craiel.UnityGameData.Runtime;
    using Enums;
    using UnityEngine;

    [Serializable]
    public class RuntimeRarityData : RuntimeGameData
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public uint MinLevel;

        [SerializeField]
        public Rarity Rarity;
        
        [SerializeField]
        public bool CanDrop = true;
        
        [SerializeField]
        public float DropRarityValue;

        [SerializeField]
        public bool CanBuyFromVendor = true;

        [SerializeField]
        public float BuyRarityValue;
        
        [SerializeField]
        public float CurrencyMultiplier;

        [SerializeField]
        public float[] ColorValues;

        public Color Color { get; private set; }
        
        public override void PostLoad()
        {
            base.PostLoad();

            if (this.ColorValues == null || this.ColorValues.Length != 4)
            {
                UnityEngine.Debug.LogErrorFormat("Rarity Color has invalid values for {0}", this.Name);
            }
            else
            {
                this.Color = ColorExtensions.FromArray(this.ColorValues);
            }
        }
    }
}

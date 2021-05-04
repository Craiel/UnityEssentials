namespace UnityGameDataExample.Runtime.Data.Runtime
{
    using Craiel.UnityGameData.Runtime;
    using Enums;
    using UnityEngine;

    public class RuntimeTagData : RuntimeGameData
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public RuntimeTagData()
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public GameStateTagType Type;

        [SerializeField]
        public bool IsVisibleTag;
        
        public override void PostLoad()
        {
            base.PostLoad();
        }
    }
}

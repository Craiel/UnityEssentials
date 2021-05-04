namespace Craiel.UnityVFX.Runtime.Data
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityGameData.Runtime;

    [Serializable]
    public class RuntimeVFXData : RuntimeGameData
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public RuntimeVFXData()
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public List<RuntimeVFXNodeData> Nodes;
        // TODO

        public override void PostLoad()
        {
            base.PostLoad();
        }
    }
}

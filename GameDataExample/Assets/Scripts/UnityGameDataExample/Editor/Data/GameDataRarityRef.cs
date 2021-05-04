namespace UnityGameDataExample.Editor.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityGameDataExample.Data.Editor;
    using Craiel.UnityEssentials.Runtime;
    using Craiel.UnityGameData.Editor;

    [Serializable]
    public class GameDataRarityRef : GameDataRefBase
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        public static IList<GameDataRarity> GetAvailable()
        {
            return GameDataHelpers.FindGameDataList(TypeCache<GameDataRarity>.Value).Cast<GameDataRarity>().ToList();
        }
    }
}
namespace UnityGameDataExample.Editor.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Craiel.UnityEssentials.Runtime;
    using Craiel.UnityGameData.Editor;

    [Serializable]
    public class GameDataFactionRef : GameDataRefBase
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        public static IList<GameDataFaction> GetAvailable()
        {
            return GameDataHelpers.FindGameDataList(TypeCache<GameDataFaction>.Value).Cast<GameDataFaction>().ToList();
        }
    }
}
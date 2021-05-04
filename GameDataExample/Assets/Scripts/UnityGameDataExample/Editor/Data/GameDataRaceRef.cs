namespace UnityGameDataExample.Editor.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Craiel.UnityEssentials.Runtime;
    using Craiel.UnityGameData.Editor;

    [Serializable]
    public class GameDataRaceRef : GameDataRefBase
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        public static IList<GameDataRace> GetAvailable()
        {
            return GameDataHelpers.FindGameDataList(TypeCache<GameDataRace>.Value).Cast<GameDataRace>().ToList();
        }
    }
}
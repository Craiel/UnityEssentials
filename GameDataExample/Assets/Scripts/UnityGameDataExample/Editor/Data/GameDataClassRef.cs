namespace UnityGameDataExample.Editor.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Craiel.UnityEssentials.Runtime;
    using Craiel.UnityGameData.Editor;

    [Serializable]
    public class GameDataClassRef : GameDataRefBase
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        public static IList<GameDataClass> GetAvailable()
        {
            return GameDataHelpers.FindGameDataList(TypeCache<GameDataClass>.Value).Cast<GameDataClass>().ToList();
        }
    }
}
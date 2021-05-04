namespace UnityGameDataExample.Editor.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityGameDataExample.Data.Editor;
    using Craiel.UnityEssentials.Runtime;
    using Craiel.UnityGameData.Editor;

    [Serializable]
    public class GameDataTagRef : GameDataRefBase
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        public static IList<GameDataTag> GetAvailable()
        {
            return GameDataHelpers.FindGameDataList(TypeCache<GameDataTag>.Value).Cast<GameDataTag>().ToList();
        }
    }
}

namespace Craiel.UnityAudio.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityGameData.Editor;

    [Serializable]
    public class GameDataAudioRef : GameDataRefBase
    {
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        public static IList<GameDataAudio> GetAvailable()
        {
            return GameDataHelpers.FindGameDataList(typeof(GameDataAudio)).Cast<GameDataAudio>().ToList();
        }
    }
}
namespace UnityGameDataExample.Core.Setup.Editor
{
    using Craiel.GameData.Editor.Contracts;
    using Craiel.UnityAudio.Editor;
    using Craiel.UnityEssentials.Runtime;
    using Craiel.UnityGameData.Editor.Builder;
    using Craiel.UnityGameData.Editor.Window;
    using Craiel.UnityVFX.Editor;
    using Data.Editor;
    using Runtime.Core.Setup;
    using UnityGameDataExample.Editor.Data;

    public class GameDataEditorConfig : IGameDataEditorConfig
    {
        private enum DataWorkspace
        {
            Visual = 1,
            Items,
            Gameplay
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Configure()
        {
            GameDataBuilder.TargetFile = Constants.GameDataDataFile;

            GameDataEditorWindow.ClearWorkspaces();
            GameDataEditorWindow.ClearContent();

            foreach (DataWorkspace workspace in EnumCache<DataWorkspace>.Values)
            {
                GameDataEditorWindow.AddWorkSpace((int)workspace, workspace.ToString());
            }

            GameDataEditorWindow.AddContent<GameDataAudio>("Audio", (int)DataWorkspace.Visual);
            GameDataEditorWindow.AddContent<GameDataVFX>("VFX", (int)DataWorkspace.Visual);
            GameDataEditorWindow.AddContent<GameDataFaction>("Faction", (int)DataWorkspace.Gameplay);
            GameDataEditorWindow.AddContent<GameDataRace>("Race", (int)DataWorkspace.Gameplay);
            GameDataEditorWindow.AddContent<GameDataClass>("Class", (int)DataWorkspace.Gameplay);
            GameDataEditorWindow.AddContent<GameDataTag>("Tag", (int)DataWorkspace.Gameplay);
            GameDataEditorWindow.AddContent<GameDataRarity>("Rarity", (int)DataWorkspace.Items, (int)DataWorkspace.Gameplay);
        }
    }
}

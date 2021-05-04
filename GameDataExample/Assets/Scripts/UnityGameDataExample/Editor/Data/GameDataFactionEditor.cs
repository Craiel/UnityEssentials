namespace UnityGameDataExample.Editor.Data
{
    using Craiel.UnityGameData.Editor.Common;
    using UnityEditor;

    [CustomEditor(typeof(GameDataFaction))]
    [CanEditMultipleObjects]
    public class GameDataFactionEditor : GameDataObjectEditor
    {
        private static bool propertiesFoldout = true;

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoDrawFull()
        {
            this.DrawProperties();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DrawProperties()
        {
            if (!this.DrawFoldout("Faction Properties", ref propertiesFoldout))
            {
                return;
            }

            this.DrawProperty<GameDataFaction>(x => x.InternalType);
            this.DrawProperty<GameDataFaction>(x => x.AvailableInCharacterCreation);
        }
    }
}
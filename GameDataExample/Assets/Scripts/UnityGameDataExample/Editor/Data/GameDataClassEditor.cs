namespace UnityGameDataExample.Editor.Data
{
    using Craiel.UnityGameData.Editor.Common;
    using UnityEditor;

    [CustomEditor(typeof(GameDataClass))]
    [CanEditMultipleObjects]
    public class GameDataClassEditor : GameDataObjectEditor
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
            if (!this.DrawFoldout("Class Properties", ref propertiesFoldout))
            {
                return;
            }

            this.DrawProperty<GameDataClass>(x => x.InternalType);
            this.DrawProperty<GameDataClass>(x => x.AvailableInCharacterCreation);
            
            this.DrawReorderableList<GameDataClass>("Factions", x => x.Factions);
            this.DrawReorderableList<GameDataClass>("Race", x => x.Races);
        }
    }
}
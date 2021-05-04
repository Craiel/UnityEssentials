namespace UnityGameDataExample.Editor.Data
{
    using Craiel.UnityGameData.Editor.Common;
    using UnityEditor;

    [CustomEditor(typeof(GameDataRace))]
    [CanEditMultipleObjects]
    public class GameDataRaceEditor : GameDataObjectEditor
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
            if (!this.DrawFoldout("Race Properties", ref propertiesFoldout))
            {
                return;
            }

            this.DrawProperty<GameDataRace>(x => x.InternalType);
            this.DrawProperty<GameDataRace>(x => x.AvailableInCharacterCreation);
            
            this.DrawReorderableList<GameDataRace>("Factions", x => x.Factions);
        }
    }
}
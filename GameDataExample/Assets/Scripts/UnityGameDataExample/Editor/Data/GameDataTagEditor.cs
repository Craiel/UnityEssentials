namespace UnityGameDataExample.Editor.Data
{
    using UnityGameDataExample.Data.Editor;
    using Craiel.UnityGameData.Editor.Common;
    using UnityEditor;

    [CustomEditor(typeof(GameDataTag))]
    [CanEditMultipleObjects]
    public class GameDataTagEditor : GameDataObjectEditor
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
            if (!this.DrawFoldout("Tag Properties", ref propertiesFoldout))
            {
                return;
            }

            this.DrawProperty<GameDataTag>(x => x.Type);
            this.DrawProperty<GameDataTag>(x => x.IsVisibleTag);
        }
    }
}

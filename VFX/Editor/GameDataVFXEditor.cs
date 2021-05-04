namespace Craiel.UnityVFX.Editor
{
    using UnityEditor;
    using UnityGameData.Editor.Common;

    [CustomEditor(typeof(GameDataVFX))]
    [CanEditMultipleObjects]
    public class GameDataVFXEditor : GameDataObjectEditor
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
            if (this.DrawFoldout("VFX Properties", ref propertiesFoldout))
            {
                // TODO
            }
        }
    }
}
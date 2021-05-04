namespace Craiel.UnityVFX.Editor
{
    using Enums;
    using UnityEssentials.Editor;

    public class VFXEditorConfig : EditorConfig<VFXEditorConfigKeys>
    {
        private const string SaveKey = "gameDataEditor";
        private const int CurrentVersion = 1;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public VFXEditorConfig() 
            : base(SaveKey, CurrentVersion)
        {
        }
    }
}
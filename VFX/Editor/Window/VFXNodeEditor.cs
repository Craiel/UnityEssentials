namespace Craiel.UnityVFX.Editor.Window
{
    using UnityEngine;
    using UnityEssentials.Editor.NodeEditor;

    public class VFXNodeEditor : ScriptableNodeEditor
    {
        private GameDataVFX activeVfx;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public VFXNodeEditor()
        {
            this.ContextMenu = new VFXNodeEditorContextMenu();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Draw(Rect drawArea, GameDataVFX vfxData)
        {
            if (this.activeVfx != vfxData)
            {
                this.activeVfx = vfxData;
                this.Reload();
            }
            
            Draw(drawArea);
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Reload()
        {
            this.Clear();
        }
    }
}
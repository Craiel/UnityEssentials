namespace Craiel.UnityVFX.Editor.Window
{
    using GameData.Editor.Events;
    using UnityEditor;
    using UnityEngine;
    using UnityEssentials.Editor;
    using UnityEssentials.Runtime.Event;
    using UnityEssentials.Runtime.Event.Editor;

    public class VFXEditorWindow : EssentialEditorWindowIM<VFXEditorWindow>
    {
        private VFXNodeEditor nodeEditor;

        private GameDataVFX activeVFX;

        private BaseEventSubscriptionTicket eventGameDataChangedTicket;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public VFXEditorWindow()
        {
            this.nodeEditor =  new VFXNodeEditor();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void OpenWindow()
        {
            OpenWindow("VFX Editor");
        }

        public override void OnEnable()
        {
            base.OnEnable();
            
            VFXEditorCore.Initialize();

            this.eventGameDataChangedTicket = EditorEvents.Subscribe<EditorEventGameDataSelectionChanged>(this.OnGameDataSelectionChanged);
        }

        public override void OnDestroy()
        {
            EditorEvents.Unsubscribe(ref this.eventGameDataChangedTicket);

            this.nodeEditor.Dispose();
            this.nodeEditor = null;
            
            base.OnDestroy();
        }

        public override void OnSelectionChange()
        {
            base.OnSelectionChange();
            
            if (UnityEditor.Selection.objects == null
                || UnityEditor.Selection.objects.Length != 1)
            {
                this.activeVFX = null;
                return;
            }
            
            this.activeVFX = UnityEditor.Selection.objects[0] as GameDataVFX;
            this.Repaint();
        }

        public void OnGUI()
        {
            // Menu Tool Bar
            EditorGUILayout.BeginHorizontal("Toolbar");
            {
                if (EditorGUILayout.DropdownButton(new GUIContent("Edit"), FocusType.Passive, "ToolbarDropDown"))
                {
                    var menu = new GenericMenu();
                    //menu.AddItem(new GUIContent("Validate"), false, ValidateGameData);
                    //menu.AddItem(new GUIContent("Export"), false, ExportGameData);
                    menu.ShowAsContext();
                    Event.current.Use();
                }

                string selectionTitle = this.activeVFX == null ? "<None>" : this.activeVFX.Name;
                if (EditorGUILayout.DropdownButton(new GUIContent("Selected: " + selectionTitle), FocusType.Passive, "ToolbarDropDown"))
                {
                    var menu = new GenericMenu();
                    foreach (GameDataVFX vfx in GameDataVFXRef.GetAvailable())
                    {
                        GameDataVFX closure = vfx;
                        menu.AddItem(new GUIContent(vfx.Name), false, () => this.SelectActiveVFX(closure));
                    }
                    
                    menu.ShowAsContext();
                    Event.current.Use();
                }

                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.EndHorizontal();

            if (this.activeVFX != null)
            {
                Rect contentRect = new Rect(10, 40, position.width - 20, position.height - 50);
                this.nodeEditor.Draw(contentRect, this.activeVFX);
            }

            this.ProcessEvents(Event.current);

            if (GUI.changed)
            {
                Repaint();
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ProcessEvents(Event eventData)
        {
            this.nodeEditor.ProcessEvent(eventData);
        }
        
        private void OnGameDataSelectionChanged(EditorEventGameDataSelectionChanged eventData)
        {
            if (eventData.SelectedObjects == null
                || eventData.SelectedObjects.Length != 1)
            {
                this.activeVFX = null;
                return;
            }

            this.activeVFX = eventData.SelectedObjects[0] as GameDataVFX;
            this.Repaint();
        }
        
        private void SelectActiveVFX(GameDataVFX vfxData)
        {
            this.activeVFX = vfxData;
            UnityEditor.Selection.objects = new[] {vfxData};
            this.Repaint();
        }
    }
}
using IVFXEditorComponent = Craiel.GameData.Editor.Contracts.VFXShared.IVFXEditorComponent;
using IVFXEditorComponentFactory = Craiel.UnityGameData.Editor.Contracts.VFXShared.IVFXEditorComponentFactory;
using VFXEditorComponentDescriptor = Craiel.UnityGameData.VFXShared.VFXEditorComponentDescriptor;

namespace Craiel.UnityVFX.Editor.Window
{
    using System.Collections.Generic;
    using Events;
    using UnityEngine;
    using UnityEssentials.Editor;
    using UnityEssentials.Runtime.Event;
    using UnityEssentials.Runtime.Event.Editor;

    public class VFXNodeEditorContextMenu : DynamicContextMenu
    {
        private BaseEventSubscriptionTicket eventVfxComponentsChangedTicket;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public VFXNodeEditorContextMenu()
        {
            this.eventVfxComponentsChangedTicket = EditorEvents.Subscribe<EditorEventVFXComponentsChanged>(this.OnVFXComponentsChanged);

            this.RebuildMenu();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                EditorEvents.Unsubscribe(ref this.eventVfxComponentsChangedTicket);
            }
            
            base.Dispose(isDisposing);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnVFXComponentsChanged(EditorEventVFXComponentsChanged eventdata)
        {
            this.RebuildMenu();
        }

        private void RebuildMenu()
        {
            this.Clear();

            var sortedDescriptors = new Dictionary<string, IList<VFXEditorComponentDescriptor>>();
            foreach (IVFXEditorComponentFactory factory in VFXEditorCore.ComponentFactories)
            {
                foreach (VFXEditorComponentDescriptor descriptor in factory.AvailableComponents)
                {
                    if (!sortedDescriptors.ContainsKey(descriptor.Category))
                    {
                        sortedDescriptors.Add(descriptor.Category, new List<VFXEditorComponentDescriptor>());
                    }
                    
                    sortedDescriptors[descriptor.Category].Add(descriptor);
                }
            }

            foreach (string categories in sortedDescriptors.Keys)
            {
                foreach (VFXEditorComponentDescriptor descriptor in sortedDescriptors[categories])
                {
                    this.RegisterAction(descriptor.Name, () => this.OnCreateComponent(descriptor), group: descriptor.Category);
                }
            }
        }

        private void OnCreateComponent(VFXEditorComponentDescriptor descriptor)
        {
            // TODO: Position
            IVFXEditorComponent entry = descriptor.Factory.CreateNew(descriptor, Vector2.zero);
            // TODO: Register with the vfx being edited
        }
    }
}
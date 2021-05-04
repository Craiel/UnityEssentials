using IVFXEditorComponent = Craiel.GameData.Editor.Contracts.VFXShared.IVFXEditorComponent;
using VFXEditorComponentDescriptor = Craiel.UnityGameData.VFXShared.VFXEditorComponentDescriptor;
using VFXShared_IVFXEditorComponentFactory = Craiel.UnityGameData.Editor.Contracts.VFXShared.IVFXEditorComponentFactory;

namespace Craiel.UnityVFX.Editor.Components
{
    using System.Collections.Generic;
    using UnityEngine;

    public class VFXIntegratedEditorComponentFactory : VFXShared_IVFXEditorComponentFactory
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public VFXIntegratedEditorComponentFactory()
        {
            this.AvailableComponents = new List<VFXEditorComponentDescriptor>
            {
                new VFXEditorComponentDescriptor(this)
                {
                    Type = typeof(VFXIntegratedEditorComponentParticle),
                    Category = VFXEditorConstants.IntegratedComponentCategory,
                    Name = "Particle"
                },
                new VFXEditorComponentDescriptor(this)
                {
                    Type = typeof(VFXIntegratedEditorComponentPrefab),
                    Category = VFXEditorConstants.IntegratedComponentCategory,
                    Name = "Prefab"
                }
            };
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<VFXEditorComponentDescriptor> AvailableComponents { get; private set; }
        
        public IVFXEditorComponent CreateNew(VFXEditorComponentDescriptor descriptor, Vector2 position)
        {
            var instance = (IVFXEditorComponent)System.Activator.CreateInstance(descriptor.Type);
            instance.Position = position;
            instance.Name = string.Format("New " + descriptor.Name);
            
            return instance;
        }
    }
}
namespace Craiel.UnityVFX.Editor
{
    using System;
    using System.Collections.Generic;
    using Components;
    using Contracts;
    using Events;
    using UnityEssentials.Runtime.Component;
    using UnityEssentials.Runtime.Event.Editor;
    using UnityGameData.Editor.Contracts.VFXShared;

    public class VFXEditorCore
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static VFXEditorCore()
        {
            ComponentFactories = new List<IVFXEditorComponentFactory>();
            
            // Add the integrated factory
            AddComponent(new VFXIntegratedEditorComponentFactory());
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool IsInitialized { get; private set; }

        public static VFXEditorConfig Config { get; private set; }
        
        public static IList<IVFXEditorComponentFactory> ComponentFactories { get; private set; }

        public static void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            Config = new VFXEditorConfig();
            Config.Load(); 

            new CraielComponentConfigurator<IVFXEditorConfig>().Configure();
            
            IsInitialized = true;
        }

        public static void AddComponent(IVFXEditorComponentFactory componentFactory)
        {
            if (ComponentFactories.Contains(componentFactory))
            {
                throw new InvalidOperationException("VFX Component Factory was already registered!");
            }
            
            ComponentFactories.Add(componentFactory);
            EditorEvents.Send(new EditorEventVFXComponentsChanged());
        }
    }
}
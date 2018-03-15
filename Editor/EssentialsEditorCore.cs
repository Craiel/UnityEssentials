using IEssentialEditorConfig = Craiel.UnityEssentials.Contracts.IEssentialEditorConfig;

namespace Craiel.UnityEssentials.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class EssentialsEditorCore
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool IsInitialized { get; private set; }

        public static void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            Type configType = typeof(IEssentialEditorConfig);
            IList<Type> implementations = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsInterface || !configType.IsAssignableFrom(type))
                        {
                            continue;
                        }

                        implementations.Add(type);
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogErrorFormat("Could not load types for assembly {0}: {1}", assembly, e);
                }
            }

            if (implementations.Count != 1)
            {
                UnityEngine.Debug.LogError("No implementation of IEssentialEditorConfig found, configure your game data first");
                return;
            }

            var config = Activator.CreateInstance(implementations.First()) as IEssentialEditorConfig;
            if (config == null)
            {
                UnityEngine.Debug.LogError("Failed to instantiate config class");
                return;
            }

            config.Configure();

            IsInitialized = true;
        }
    }
}

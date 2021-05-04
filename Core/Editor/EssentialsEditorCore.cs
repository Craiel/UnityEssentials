namespace Craiel.UnityEssentials.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Runtime;
    using Runtime.Contracts;

    public static class EssentialsEditorCore
    {
        private static readonly Type ConfigType = TypeCache<IEssentialEditorConfig>.Value;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool IsInitialized { get; private set; }

        public static void Configure(bool reconfigure = false)
        {
            if (IsInitialized && !reconfigure)
            {
                return;
            }

            IList<Type> implementations = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsInterface || !ConfigType.IsAssignableFrom(type))
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

            if (implementations.Count == 0)
            {
                UnityEngine.Debug.LogError($"No implementation of {ConfigType.Name} found, configure your game data first");
                return;
            }

            // Remove the default configuration if we have more than one
            if (implementations.Count > 1)
            {
                implementations.Remove(TypeCache<EssentialsDefaultConfig>.Value);
            }
            else
            {
                if (implementations[0] == TypeCache<EssentialsDefaultConfig>.Value)
                {
                    UnityEngine.Debug.LogWarning($"Using Default {ConfigType.Name}");
                }
            }

            if (implementations.Count > 1)
            {
                UnityEngine.Debug.LogWarning($"Multiple implementations of {ConfigType.Name} found, using first");
            }

            var config = Activator.CreateInstance(implementations[0]) as IEssentialEditorConfig;
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

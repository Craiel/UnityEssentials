namespace Craiel.UnityEssentials.Runtime.Component
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Contracts;

    public class CraielComponentConfigurator<T>
        where T : class, ICraielComponentConfig
    {
        private static readonly string[] IgnoredAssemblies =
        {
            "mscorlib",
            "UnityEngine",
            "UnityEditor",
            "Unity.",
            "System.",
            "Assembly-",
            "Mono.",
            "nunit.",
            "Demi",
            "DOTWeen",
            "Rewired",
            "NLog",
            "Jetbrains",
            "YamlDotNet",
            "LiteDB",
            "netstandard",
            "ExCSS",
            "Microsoft.",
            "Accessibility"
        };

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public void Configure()
        {
            try
            {
                this.Initialize();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogErrorFormat("Failed to initialize Component Config For {0}: {1}", TypeCache<T>.Value, e);
                throw;
            }

        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Initialize()
        {
            Type configType = TypeCache<T>.Value;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            IList<Type> implementations = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                bool ignoreAssembly = false;
                for (var i = 0; i < IgnoredAssemblies.Length; i++)
                {
                    if (assembly.FullName.StartsWith(IgnoredAssemblies[i], StringComparison.OrdinalIgnoreCase))
                    {
                        ignoreAssembly = true;
                        break;
                    }
                }

                if (ignoreAssembly)
                {
                    // UnityEngine.Debug.LogFormat("ComponentConfigure: Ignoring {0}", assembly.FullName);
                    continue;
                }

#if DEBUG
                // UnityEngine.Debug.LogFormat("ComponentConfigure: Scanning {0}", assembly.FullName);
#endif

                try
                {
                    Type[] candidates = assembly.GetTypes();
                    foreach (Type candidate in candidates)
                    {
                        if (candidate.IsInterface || !configType.IsAssignableFrom(candidate))
                        {
                            continue;
                        }

                        implementations.Add(candidate);
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogWarningFormat("Failed to GetTypes for Assembly {0}: {1}", assembly, e);
                }
            }

            if (implementations.Count != 1)
            {
                EssentialsCore.Logger.Error("No implementation of {0} found, configure your game data first", TypeCache<T>.Value);
                return;
            }

            T config = Activator.CreateInstance(implementations.First()) as T;
            if (config == null)
            {
                EssentialsCore.Logger.Error("Failed to instantiate config class of type {0}", TypeCache<T>.Value);
                return;
            }

            config.Configure();
        }
    }
}
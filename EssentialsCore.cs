namespace Craiel.UnityEssentials
{
    using System;
    using System.Linq;
    using Contracts;
    using Input;
    using IO;
    using NLog;
    using UnityEngine;

    public static class EssentialsCore
    {
        public static readonly Color DefaultGizmoColor = Color.black;
        
        public const string ResourcesFolderName = "Resources";

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly ManagedDirectory AssetsPath = new ManagedDirectory("Assets");

        public static readonly ManagedDirectory DefaultScenesPath = AssetsPath.ToDirectory("Scenes");

        private static ManagedDirectory dataPath;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static EssentialsCore()
        {
            LocalizationSaveInterval = 60f;

            DefaultInputState = InputStateDefault.Instance;

            CoreScenes = new ManagedFile[0];
            
            ScenesPath = DefaultScenesPath;

            Initialize();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static IInputState DefaultInputState { get; set; }

        public static float LocalizationSaveInterval { get; set; }

        public static ManagedFile[] CoreScenes { get; set; }

        public static ManagedDirectory DataPath
        {
            get { return dataPath ?? (dataPath = new ManagedDirectory(Application.dataPath)); }
        }

        public static ManagedDirectory ScenesPath { get; set; }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void Initialize()
        {
            Type configType = typeof(IEssentialConfig);
            var implementations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => !x.IsInterface && configType.IsAssignableFrom(x))
                .ToList();

            if (implementations.Count != 1)
            {
                Logger.Error("No implementation of IEssentialConfig found, configure your game data first");
                return;
            }

            var config = Activator.CreateInstance(implementations.First()) as IEssentialConfig;
            if (config == null)
            {
                Logger.Error("Failed to instantiate config class");
                return;
            }

            config.Configure();
        }
    }
}

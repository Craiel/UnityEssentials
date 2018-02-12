namespace Assets.Scripts.Craiel.Essentials
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

        public static readonly CarbonDirectory AssetsPath = new CarbonDirectory("Assets");

        public static readonly CarbonDirectory DefaultScenesPath = AssetsPath.ToDirectory("Scenes");

        private static CarbonDirectory dataPath;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static EssentialsCore()
        {
            LocalizationSaveInterval = 60f;

            DefaultInputState = InputStateDefault.Instance;

            CoreScenes = new CarbonFile[0];
            
            ScenesPath = DefaultScenesPath;

            Initialize();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static IInputState DefaultInputState { get; set; }

        public static float LocalizationSaveInterval { get; set; }

        public static CarbonFile[] CoreScenes { get; set; }

        public static CarbonDirectory DataPath
        {
            get { return dataPath ?? (dataPath = new CarbonDirectory(Application.dataPath)); }
        }

        public static CarbonDirectory ScenesPath { get; set; }

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

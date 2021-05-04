namespace Craiel.UnityEssentials.Runtime
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
        public static readonly int ResourceFolderNameSize = ResourcesFolderName.Length;

        public const string PrefabExtension = ".prefab";

        public static readonly ManagedDirectory AssetsPath = new ManagedDirectory("Assets");

        public static readonly ManagedDirectory ResourcesPath = AssetsPath.ToDirectory(ResourcesFolderName);

        public static readonly ManagedDirectory DefaultScenesPath = AssetsPath.ToDirectory("Scenes");

        public static readonly NLog.Logger Logger = LogManager.GetLogger("CRAIEL_ESSENTIALS");

        public static readonly ManagedDirectory PersistentDataPath = new ManagedDirectory(Application.persistentDataPath);
        public static readonly ManagedDirectory DefaultSavePath = PersistentDataPath.ToDirectory("Save");
        public const string DefaultSavePrefix = "esv_";

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
            Type configType = TypeCache<IEssentialConfig>.Value;
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

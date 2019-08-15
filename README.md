# Craiel Unity Essentials

Essential Library for creating Games with Unity.

### Provides the following Features

#### Editor
- Base classes for Editor Windows and Inspectors providing templated access to Serialized Objects
- Transform and RectTransform Extensions
- Extended Color Inspector
- Managed Directory and File path handling with cross platform support (and unity internal paths)
- Directory Utilities for finding and removing empty directories in the Project
- NLog Logging support with a custom NLog Console for better log output with all features NLog offers
- Game Event System with ticketing (Runtime + Editor)
- Build Generation Utilities
- Utility Window to search for components used in the project (and find un-used components)
- Hot Reload Guard that will stop unity if compilation occurs
- Several Attribute Drawers with builtin functionality (Read Only, Enum Flags, Angles, Progress Bar etc)
- Pixel Perfect Camera Editor
- Serializable TimeSpan + DateTime
- Basic Node Editor Framework + Window
- Reorderable List
- IMGUI utilities for Layout and Styles

#### Runtime
- FNV Hash Helpers
- Non-Blocking Semaphores
- Full Behavior Tree Implementation with common features and Behavior Stream Extension
- Several Specialized Collections (Circular Buffer, Extended Dictionary, Default Value Dictionary, Priority Queue etc)
- Free Movement and Mouse Look Controllers ready to use
- Serialized Binary Tag Data (SBT) for efficient and fast serialization of data to binary streams
- "Engine" Core to bootstrap projects with the most common modules
- Save / Load Module with File and PlayerPrefs support
- Ticket Providers
- Several Extenion Methods for commonly used objects (Bounds, Color, GameObject, Rect, String etc)
- Quaternion Compression support for efficient network transfer
- Customizable text Formatter with variable support
- Finite State Machine implementation
- Model and Geometry utilities (Dynamic Mesh, Obj import and export etc)
- Full Extendable Grammar Parser implementation with several built-in grammars (Command line, Java, SQL)
- I18N Localization support classes
- Basic Input system based on unity input mapping
- Random Distribution Algorithms (Constant, Gaussian, Triangular, Uniform etc)
- Basic Noise Provider with several algorithms (Cellular, Cubic, Fractal, Perlin & Simplex)
- Pooling base classes for objects, behaviors and support for Jobs
- Basic runtime FPS Profiling graph
- Resource Loading Utilities with support for Asset bundles, soft and hard references, caching of resources, pre-loading and loading at runtime (User Generated Content)
- Scene Transition and organization utilities
- Singleton Implementations (Static + Behaviours)
- Spatial Utilities (Hyper Rect, KDTree + Octree)
- Threading support (Native C# Thread, not Job system) with dispatchers, synchronization and custom timing
- Lite Tweening support
- Yaml Fluent Serializer and Deserializer

## Getting Started

Add the package and dependencies to your Project Manifest.json:
```
{
    "dependencies": {
    ...
    "com.craiel.unity.essentials": "https://github.com/Craiel/UnityEssentials.git"
    ...
  }
}
```


### Configuration

Most things in the Library do not require any configuration, you can start using them right away. All custom inspectors and extensions will start to take effect immediatly after being added to the Project.

If you plan to use the EngineCore + SceneTransition you will have to configure the Library by adding a class implementing `IAudioConfig` in the project:

```
    [UsedImplicitly]
    public class EssentialsConfig : IEssentialConfig
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Configure()
        {
            IList<ManagedFile> coreScenes = new List<ManagedFile>();
            foreach (GameSceneType sceneType in EnumValues.GameSceneTypeValues)
            {
                coreScenes.Add(EssentialsCore.ScenesPath.ToFile(sceneType + EssentialsConstants.UnitySceneExtension));
            }
    
            EssentialsCore.CoreScenes = coreScenes.ToArray();
        }
    }
```

GameSceneType is an enum that functions as a mapping to the main scenes of the game, this is used to facilitate the general transition of scenes during a normal game process.

```
    public enum GameSceneType
    {
        Intro,
        MainMenu,
        LocalMap,
        WorldMap
    }
```


### Usage

To setup the project to use the Library core create a class inheriting from `EssentialEngineCore<T, GameSceneType>`:

```
    public class GameCore : EssentialEngineCore<GameCore, GameSceneType>
    {
        public GameCore()
        {
            this.RegisterSceneImplementation<SceneIntro>(GameSceneType.Intro);
            this.RegisterSceneImplementation<SceneMainMenu>(GameSceneType.MainMenu);
            this.RegisterSceneImplementation<SceneLocalMap>(GameSceneType.LocalMap);
            this.RegisterSceneImplementation<SceneWorldMap>(GameSceneType.WorldMap);
        }

        protected override void InitializeGameComponents()
        {
            // If you are using the Craiel GameData package:
            //GameRuntimeData.InstantiateAndInitialize();
            //GameRuntimeData.Instance.Load(ResourceKey.Create<TextAsset>("gamedata.bytes"));
            
            // If you are using Craiel Audio
            //AudioSystem.InstantiateAndInitialize();
            
            GameModules.InstantiateAndInitialize();
            
            UIEventSystemMonitor.InstantiateAndInitialize();

            // Hide the mouse cursor, we won't need it
            Cursor.visible = false;
            
            GameCore.Instance.Transition(GameSceneType.Intro);
        }
    }
```

the call to `GameCore.Instance.Transition` functions as the global way to start a Scene Transition, this will start a loading process with several stages that can be used to pre-load assets or do other operations.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.


## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

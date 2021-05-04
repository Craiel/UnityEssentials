# Craiel Unity Audio

Audio System using Craiel GameData with support for Localized and Attached Emitters, Audio Source Pooling, UI Audio Events, Fading and Multiple Clips in sequence or random.

## Getting Started

Add the package and dependencies to your Project Manifest.json:
```
{
    "dependencies": {
    ...
    "com.craiel.unity.essentials": "https://github.com/Craiel/UnityEssentials.git",
    "com.craiel.unity.gamedata": "https://github.com/Craiel/UnityGameData.git",
    "com.craiel.unity.audio": "https://github.com/Craiel/UnityAudio.git",
    ...
  }
}
```


### Prerequisites
 
- https://github.com/Craiel/UnityEssentials
- https://github.com/Craiel/UnityGameData


### Configuration

Before use the Audio System will have to be configured and integrated into GameData.
To Configure the system add a class implementing `IAudioConfig` in the project:

```
    [UsedImplicitly]
    public class AudioConfig : IAudioConfig
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Configure()
        {
            AudioCore.DynamicAudioSourceResource = ResourceKey.Create<GameObject>("Audio/DynamicAudioSource");
            AudioCore.MasterMixerResource = ResourceKey.Create<AudioMixer>("Audio/MasterMixer");
            AudioCore.AudioEventMappingResource = ResourceKey.Create<AudioEventMapping>("Audio/EventMapping");
        }
    }
```

- DynamicAudioSource is a Prefab that contains the Audio Source object and the `DynamicAudioSource` `MonoBehaviour`
- MasterMixer is the Unity `MasterMixer` to use
- AudioEventMapping is a Scriptable Object (Create -> Craiel -> Audio -> EventMapping) that maps audio events to audio game data entries

To Register the Audio System with GameData add it to the `IGameDataEditorConfig` script:

```
GameDataEditorWindow.AddContent<GameDataAudio>("Audio");
```

### Usage

To play audio call `AudioSystem` with the corresponding data id:

```
AudioSystem.Instance.Play(this.audioDataId);
```

There are several different functions to play audio depending on the situation as well as corresponding Stop functions.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.


## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

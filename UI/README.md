# Craiel Unity UI Essentials

UI Extension for the Craiel Essentials Project.
Provides Theming Support and other transition helpers for more dynamic UI Elements based on uGUI.

## Getting Started

Add the package and dependencies to your Project Manifest.json:
```
{
    "dependencies": {
    ...
    "com.craiel.unity.essentials": "https://github.com/Craiel/UnityEssentials.git",
    "com.craiel.unity.essentials.ui": "https://github.com/Craiel/UnityEssentialsUI.git",
    ...
  }
}
```

### Usage

To use the Theme Library create a ColorScheme Scriptable object (Create -> Craiel -> UI -> ColorScheme).
Then add the `UIColorSchemeSystem` to your ui hierarchy and set it to auto instantiate.

Now you can use it by adding an `UIColorSchemeElement` behaviour to your UI Elements that you want to theme. simply select the shade value and it will apply the theme at editor and runtime.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.


## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

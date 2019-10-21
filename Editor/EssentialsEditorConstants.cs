using Craiel.UnityEssentials.Runtime.IO;

namespace Craiel.UnityEssentials.Editor
{
    public static class EssentialsEditorConstants
    {
        public const string MenuRoot = "Window/Craiel/";

        public const string PackageName = "com.craiel.unity.essentials";

        public static readonly ManagedDirectory BasePath = new ManagedDirectory("packages").ToDirectory(PackageName);
        public static readonly ManagedDirectory UIPath = BasePath.ToDirectory("Editor", "UI");
    }
}
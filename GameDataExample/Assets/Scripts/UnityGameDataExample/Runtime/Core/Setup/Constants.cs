namespace UnityGameDataExample.Runtime.Core.Setup
{
    using Craiel.UnityEssentials.Runtime;
    using Craiel.UnityEssentials.Runtime.IO;
    using Craiel.UnityEssentials.Runtime.Resource;
    using UnityEngine;

    public static class Constants
    {
        public static readonly ManagedDirectory ResourcesFolder = new ManagedDirectory(EssentialsCore.ResourcesFolderName);
        public static readonly ManagedDirectory ResourcesPath = EssentialsCore.AssetsPath.ToDirectory(ResourcesFolder);
        
        public const string GameDataFileName = "gamedata";
        public const string GameDataExtension = ".bytes";
        
        public const int LogFileArchiveSize = 10485760;
        public const int LogFileArchiveCount = 10;
        
        public static readonly ManagedFile GameDataDataFile = ResourcesPath.ToFile(GameDataFileName + GameDataExtension);
        public static readonly ResourceKey GameDataResourceKey = ResourceKey.Create<TextAsset>(GameDataFileName);
    }
}
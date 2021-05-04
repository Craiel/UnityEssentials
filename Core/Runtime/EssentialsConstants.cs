namespace Craiel.UnityEssentials.Runtime
{
    public static class EssentialsConstants
    {
        public const int LogFileArchiveSize = 10485760;
        public const int LogFileArchiveCount = 10;
        
        public const string LocalizationIgnoreString = "XX_";

        public const string UnitySceneExtension = ".unity";
        
#if UNITY_EDITOR
        public static readonly bool HasPro = UnityEditorInternal.InternalEditorUtility.HasPro();
#endif
    }
}
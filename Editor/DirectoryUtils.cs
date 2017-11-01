namespace Assets.Scripts.Craiel.Essentials.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using IO;
    using NLog;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public class DirectoryUtils : UnityEditor.AssetModificationProcessor
    {
        public static readonly CarbonDirectory DataPath = new CarbonDirectory(Application.dataPath);

        // return: Is this directory empty?
        private const string CleanOnSaveKey = "k1";

        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static event Action OnAutoClean;

        public static bool CleanOnSaveEnabled
        {
            get
            {
                return EditorPrefs.GetBool(CleanOnSaveKey, true);
            }

            set
            {
                EditorPrefs.SetBool(CleanOnSaveKey, value);
            }
        }

        public static string[] OnWillSaveAssets(string[] paths)
        {
            if (CleanOnSaveEnabled)
            {
                List<CarbonDirectory> emptyDirectories = new List<CarbonDirectory>();
                FillEmptyDirList(emptyDirectories);
                if (emptyDirectories.Count > 0)
                {
                    DeleteAllEmptyDirAndMeta(emptyDirectories);

                    Logger.Info("[Clean] Cleaned Empty Directories on Save");

                    if (OnAutoClean != null)
                    {
                        OnAutoClean();
                    }
                }
            }

            return paths;
        }

        public static void FillEmptyDirList(List<CarbonDirectory> target)
        {
            var assetDir = new CarbonDirectory(Application.dataPath);

            WalkDirectoryTree(target, assetDir);
        }

        public static void DeleteAllEmptyDirAndMeta(List<CarbonDirectory> directoryList)
        {
            CarbonDirectory currentDirectory = new CarbonDirectory(System.IO.Directory.GetCurrentDirectory());
            foreach (CarbonDirectory info in directoryList)
            {
                CarbonDirectory relative = info.ToRelative<CarbonDirectory>(currentDirectory);
                AssetDatabase.MoveAssetToTrash(relative.GetUnityPath());
            }

            directoryList.Clear();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static bool CheckDirectoryEmpty(List<CarbonDirectory> target, CarbonDirectory directory, bool areSubDirectoriesEmpty)
        {
            bool isDirEmpty = areSubDirectoriesEmpty && !DirectoryHasFile(directory);
            if (isDirEmpty)
            {
                target.Add(directory);
            }

            return isDirEmpty;
        }
        
        // return: Is this directory empty?
        private static bool WalkDirectoryTree(List<CarbonDirectory> target, CarbonDirectory root)
        {
            CarbonDirectoryResult[] subDirectories = root.GetDirectories();

            bool areSubDirsEmpty = true;
            foreach (CarbonDirectoryResult result in subDirectories)
            {
                if (!WalkDirectoryTree(target, result.Absolute))
                {
                    areSubDirsEmpty = false;
                }
            }

            bool isRootEmpty = CheckDirectoryEmpty(target, root, areSubDirsEmpty);
            return isRootEmpty;
        }

        private static bool DirectoryHasFile(CarbonDirectory directory)
        {
            try
            {
                CarbonFileResult[] files = directory.GetFiles();
                return files.Any(x => !IsMetaFile(x.Absolute));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to check directory contents");
            }

            return false;
        }
        
        private static bool IsMetaFile(CarbonFile file)
        {
            return file.Extension.Equals(".meta", StringComparison.OrdinalIgnoreCase);
        }
    }
}

using ManagedDirectory = Craiel.UnityEssentials.Runtime.IO.ManagedDirectory;
using ManagedDirectoryResult = Craiel.UnityEssentials.Runtime.IO.ManagedDirectoryResult;
using ManagedFile = Craiel.UnityEssentials.Runtime.IO.ManagedFile;
using ManagedFileResult = Craiel.UnityEssentials.Runtime.IO.ManagedFileResult;

namespace Craiel.UnityEssentials.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NLog;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public class DirectoryUtils : UnityEditor.AssetModificationProcessor
    {
        public static readonly ManagedDirectory DataPath = new ManagedDirectory(Application.dataPath);

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
                List<ManagedDirectory> emptyDirectories = new List<ManagedDirectory>();
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

        public static void FillEmptyDirList(List<ManagedDirectory> target)
        {
            var assetDir = new ManagedDirectory(Application.dataPath);

            WalkDirectoryTree(target, assetDir);
        }

        public static void DeleteAllEmptyDirAndMeta(List<ManagedDirectory> directoryList)
        {
            ManagedDirectory currentDirectory = new ManagedDirectory(System.IO.Directory.GetCurrentDirectory());
            foreach (ManagedDirectory info in directoryList)
            {
                ManagedDirectory relative = info.ToRelative<ManagedDirectory>(currentDirectory);
                AssetDatabase.MoveAssetToTrash(relative.GetUnityPath());
            }

            directoryList.Clear();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static bool CheckDirectoryEmpty(List<ManagedDirectory> target, ManagedDirectory directory, bool areSubDirectoriesEmpty)
        {
            bool isDirEmpty = areSubDirectoriesEmpty && !DirectoryHasFile(directory);
            if (isDirEmpty)
            {
                target.Add(directory);
            }

            return isDirEmpty;
        }
        
        // return: Is this directory empty?
        private static bool WalkDirectoryTree(List<ManagedDirectory> target, ManagedDirectory root)
        {
            ManagedDirectoryResult[] subDirectories = root.GetDirectories();

            bool areSubDirsEmpty = true;
            foreach (ManagedDirectoryResult result in subDirectories)
            {
                if (!WalkDirectoryTree(target, result.Absolute))
                {
                    areSubDirsEmpty = false;
                }
            }

            bool isRootEmpty = CheckDirectoryEmpty(target, root, areSubDirsEmpty);
            return isRootEmpty;
        }

        private static bool DirectoryHasFile(ManagedDirectory directory)
        {
            try
            {
                ManagedFileResult[] files = directory.GetFiles();
                return files.Any(x => !IsMetaFile(x.Absolute));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to check directory contents");
            }

            return false;
        }
        
        private static bool IsMetaFile(ManagedFile file)
        {
            return file.Extension.Equals(".meta", StringComparison.OrdinalIgnoreCase);
        }
    }
}

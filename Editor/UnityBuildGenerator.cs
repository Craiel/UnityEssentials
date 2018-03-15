namespace Craiel.UnityEssentials.Editor
{
    using System.Linq;
    using UnityEditor;

    public static class UnityBuildGenerator
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void BuildWindowsClientDebug()
        {
            var context = new UnityBuildContext();
            BuildClient(context);
        }

        public static void BuildWindowsClientRelease()
        {
            var context = new UnityBuildContext
            {
                IsReleaseBuild = true
            };

            BuildClient(context);
        }

        public static void BuildAndroidClientDebug()
        {
            var context = new UnityBuildContext
            {
                BuildTarget = BuildTarget.Android,
                BuildTargetGroup = BuildTargetGroup.Android
            };

            BuildClient(context);
        }

        public static void BuildAndroidClientRelease()
        {
            var context = new UnityBuildContext
            {
                BuildTarget = BuildTarget.Android,
                BuildTargetGroup = BuildTargetGroup.Android,
                IsReleaseBuild = true
            };

            BuildClient(context);
        }

        public static void BuildClient(UnityBuildContext context)
        {
            if (!CommonPreBuildSteps(context))
            {
                return;
            }

            if (!Step5ExecuteBuild(context))
            {
                return;
            }
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static bool CommonPreBuildSteps(UnityBuildContext context)
        {
            if (!Step1CheckContext(context))
            {
                return false;
            }

            if (!Step2PrepareForBuild(context))
            {
                return false;
            }

            if (!Step3ExecuteBuildCallbacks(context))
            {
                return false;
            }

            if (!Step4SetBuildTarget(context))
            {
                return false;
            }

            return true;
        }

        private static bool Step1CheckContext(UnityBuildContext context)
        {
            if (context.TargetDirectory == null || context.TargetDirectory.IsNull)
            {
                UnityEngine.Debug.LogError("[UBG] Build Target directory is invalid!");
                return false;
            }

            if (context.StartupScene == null || context.StartupScene.IsNull)
            {
                UnityEngine.Debug.LogError("[UBG] Startup Scene is invalid!");
            }

            return true;
        }

        private static bool Step2PrepareForBuild(UnityBuildContext context)
        {
            UnityEngine.Debug.Log("[UBG] Refreshing AssetDatabase");
            AssetDatabase.Refresh();

            UnityEngine.Debug.Log("[UBG] Creating Build Directory");
            context.TargetDirectory.Create();

            if (!context.TargetDirectory.Exists)
            {
                UnityEngine.Debug.LogErrorFormat("Could not create Build target directory: {0}", context.TargetDirectory);
                return false;
            }

            return true;
        }

        private static bool Step3ExecuteBuildCallbacks(UnityBuildContext context)
        {
            return true;
        }

        private static bool Step4SetBuildTarget(UnityBuildContext context)
        {
            if (EditorUserBuildSettings.activeBuildTarget != context.BuildTarget)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(context.BuildTargetGroup, context.BuildTarget);
            }

            return true;
        }

        private static bool Step5ExecuteBuild(UnityBuildContext context)
        {
            BuildOptions options = BuildOptions.None;
            if (!context.IsReleaseBuild)
            {
                options |= BuildOptions.Development;
            }
            else
            {
                options |= BuildOptions.CompressWithLz4HC;
            }

            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = context.ScenesToIncludeInBuild.Select(x => x.GetUnityPath()).ToArray(),
                options = options,
                target = context.BuildTarget,
                targetGroup = context.BuildTargetGroup,
                locationPathName = context.TargetDirectory.ToFile(context.TargetFileName).GetPath()
            };

            BuildPipeline.BuildPlayer(buildOptions);
            return true;
        }
    }
}

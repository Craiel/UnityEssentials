using ManagedDirectory = Craiel.UnityEssentials.Runtime.IO.ManagedDirectory;
using ManagedFile = Craiel.UnityEssentials.Runtime.IO.ManagedFile;

namespace Craiel.UnityEssentials.Editor
{
    using System.Collections.Generic;
    using Runtime;
    using UnityEditor;
    using UnityEngine;

    public class UnityBuildContext
    {
        private readonly IList<ManagedFile> additionalScenesToInclude;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public UnityBuildContext()
        {
            this.BuildTarget = BuildTarget.StandaloneWindows64;
            this.BuildTargetGroup = BuildTargetGroup.Standalone;

            // Set some sensible defaults
            this.TargetDirectory = EssentialsCore.DataPath.GetParent().ToDirectory("Build");
            this.SetTargetName(Application.productName);

            this.SceneBasePath = EssentialsCore.ScenesPath;

            this.IncludeCoreScenes = true;
            
            this.ScenesToIncludeInBuild = new List<ManagedFile>();
            this.additionalScenesToInclude = new List<ManagedFile>();

            this.SetStartupScene("Init");
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsReleaseBuild { get; set; }

        public BuildTarget BuildTarget { get; set; }
        public BuildTargetGroup BuildTargetGroup { get; set; }

        public ManagedDirectory TargetDirectory { get; set; }

        public ManagedFile TargetFileName { get; set; }

        public ManagedFile StaticDataFile { get; set; }

        public ManagedDirectory SceneBasePath { get; set; }

        public ManagedFile StartupScene { get; set; }

        public bool IncludeCoreScenes { get; set; }
        
        public IList<ManagedFile> ScenesToIncludeInBuild { get; private set; }

        public void SetTargetName(string name)
        {
            this.TargetFileName = new ManagedFile(name + ".exe");
        }

        public void SetStartupScene(string name)
        {
            this.StartupScene = this.SceneBasePath.ToFile(name + EssentialsConstants.UnitySceneExtension);
            this.RefreshScenesToInclude();
        }

        public void IncludeScene(ManagedFile file)
        {
            this.additionalScenesToInclude.Add(file);
            this.RefreshScenesToInclude();
        }

        public void RefreshScenesToInclude()
        {
            this.ScenesToIncludeInBuild.Clear();
            this.ScenesToIncludeInBuild.Add(this.StartupScene);

            if (this.IncludeCoreScenes)
            {
                Runtime.Extensions.CollectionExtensions.AddRange(this.ScenesToIncludeInBuild, EssentialsCore.CoreScenes);
            }

            Runtime.Extensions.CollectionExtensions.AddRange(this.ScenesToIncludeInBuild, this.additionalScenesToInclude);
        }
    }
}

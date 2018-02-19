namespace Assets.Scripts.Craiel.Essentials.Editor
{
    using System.Collections.Generic;
    using IO;
    using UnityEditor;
    using UnityEngine;

    public class UnityBuildContext
    {
        private readonly IList<CarbonFile> additionalScenesToInclude;

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
            
            this.ScenesToIncludeInBuild = new List<CarbonFile>();
            this.additionalScenesToInclude = new List<CarbonFile>();

            this.SetStartupScene("Init");
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsReleaseBuild { get; set; }

        public BuildTarget BuildTarget { get; set; }
        public BuildTargetGroup BuildTargetGroup { get; set; }

        public CarbonDirectory TargetDirectory { get; set; }

        public CarbonFile TargetFileName { get; set; }

        public CarbonFile StaticDataFile { get; set; }

        public CarbonDirectory SceneBasePath { get; set; }

        public CarbonFile StartupScene { get; set; }

        public bool IncludeCoreScenes { get; set; }
        
        public IList<CarbonFile> ScenesToIncludeInBuild { get; private set; }

        public void SetTargetName(string name)
        {
            this.TargetFileName = new CarbonFile(name + ".exe");
        }

        public void SetStartupScene(string name)
        {
            this.StartupScene = this.SceneBasePath.ToFile(name + ".unity");
            this.RefreshScenesToInclude();
        }

        public void IncludeScene(CarbonFile file)
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
                this.ScenesToIncludeInBuild.AddRange(EssentialsCore.CoreScenes);
            }

            this.ScenesToIncludeInBuild.AddRange(this.additionalScenesToInclude);
        }
    }
}

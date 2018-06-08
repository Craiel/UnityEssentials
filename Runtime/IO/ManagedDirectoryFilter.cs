namespace Craiel.UnityEssentials.Runtime.IO
{
    using System.IO;

    public class ManagedDirectoryFilter
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ManagedDirectoryFilter(ManagedDirectory directory, params string[] filterStrings)
        {
            this.Directory = directory;
            this.FilterStrings = filterStrings;
            this.Option = SearchOption.TopDirectoryOnly;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ManagedDirectory Directory { get; private set; }

        public string[] FilterStrings { get; set; }

        public SearchOption Option { get; set; }
    }
}

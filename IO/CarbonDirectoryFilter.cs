﻿namespace Assets.Scripts.Craiel.Essentials.IO
{
    using System.IO;

    public class CarbonDirectoryFilter
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonDirectoryFilter(CarbonDirectory directory, params string[] filterStrings)
        {
            this.Directory = directory;
            this.FilterStrings = filterStrings;
            this.Option = SearchOption.TopDirectoryOnly;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CarbonDirectory Directory { get; private set; }

        public string[] FilterStrings { get; set; }

        public SearchOption Option { get; set; }
    }
}

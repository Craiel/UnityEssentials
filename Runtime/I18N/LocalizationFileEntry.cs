namespace Craiel.UnityEssentials.Runtime.I18N
{
    using System;

    [Serializable]
    public class LocalizationFileEntry
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LocalizationFileEntry()
        {
            this.Id = new string[0];
            this.Str = new string[0];
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Note;

        public string[] Id;

        public string[] Str;
    }
}
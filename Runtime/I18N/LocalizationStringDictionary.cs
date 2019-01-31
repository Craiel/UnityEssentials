namespace Craiel.UnityEssentials.Runtime.I18N
{
    using System.Collections.Generic;

    public class LocalizationStringDictionary : Dictionary<string, string>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LocalizationStringDictionary()
        {
        }

        public LocalizationStringDictionary(LocalizationStringDictionary source)
            : base(source)
        {
        }
    }
}

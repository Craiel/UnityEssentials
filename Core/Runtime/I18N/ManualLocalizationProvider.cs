namespace Craiel.UnityEssentials.Runtime.I18N
{
    public class ManualLocalizationProvider : ILocalizationProvider
    {
        private readonly LocalizationStringDictionary entries;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ManualLocalizationProvider()
        {
            this.entries = new LocalizationStringDictionary();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string GetString(string key)
        {
            if (this.entries.TryGetValue(key, out string result))
            {
                return result;
            }

            return key;
        }

        public void Clear()
        {
            this.entries.Clear();
        }

        public void Add(string key, string value)
        {
            this.entries.Add(key, value);
        }
    }
}
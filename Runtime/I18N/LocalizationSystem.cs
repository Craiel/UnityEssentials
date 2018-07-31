namespace Craiel.UnityEssentials.Runtime.I18N
{
    using System.Globalization;
    using IO;
    using Singletons;
    using UnityEngine;

    public class LocalizationSystem : UnitySingletonBehavior<LocalizationSystem>
    {
        private float lastAutoSave;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Update()
        {
            if (Time.time > this.lastAutoSave + EssentialsCore.LocalizationSaveInterval)
            {
                EssentialsCore.Logger.Info("Saving Localization: {0}", Localization.Root);

                this.lastAutoSave = Time.time;

                // TODO: Localization
                Localization.SaveDictionaries();
            }
        }

        public override void Initialize()
        {
            base.Initialize();

            Localization.SetRoot(new ManagedDirectory(Application.persistentDataPath));
        }

        public void SetCulture(CultureInfo culture)
        {
            Localization.CurrentCulture = culture;
            Localization.ReloadDictionaries();
        }
    }
}

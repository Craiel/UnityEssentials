namespace Craiel.UnityEssentials.Runtime.I18N
{
    using System.Globalization;
    using IO;
    using NLog;
    using Singletons;
    using UnityEngine;

    public class LocalizationSystem : UnitySingletonBehavior<LocalizationSystem>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private float lastAutoSave;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Update()
        {
            if (Time.time > this.lastAutoSave + EssentialsCore.LocalizationSaveInterval)
            {
                Logger.Info("Saving Localization: {0}", Localization.Root);

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

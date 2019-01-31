namespace Craiel.UnityEssentials.Runtime.I18N
{
    using System.Globalization;
    using IO;
    using Singletons;
    using UnityEngine;

    public class LocalizationSystem : UnitySingletonBehavior<LocalizationSystem>
    {
        private const float AutoSaveInterval = 60 * 5;
        
        private LocalizationProvider provider;

        private float lastAutoSave;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Update()
        {
#if DEBUG
            if (Time.time > this.lastAutoSave + AutoSaveInterval)
            {
                EssentialsCore.Logger.Info("Saving Localization: {0}", this.provider.Root);

                this.lastAutoSave = Time.time;

                // TODO: Localization
                this.provider.SaveDictionary();
            }
#endif
        }

        public override void Initialize()
        {
            base.Initialize();

            this.provider = new LocalizationProvider();

#if DEBUG
            this.provider.SetRoot(new ManagedDirectory(Application.persistentDataPath));
#endif

            Localization.Initialize(this.provider);
        }

        public void SetCulture(CultureInfo culture)
        {
            Localization.CurrentCulture = culture;
            this.provider.ReloadDictionary();
        }
    }
}

namespace Craiel.UnityEssentials.Runtime.I18N
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Enums;
    using IO;
    using Resource;
    
    using UnityEngine;
    using YamlDotNet.Core;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class LocalizationProvider : ILocalizationProvider
    {
        private const string SubDirectory = "i18n";
        private const string PoFileExtension = ".txt";
        private const string LocalFileName = "locale.po" + PoFileExtension;
        
        private readonly LocalizationStringDictionary currentDictionary;

        private CultureInfo loadedCulture;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LocalizationProvider()
        {
            this.currentDictionary = new LocalizationStringDictionary();

#if DEBUG
            Root = EssentialsCore.PersistentDataPath;
#endif
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ManagedDirectory Root { get; private set; }

        public void SetRoot(ManagedDirectory newRoot)
        {
            Root = newRoot;
            ReloadDictionary();
        }

        public void ReloadDictionary(bool forceLoadFromResource = false)
        {
#if DEBUG
            this.SaveDictionary();
#endif

#if DEBUG
            if (!forceLoadFromResource)
            {
                if (this.LoadDictionaryFromFile())
                {
                    this.loadedCulture = Localization.CurrentCulture;
                }
                
                return;
            }
#endif
            
            if (this.LoadDictionaryFromResource())
            {
                this.loadedCulture = Localization.CurrentCulture;
            }
        }

        public bool HasString(string key)
        {
            return this.currentDictionary.ContainsKey(key);
        }

        public string GetString(string key)
        {
            if (key.StartsWith(EssentialsConstants.LocalizationIgnoreString))
            {
                return key;
            }
            
            string result;
            if (this.currentDictionary.TryGetValue(key, out result))
            {
                if (string.IsNullOrEmpty(result))
                {
                    return key;
                }
                
                return result;
            }

#if DEBUG
            if (this.currentDictionary.Values.Any(x => string.Equals(x, key)))
            {
                EssentialsCore.Logger.Warn("Localization recursion where key = '{0}'", key);

                return key;
            }

            this.currentDictionary.Add(key, key);
#endif

            return key;
        }

        public LocalizationStringDictionary GetCopy()
        {
            return new LocalizationStringDictionary(this.currentDictionary);
        }

        public void SetData(CultureInfo culture, LocalizationStringDictionary data)
        {
            this.loadedCulture = culture;
            this.currentDictionary.Clear();
            foreach (string key in data.Keys)
            {
                this.currentDictionary.Add(key, data[key]);
            }
        }

#if DEBUG
        public void OverrideLocalizationFilesByMasterFiles()
        {
            foreach (var cultureInfo in LocaleConstants.AllCultures)
            {
                ManagedFile cultureLocalizationFile = this.GetLocalizationFile(cultureInfo);

                cultureLocalizationFile.GetDirectory().Create();
                cultureLocalizationFile.DeleteIfExists();

                cultureLocalizationFile.WriteAsString(this.LoadDictionaryResource(cultureInfo));
            }

            // Clear the loaded culture to avoid auto-saving
            this.loadedCulture = null;
            ReloadDictionary();
        }

        public void SaveDictionary(ManagedFile file = null)
        {
            if (this.loadedCulture == null)
            {
                return;
            }

            if (file == null)
            {
                file = this.GetLocalizationFile(this.loadedCulture);
            }

            file.GetDirectory().Create();
            var newFormat = new LocalizationFile();
            newFormat.AddData(this.loadedCulture, this.currentDictionary);
            newFormat.SaveAsPO(this.loadedCulture, file);
        }
#endif

        public string LoadDictionaryResource(CultureInfo customCulture = null)
        {
            ResourceKey key = LocaleConstants.GetLocalizationMasterFile(customCulture ?? Localization.CurrentCulture);
            
#if UNITY_EDITOR
            if (!ResourceProvider.IsInstanceActive)
            {
                var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(EssentialsCore.ResourcesPath.ToFile(key.Path + PoFileExtension).GetUnityPath());
                return asset != null ? asset.text : null;
            }
#endif

            var textAsset = key.LoadManaged<TextAsset>();
            if (textAsset == null)
            {
                EssentialsCore.Logger.Warn("Could not load dictionary for {0}, file not found", customCulture ?? Localization.CurrentCulture);
                return null;
            }
            
            return textAsset.text;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private ManagedFile GetLocalizationFile(CultureInfo customCulture = null)
        {
            return Root.ToFile(SubDirectory, customCulture == null ? Localization.CurrentCulture.Name : customCulture.Name, LocalFileName);
        }

        private bool LoadDictionaryFromResource()
        {
            string data = LoadDictionaryResource();
            if (data == null)
            {
                return false;
            }

            var localizationFile = new LocalizationFile();
            localizationFile.LoadFromPO(data);
            return this.Load(localizationFile);
        }

        private bool LoadDictionaryFromFile(ManagedFile file = null)
        {
            if (file == null)
            {
                file = this.GetLocalizationFile();
            }

            if (!file.Exists)
            {
                EssentialsCore.Logger.Warn("Could not load dictionary for {0}, file not found: {1}", Localization.CurrentCulture, file);
                return false;
            }

            EssentialsCore.Logger.Info("Loading Dictionary {0} ({1})", Localization.CurrentCulture, file);
            
            var localizationFile = new LocalizationFile();
            localizationFile.LoadFromPO(file);
            return this.Load(localizationFile);
        }

        private bool Load(LocalizationFile data)
        {
            if (data == null)
            {
                return false;
            }

            this.currentDictionary.Clear();
            LocalizationStringDictionary dictionary = data.ToDictionary();
            foreach (string key in dictionary.Keys)
            {
                this.currentDictionary.Add(key, dictionary[key]);
            }

            return true;
        }
    }
}
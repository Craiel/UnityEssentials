namespace Craiel.UnityEssentials.Runtime.EngineCore.Modules
{
    using System.Collections.Generic;
    using Enums;
    using IO;

    public class ModuleSaveLoad : GameModuleLite
    {
        private readonly IDictionary<string, object> data;
        
        private ManagedDirectory saveRoot;
        private string savePrefix;
        private string activeSaveName;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModuleSaveLoad()
        {
            this.data = new Dictionary<string, object>();
            
            this.saveRoot = EssentialsCore.DefaultSavePath;
            this.savePrefix = EssentialsCore.DefaultSavePrefix;
            this.Mode = SaveLoadMode.PlayerPrefs;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public SaveLoadMode Mode { get; private set; }
        
        public override void Initialize()
        {
            base.Initialize();
            
            this.RefreshSaveRegister();
        }

        public void Set<T>(string key, T value)
        {
            if (this.data.ContainsKey(key))
            {
                this.data[key] = value;
            }
            else
            {
                this.data.Add(key, value);
            }
        }

        public T Get<T>(string key)
        {
            if (this.data.TryGetValue(key, out object value))
            {
                return (T) value;
            }

            return default;
        }
        
        public bool SaveAs(string name)
        {
            // TODO

            return false;
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(this.activeSaveName))
            {
                EssentialsCore.Logger.Error("Save Called with no active save state");
                return;
            }
            
            // TODO
        }

        public bool Load(string name)
        {
            // TODO

            return false;
        }

        public IList<string> GetAvailableSaves()
        {
            this.RefreshSaveRegister();
            
            // TODO
            return null;
        }

        public void SetSavePath(ManagedDirectory newSavePath)
        {
            this.saveRoot = newSavePath;
        }

        public void SetSavePrefix(string newPrefix)
        {
            if (string.IsNullOrEmpty(newPrefix))
            {
                EssentialsCore.Logger.Error("Save Prefix was invalid!");
                return;
            }

            this.savePrefix = newPrefix;
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void RefreshSaveRegister()
        {
            // TODO
        }
    }
}
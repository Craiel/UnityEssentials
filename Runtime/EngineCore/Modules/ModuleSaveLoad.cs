namespace Craiel.UnityEssentials.Runtime.EngineCore.Modules
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Data;
    using Data.SBT;
    using Enums;
    using IO;
    using UnityEngine;

    public class ModuleSaveLoad : GameModuleLite
    {
        private static readonly byte[] SaveHeaderInternal = {69, 83, 83, 33};
        private const ushort SaveVersionInternal = 1;
        private const string PlayerPrefsRegisterPrefix = "SLREG";
        private const string PlayerPrefsPrefix = "ESC_";
        private const char PlayerPrefsDataSeparator = '|';
        
        private readonly HashSet<SaveDataId> register;

        private ManagedDirectory saveRoot;
        private string savePrefix;
        private string saveSuffix;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModuleSaveLoad()
        {
            this.register = new HashSet<SaveDataId>();
            
            this.saveRoot = EssentialsCore.DefaultSavePath;
            this.savePrefix = EssentialsCore.DefaultSavePrefix;
            this.saveSuffix = "";
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
        
        public SBTDictionary Create(SaveDataId key)
        {
            if (this.Exists(key))
            {
                throw new InvalidOperationException("Save Exists!");
            }
            
            var entry = new SBTDictionary();
            this.Save(key, entry);
            
            this.register.Add(key);
            if (this.Mode == SaveLoadMode.PlayerPrefs)
            {
                this.SavePlayerPrefsRegister();
            }
            
            return entry;
        }
        
        public bool Exists(SaveDataId key)
        {
            return this.register.Contains(key);
        }
        
        public bool Save(SaveDataId key, SBTDictionary data)
        {
            return this.DoSave(key, data);
        }

        public SBTDictionary Load(SaveDataId key)
        {
            if (!this.Exists(key))
            {
                return null;
            }
            
            return this.DoLoad(key);
        }

        public bool Delete(SaveDataId key)
        {
            return this.DoDelete(key);
        }

        public IList<SaveDataId> GetAvailableSaves()
        {
            this.RefreshSaveRegister();
            return new List<SaveDataId>(this.register);
        }

        public void SetSavePath(ManagedDirectory newSavePath)
        {
            this.saveRoot = newSavePath;
            this.RefreshSaveRegister();
        }

        public ManagedFile GetSavePath(SaveDataId id)
        {
            switch (this.Mode)
            {
                case SaveLoadMode.PlayerPrefs:
                {
                    throw new InvalidOperationException("PlayerPrefs mode has no save paths");
                }
            }

            string fileName = string.Format("{0}{1}{2}", this.savePrefix, id.Id, this.saveSuffix);
            return this.saveRoot.ToFile(fileName);
        }

        public void SetSavePrefix(string newPrefix)
        {
            if (newPrefix == null)
            {
                newPrefix = "";
            }

            this.savePrefix = newPrefix;
        }
        
        public void SetSaveSuffix(string newSuffix)
        {
            if (newSuffix == null)
            {
                newSuffix = "";
            }

            this.saveSuffix = newSuffix;
        }
        
        public void SetMode(SaveLoadMode newMode)
        {
            if (this.Mode == newMode)
            {
                return;
            }

            this.Mode = newMode;
            this.RefreshSaveRegister();
        }

        public void ForceRefresh()
        {
            this.RefreshSaveRegister();
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void RefreshSaveRegister()
        {
            this.register.Clear();
            
            switch (this.Mode)
            {
                case SaveLoadMode.PlayerPrefs:
                {
                    var registerKey = string.Concat(PlayerPrefsPrefix, PlayerPrefsRegisterPrefix, SaveVersionInternal);

                    string data = PlayerPrefs.GetString(registerKey);
                    string[] idList = data.Split(PlayerPrefsDataSeparator);
                    foreach (string id in idList)
                    {
                        if (string.IsNullOrEmpty(id))
                        {
                            continue;
                        }

                        this.LoadRegisterId(id.Replace(PlayerPrefsPrefix.ToLowerInvariant(), string.Empty));
                    }
                    
                    break;
                }

                case SaveLoadMode.PersistentDataPath:
                {
                    ManagedFileResult[] results = this.saveRoot.GetFiles(string.Concat(this.savePrefix, "*", this.saveSuffix));
                    foreach (ManagedFileResult result in results)
                    {
                        using (var stream = result.Absolute.OpenRead())
                        {
                            byte[] header = new byte[SaveHeaderInternal.Length];
                            stream.Read(header, 0, header.Length);
                            if (!header.SequenceEqual(SaveHeaderInternal))
                            {
                                // File header incorrect, skip
                                continue;
                            }

                            this.LoadRegisterId(result.Absolute.FileName);
                        }
                    }
                    
                    break;
                }
            }
        }

        private void LoadRegisterId(string rawId)
        {
            string finalId = rawId;
            if (!string.IsNullOrEmpty(this.savePrefix))
            {
                finalId = finalId.Replace(this.savePrefix, string.Empty);
            }

            if (!string.IsNullOrEmpty(this.saveSuffix))
            {
                finalId = finalId.Replace(this.saveSuffix, string.Empty);
            }

            this.register.Add(new SaveDataId(finalId));
        }
        
        private bool DoSave(SaveDataId key, SBTDictionary data)
        {
            switch (this.Mode)
            {
                case SaveLoadMode.PlayerPrefs:
                {
                    string serialized = data.SerializeToString();
                    PlayerPrefs.SetString(PlayerPrefsPrefix + key.Id, serialized);
                    PlayerPrefs.Save();
                    
                    return true;
                }

                case SaveLoadMode.PersistentDataPath:
                {
                    ManagedFile file = this.GetSavePath(key);
                    if (file == null)
                    {
                        return false;
                    }
                    
                    // Ensure the directory exists
                    file.GetDirectory().Create();
                    
                    using (var stream = file.OpenWrite())
                    {
                        using (var writer = new BinaryWriter(stream))
                        {
                            writer.Write(SaveHeaderInternal, 0, SaveHeaderInternal.Length);
                            writer.Write(SaveVersionInternal);
                            
                            data.Serialize(writer);
                        }
                    }

                    return true;
                }

                default:
                {
                    throw new NotImplementedException();
                }
            }
        }

        private SBTDictionary DoLoad(SaveDataId key)
        {
            if (!this.Exists(key))
            {
                return null;
            }

            switch (this.Mode)
            {
                case SaveLoadMode.PlayerPrefs:
                {
                    string data = PlayerPrefs.GetString(PlayerPrefsPrefix + key.Id);
                    if (string.IsNullOrEmpty(data))
                    {
                        EssentialsCore.Logger.Warn("PlayerPrefs data was empty!");
                        return null;
                    }

                    return SBTDictionary.Deserialize(data);
                }

                case SaveLoadMode.PersistentDataPath:
                {
                    ManagedFile file = this.GetSavePath(key);
                    if (!file.Exists)
                    {
                        EssentialsCore.Logger.Warn("Load called on non-existing save: {0} -> {1}", key, file);
                        return null;
                    }
                    
                    var result = new SBTDictionary();
                    using (var stream = file.OpenRead())
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            byte[] header = new byte[SaveHeaderInternal.Length];
                            reader.Read(header, 0, header.Length);
                            if (!header.SequenceEqual(SaveHeaderInternal))
                            {
                                EssentialsCore.Logger.Warn("Load called on invalid save file");
                                return null;
                            }
                            
                            ushort version = reader.ReadUInt16();
                            if (version < SaveVersionInternal)
                            {
                                EssentialsCore.Logger.Warn("Load called on outdated save file");
                                return null;
                            }
                            
                            result.Deserialize(reader);
                            return result;
                        }
                    }
                }

                default:
                {
                    throw new NotImplementedException();
                }
            }
        }

        private bool DoDelete(SaveDataId key)
        {
            switch (this.Mode)
            {
                case SaveLoadMode.PlayerPrefs:
                {
                    PlayerPrefs.DeleteKey(PlayerPrefsPrefix + key.Id);
                    PlayerPrefs.Save();
                    
                    this.register.Remove(key);
                    this.SavePlayerPrefsRegister();

                    return true;
                }

                case SaveLoadMode.PersistentDataPath:
                {
                    ManagedFile file = this.GetSavePath(key);
                    if (file.Exists)
                    {
                        file.Delete();
                        this.RefreshSaveRegister();
                        return true;
                    }

                    return false;
                }

                default:
                {
                    throw new NotImplementedException();
                }
            }
        }

        private void SavePlayerPrefsRegister()
        {
            var registerKey = string.Concat(PlayerPrefsPrefix, PlayerPrefsRegisterPrefix, SaveVersionInternal);
            string data = string.Join(PlayerPrefsDataSeparator.ToString(), this.register.Select(x => x.Id));
            
            PlayerPrefs.SetString(registerKey, data);
            PlayerPrefs.Save();
        }
    }
}
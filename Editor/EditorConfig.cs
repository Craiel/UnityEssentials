namespace Assets.Scripts.Craiel.Essentials.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NLog;
    using UnityEditor;
    using UnityEngine;

    internal static class EditorConfigStatic
    {
        internal static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
    }

    public abstract class EditorConfig<T>
        where T : struct, IConvertible
    {
        private const string SaveKeyDataSuffix = "_data";
        private const string SaveKeyVersionSuffix = "_version";

        private readonly string saveKey;

        private readonly IList<T> enumValues;
        
        private SaveContent content;

        private bool inBatch;

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected EditorConfig(string saveKey, int version)
        {
            if (string.IsNullOrEmpty(saveKey))
            {
                throw new InvalidOperationException("EditorConfig Save key is invalid");
            }
            
            this.enumValues = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            this.content = new SaveContent();
            this.content.Initialize(this.enumValues.Count);

            this.saveKey = saveKey;
            this.Version = version;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Version { get; private set; }

        public void Save()
        {
            string rawData = JsonUtility.ToJson(this.content);

            EditorPrefs.SetInt(this.saveKey + SaveKeyVersionSuffix, this.Version);
            EditorPrefs.SetString(this.saveKey + SaveKeyDataSuffix, rawData);
        }

        public void Load()
        {
            int version = EditorPrefs.GetInt(this.saveKey + SaveKeyVersionSuffix, this.Version);
            string rawData = EditorPrefs.GetString(this.saveKey + SaveKeyDataSuffix, string.Empty);

            if (string.IsNullOrEmpty(rawData))
            {
                this.content.Clear();
                return;
            }

            SaveContent loadedContent = JsonUtility.FromJson<SaveContent>(rawData);
            if (loadedContent == null)
            {
                this.content.Clear();
                return;
            }
            
            if (version != this.Version)
            {
                if (!this.Upgrade(ref loadedContent, version))
                {
                    return;
                }
            }

            this.content = loadedContent;
            this.content.Initialize(this.enumValues.Count);
        }

        public void Set(T key, string value)
        {
            this.content.StringData[Convert.ToInt32(key)] = value;

            if (!this.inBatch)
            {
                this.Save();
            }
        }

        public void Set(T key, int? value = null)
        {
            int keyId = Convert.ToInt32(key);

            if (value == null)
            {
                this.content.IntData[keyId] = 0;
                this.content.IntDataSet[keyId] = false;
            }
            else
            {
                this.content.IntData[keyId] = value.Value;
                this.content.IntDataSet[keyId] = true;
            }

            if (!this.inBatch)
            {
                this.Save();
            }
        }

        public void Set(T key, bool? value = null)
        {
            int keyId = Convert.ToInt32(key);

            if (value == null)
            {
                this.content.BoolData[keyId] = false;
                this.content.BoolDataSet[keyId] = false;
            }
            else
            {
                this.content.BoolData[keyId] = value.Value;
                this.content.BoolDataSet[keyId] = true;
            }

            if (!this.inBatch)
            {
                this.Save();
            }
        }

        public void Set(T key, float? value = null)
        {
            int keyId = Convert.ToInt32(key);

            if (value == null)
            {
                this.content.FloatData[keyId] = 0f;
                this.content.FloatDataSet[keyId] = false;
            }
            else
            {
                this.content.FloatData[keyId] = value.Value;
                this.content.FloatDataSet[keyId] = true;
            }

            if (!this.inBatch)
            {
                this.Save();
            }
        }

        public string GetString(T key, string defaultValue = null)
        {
            int keyId = Convert.ToInt32(key);
            
            string value = this.content.StringData[keyId];
            return value ?? defaultValue;
        }

        public int GetInt(T key, int defaultValue = 0)
        {
            int keyId = Convert.ToInt32(key); 

            if (!this.content.IntDataSet[keyId])
            {
                return defaultValue;
            }
            
            return this.content.IntData[keyId];
        }

        public bool GetBool(T key, bool defaultValue = false)
        {
            int keyId = Convert.ToInt32(key);

            if (!this.content.BoolDataSet[keyId])
            {
                return defaultValue;
            }
            
            return this.content.BoolData[keyId];
        }

        public float GetFloat(T key, float defaultValue = 0f)
        {
            int keyId = Convert.ToInt32(key);

            if (!this.content.FloatDataSet[keyId])
            {
                return defaultValue;
            }
            
            return this.content.FloatData[keyId];
        }

        public void BeginBatch()
        {
            if (this.inBatch)
            {
                throw new InvalidOperationException("EditorConfig is already in batch");
            }

            this.inBatch = true;
        }

        public void EndBatch()
        {
            if (!this.inBatch)
            {
                throw new InvalidOperationException("EditorConfig was not in batch");
            }

            this.inBatch = false;
            this.Save();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected bool Upgrade(ref SaveContent oldContent, int oldVersion)
        {
            EditorConfigStatic.Logger.Warn("EditorConfig Upgrade not implemented for {0}, Version {1} -> {2}", this.GetType().Name, oldVersion, this.Version);
            return false;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        [Serializable]
        protected class SaveContent
        {
            [SerializeField]
            public string[] StringData;

            [SerializeField]
            public int[] IntData;

            [SerializeField]
            public bool[] IntDataSet;

            [SerializeField]
            public bool[] BoolData;

            [SerializeField]
            public bool[] BoolDataSet;

            [SerializeField]
            public float[] FloatData;

            [SerializeField]
            public bool[] FloatDataSet;

            public void Clear()
            {
                Array.Clear(this.StringData, 0, this.StringData.Length);
                Array.Clear(this.IntData, 0, this.IntData.Length);
                Array.Clear(this.IntDataSet, 0, this.IntDataSet.Length);
                Array.Clear(this.BoolData, 0, this.BoolData.Length);
                Array.Clear(this.BoolDataSet, 0, this.BoolDataSet.Length);
                Array.Clear(this.FloatData, 0, this.FloatData.Length);
                Array.Clear(this.FloatDataSet, 0, this.FloatDataSet.Length);
            }

            public void Initialize(int entryCount)
            {
                this.StringData = this.StringData ?? new string[entryCount];
                this.IntData = this.IntData ?? new int[entryCount];
                this.IntDataSet = this.IntDataSet ?? new bool[entryCount];
                this.BoolData = this.BoolData ?? new bool[entryCount];
                this.BoolDataSet = this.BoolDataSet ?? new bool[entryCount];
                this.FloatData = this.FloatData ?? new float[entryCount];
                this.FloatDataSet = this.FloatDataSet ?? new bool[entryCount];
            }
        }
    }
}

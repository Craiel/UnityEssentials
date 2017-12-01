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

        private readonly IList<T> enumValues;

        private readonly string saveKey;

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

            // Get all keys
            this.enumValues = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            this.content = new SaveContent(this.enumValues.Count);

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
            if (version != this.Version)
            {
                if (!this.Upgrade(ref loadedContent, version))
                {
                    return;
                }
            }

            this.content = loadedContent;
        }

        public void Set(T key, string value)
        {
            this.content.StringData[Convert.ToInt32(key)] = value;

            if (!this.inBatch)
            {
                this.Save();
            }
        }

        public void Set(T key, int value)
        {
            this.content.IntData[Convert.ToInt32(key)] = value;

            if (!this.inBatch)
            {
                this.Save();
            }
        }

        public void Set(T key, bool value)
        {
            this.content.BoolData[Convert.ToInt32(key)] = value;

            if (!this.inBatch)
            {
                this.Save();
            }
        }

        public void Set(T key, float value)
        {
            this.content.FloatData[Convert.ToInt32(key)] = value;

            if (!this.inBatch)
            {
                this.Save();
            }
        }

        public string GetString(T key, string defaultValue = null)
        {
            string value = this.content.StringData[Convert.ToInt32(key)];
            return value ?? defaultValue;
        }

        public int GetInt(T key, int defaultValue = 0)
        {
            int? value = this.content.IntData[Convert.ToInt32(key)];
            return value ?? defaultValue;asd
        }

        public bool GetBool(T key, bool defaultValue = false)
        {
            bool? value = this.content.BoolData[Convert.ToInt32(key)];
            return value ?? defaultValue;asd
        }

        public float GetFloat(T key, float defaultValue = 0f)
        {
            float? value = this.content.FloatData[Convert.ToInt32(key)];
            return value ?? defaultValue;asd
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
            public SaveContent(int entryCount)
            {
                this.StringData = new string[entryCount];
                this.IntData = new int[entryCount];
                this.IntDataSet = new bool[entryCount];
                this.BoolData = new bool[entryCount];
                this.BoolDataSet = new bool[entryCount];
                this.FloatData = new float[entryCount];
                this.FloatDataSet = new bool[entryCount];
            }

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
        }
    }
}

namespace Craiel.UnityEssentials.Json
{
    using System;
    using Contracts;
    using IO;
    using NLog;
    using UnityEngine;
    using Debug = System.Diagnostics.Debug;

    public static class JsonConfigStatic
    {
        internal static readonly global::NLog.Logger Logger = LogManager.GetCurrentClassLogger();
    }

    public class JsonConfig<T> : IJsonConfig<T>
        where T : class
    {
        private ManagedFile configFile;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public T Current { get; set; }

        public virtual bool Load(ManagedFile file)
        {
            this.configFile = file;
            return this.LoadConfig(this.configFile);
        }

        public virtual bool Save(ManagedFile file = null)
        {
            ManagedFile targetFile = file ?? this.configFile;
            Debug.Assert(targetFile != null);

            try
            {
                string contents = JsonUtility.ToJson(this.Current);
                targetFile.WriteAsString(contents);
                return true;
            }
            catch (Exception e)
            {
                JsonConfigStatic.Logger.Error(e, "Could not save config to {0}", file);
                return false;
            }
        }

        public virtual void Reset()
        {
            this.Current = this.GetDefault();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual T GetDefault()
        {
            return Activator.CreateInstance<T>();
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool LoadConfig(ManagedFile file)
        {
            if (file.Exists)
            {
                string contents = file.ReadAsString();
                this.Current = JsonUtility.FromJson<T>(contents);
            }
            else
            {
                JsonConfigStatic.Logger.Warn("Config {0} does not exist, skipping", file);
            }

            if (this.Current == null)
            {
                JsonConfigStatic.Logger.Error("Config is invalid, resetting to default");
                this.Current = this.GetDefault();

                string contents = JsonUtility.ToJson(this.Current);
                file.WriteAsString(contents);
                return false;
            }

            return true;
        }
    }
}

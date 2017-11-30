namespace Assets.Scripts.Craiel.Essentials.Debug.Gym
{
    using IO;
    using Logging;
    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using SetupCode;
    using UnityEngine;

    public abstract class DebugGymBase : MonoBehaviour
    {
        private bool sceneBehaviorsDisabled;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public GameObject Controller;

        public void LateUpdate()
        {
            if (!this.sceneBehaviorsDisabled)
            {
                this.sceneBehaviorsDisabled = true;
                this.DisableSceneBehaviors();
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void DisableSceneBehaviors()
        {
        }

        protected void DisableSceneBehavior<T>()
            where T : MonoBehaviour
        {
            var targets = FindObjectsOfType<T>();
            foreach (T target in targets)
            {
                target.enabled = false;
            }

            Debug.LogFormat("DEBUG Disabled {0} Behaviors of type {1}", targets.Length, typeof(T).Name);
        }

        protected void InitDefaultNLog()
        {
            if (!NLogInterceptor.IsInstanceActive)
            {
                NLogInterceptor.InstantiateAndInitialize();
            }

            UnityNLogRelay.InstantiateAndInitialize();

            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();
            config.AddTarget("interceptor", NLogInterceptor.Instance.Target);

            var fileTarget = new FileTarget
            {
                ArchiveAboveSize = Constants.LogFileArchiveSize,
                MaxArchiveFiles = Constants.LogFileArchiveCount,
                KeepFileOpen = true,
                ConcurrentWrites = true,
                ArchiveOldFileOnStartup = true
            };

            config.AddTarget("file", fileTarget);

            var target = new CarbonDirectory(Application.persistentDataPath);
            target.Create();

            UnityEngine.Debug.Log("NLog Path: " + target);

            // Step 3. Set target properties 
            fileTarget.FileName = target.GetUnityPath() + "/all.log";
            fileTarget.Layout = @"${date:format=HH\:mm\:ss.fff} [${threadid}] ${level} ${message}";

            var rule = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule);

            rule = new LoggingRule("*", LogLevel.Debug, NLogInterceptor.Instance.Target);
            config.LoggingRules.Add(rule);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;
        }
    }
}

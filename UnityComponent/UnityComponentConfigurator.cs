namespace Assets.Scripts.Craiel.Essentials.UnityComponent
{
    using System;
    using System.Linq;
    using Audio.Contracts;
    using Contracts;
    using NLog;

    internal class UnityComponentCoreStatic
    {
        public static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
    }

    public class UnityComponentConfigurator<T>
        where T : IComponentConfig
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public void Configure()
        {
            Initialize();
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Initialize()
        {
            Type configType = typeof(T);
            var implementations = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => !x.IsInterface && configType.IsAssignableFrom(x))
                .ToList();

            if (implementations.Count != 1)
            {
                UnityComponentCoreStatic.Logger.Error("No implementation of {0} found, configure your game data first", typeof(T).Name);
                return;
            }

            var config = Activator.CreateInstance(implementations.First()) as IAudioConfig;
            if (config == null)
            {
                UnityComponentCoreStatic.Logger.Error("Failed to instantiate config class of type {0}", typeof(T).Name);
                return;
            }

            config.Configure();
        }
    }
}
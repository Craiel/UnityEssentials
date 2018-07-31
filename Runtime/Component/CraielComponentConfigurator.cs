namespace Craiel.UnityEssentials.Runtime.Component
{
    using System;
    using System.Linq;
    using Contracts;

    public class CraielComponentConfigurator<T>
        where T : class, ICraielComponentConfig
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public void Configure()
        {
            this.Initialize();
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
                EssentialsCore.Logger.Error("No implementation of {0} found, configure your game data first", typeof(T).Name);
                return;
            }

            T config = Activator.CreateInstance(implementations.First()) as T;
            if (config == null)
            {
                EssentialsCore.Logger.Error("Failed to instantiate config class of type {0}", typeof(T).Name);
                return;
            }

            config.Configure();
        }
    }
}
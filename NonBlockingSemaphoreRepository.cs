using INonBlockingSemaphore = Craiel.UnityEssentials.Contracts.INonBlockingSemaphore;

namespace Craiel.UnityEssentials
{
    using System.Collections.Generic;
    using Contracts;

    public static class NonBlockingSemaphoreRepository
    {
        private static readonly IDictionary<string, INonBlockingSemaphore> Repository;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static NonBlockingSemaphoreRepository()
        {
            Repository = new Dictionary<string, INonBlockingSemaphore>();
            Factory = new SimpleNonBlockingSemaphore.LocalFactory();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ISemaphoreFactory Factory { get; set; }

        public static INonBlockingSemaphore Add(string name, int maxResources)
        {
            INonBlockingSemaphore semaphore = Factory.Create(name, maxResources);
            Repository.Add(name, semaphore);
            return semaphore;
        }

        public static INonBlockingSemaphore Get(string name)
        {
            return Repository[name];
        }

        public static void Clear()
        {
            Repository.Clear();
        }
    }
}

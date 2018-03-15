namespace Craiel.UnityEssentials
{
    using Contracts;

    public class SimpleNonBlockingSemaphore : INonBlockingSemaphore
    {
        private int acquiredResources;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SimpleNonBlockingSemaphore(string name, int maxResources)
        {
            this.Name = name;
            this.MaxResources = maxResources;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static readonly LocalFactory Factory = new LocalFactory();

        public int MaxResources { get; private set; }

        public string Name { get; private set; }

        public bool Acquire(int resources = 1)
        {
            if (this.acquiredResources + resources <= this.MaxResources)
            {
                this.acquiredResources += resources;
                return true;
            }

            return false;
        }

        public bool Release(int resources = 1)
        {
            if (this.acquiredResources - resources >= 0)
            {
                this.acquiredResources -= resources;
                return true;
            }

            return false;
        }

        public class LocalFactory : ISemaphoreFactory
        {
            public INonBlockingSemaphore Create(string name, int maxResources)
            {
                return new SimpleNonBlockingSemaphore(name, maxResources);
            }
        }
    }
}

namespace Craiel.UnityEssentials.Contracts
{
    /// <summary>
    /// Abstract factory for creating concrete instances of classes implementing <see cref="INonBlockingSemaphore"/>
    /// </summary>
    public interface ISemaphoreFactory
    {
        /// <summary>
        /// Creates a semaphore with the specified name and resources
        /// </summary>
        /// <param name="name">the name of the semaphore</param>
        /// <param name="maxResources"> the maximum number of resource</param>
        /// <returns>the newly created semaphore</returns>
        INonBlockingSemaphore Create(string name, int maxResources);
    }
}

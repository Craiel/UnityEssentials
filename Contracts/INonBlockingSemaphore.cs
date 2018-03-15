namespace Craiel.UnityEssentials.Contracts
{
    /// <summary>
    /// A counting semaphore that does not block the thread when the requested resource is not available.
    /// No actual resource objects are used; the semaphore just keeps a count of the number available and acts accordingly
    /// </summary>
    public interface INonBlockingSemaphore
    {
        /// <summary>
        /// Acquires the specified number of resources if they all are available
        /// </summary>
        /// <param name="resources">amount of resources to acquire</param>
        /// <returns>true if all the requested resources have been acquired; false otherwise</returns>
        bool Acquire(int resources = 1);

        /// <summary>
        /// Releases the specified number of resources returning it to this semaphore
        /// </summary>
        /// <param name="resources">amount of resources to release</param>
        /// <returns>true if all the requested resources have been released; false otherwise</returns>
        bool Release(int resources = 1);
    }
}

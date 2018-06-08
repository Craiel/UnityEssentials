namespace Craiel.UnityEssentials.Runtime.Contracts
{
    using Pool;

    /// <summary>
    /// Objects implementing this interface will have <see cref="Reset"/> called when passed to <see cref="BasePool{T}.Free"/>
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Resets the object for reuse. Object references should be nulled and fields may be set to default values
        /// </summary>
        void Reset();
    }
}

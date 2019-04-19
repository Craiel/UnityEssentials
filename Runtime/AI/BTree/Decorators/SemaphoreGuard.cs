namespace Craiel.UnityEssentials.Runtime.AI.BTree.Decorators
{
    using BTree;
    using Contracts;
    using Runtime.Contracts;

    /// <summary>
    /// A <see cref="SemaphoreGuard{T}"/> decorator allows you to specify how many characters should be allowed to concurrently execute its
    /// child which represents a limited resource used in different behavior trees(note that this does not necessarily involve
    /// multithreading concurrency).
    /// <para></para>
    /// This is a simple mechanism for ensuring that a limited shared resource is not over subscribed.You might have a pool of 5
    /// pathfinders, for example, meaning at most 5 characters can be pathfinding at a time.Or you can associate a semaphore to the
    /// player character to ensure that at most 3 enemies can simultaneously attack him.
    /// <para></para>
    /// This decorator fails when it cannot acquire the semaphore.This allows a selector task higher up the tree to find a different
    /// action that doesn't involve the contested resource
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    public class SemaphoreGuard<T> : Decorator<T>
        where T : IBlackboard
    {
        private INonBlockingSemaphore semaphore;
        private bool semaphoreAcquired;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SemaphoreGuard()
        {
        }

        public SemaphoreGuard(TaskId child)
            : base(child)
        {
        }

        public SemaphoreGuard(string name)
        {
            this.Name = name;
        }

        public SemaphoreGuard(string name, TaskId child)
            : base(child)
        {
            this.Name = name;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }

        public override void Start()
        {
            if (this.semaphore == null)
            {
                this.semaphore = NonBlockingSemaphoreRepository.Get(this.Name);
            }

            this.semaphoreAcquired = this.semaphore.Acquire();
            base.Start();
        }

        public override void Run()
        {
            if (this.semaphoreAcquired)
            {
                base.Run();
            }
            else
            {
                this.Fail();
            }
        }

        public override void End()
        {
            if (this.semaphoreAcquired)
            {
                this.semaphore.Release();
                this.semaphoreAcquired = false;
            }

            base.End();
        }

        public override void Reset()
        {
            base.Reset();
            this.semaphore = null;
            this.semaphoreAcquired = false;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void CopyTo(Task<T> clone)
        {
            SemaphoreGuard<T> guard = (SemaphoreGuard<T>)clone;
            guard.Name = this.Name;
        }
    }
}

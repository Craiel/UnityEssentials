namespace Craiel.UnityEssentials.Runtime.AI.BTree
{
    using System;
    using System.Collections.Generic;
    using Contracts;
    using Enums;
    using Exceptions;
    using Runtime.Exceptions;

    /// <summary>
    /// Creates a new BehaviorStream object that is used to execute and maintain the tree at runtime
    /// </summary>
    /// <typeparam name="T">The type of <see cref="IBlackboard"/> this stream is using</typeparam>
    public class BehaviorStream<T> : Task<T>
        where T : IBlackboard
    {
        private readonly IList<IListener<T>> listeners;

        private T blackboard;

        private TaskId idPool = TaskId.FirstValid;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Creates a empty <see cref="BehaviorStream{T}"/> object.
        /// </summary>
        public BehaviorStream() 
            : this(default(T))
        {
        }

        /// <summary>
        /// Creates a <see cref="BehaviorStream{T}"/> with a <see cref="IBlackboard"/> object.
        /// </summary>
        /// <param name="blackboard">the <see cref="IBlackboard"/>. It can be null</param>
        /// <param name="initialSize">initial size of the stream</param>
        /// <param name="growBy">the amount of elements to grow by if we exceed the size</param>
        public BehaviorStream(T blackboard = default(T), int initialSize = 11, int growBy = 10)
        {
            this.listeners = new List<IListener<T>>();
            this.stream = new Task<T>[initialSize];

            // Make the stream part of itself
            this.Id = this.idPool;
            this.Stream = this;
            this.stream[this.Id.Value] = this;

            this.blackboard = blackboard;
            this.GuardEvaluator = this.Add(new GuardEvaluator<T>(this));
            this.GrowBy = growBy;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool HasListeners { get; private set; }

        public TaskId GuardEvaluator { get; private set; }

        public TaskId Root { get; set; }

        public int GrowBy { get; set; }

        public override int ChildCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// This method should be called when game entity needs to make decisions: call this in game loop or after a fixed time slice if
        /// the game is real-time, or on entity's turn if the game is turn-based
        /// </summary>
        public void Step()
        {
            if (this.Root == TaskId.Invalid)
            {
                throw new IllegalStateException("Behavior Stream root is not set!");
            }
            
            this.CurrentTaskToRun = new BehaviorStreamTaskToRun(this.Root, this.Id);
            while (this.ContinueRun())
            {
            }
        }

        /// <summary>
        /// Adds a new listener which will receive events defined in <see cref="IListener{T}"/>
        /// </summary>
        /// <param name="listener">the listener to add</param>
        public void AddListener(IListener<T> listener)
        {
            this.listeners.Add(listener);
            this.HasListeners = true;
        }

        /// <summary>
        /// Removes a listener
        /// </summary>
        /// <param name="listener">the listener to remove</param>
        public void RemoveListener(IListener<T> listener)
        {
            this.listeners.Remove(listener);
            this.HasListeners = this.listeners.Count > 0;
        }

        /// <summary>
        /// Removes all listeners from the tree
        /// </summary>
        public void RemoveListeners()
        {
            this.listeners.Clear();
            this.HasListeners = false;
        }

        /// <summary>
        /// Notifies all listeners of a child being added
        /// </summary>
        /// <param name="task">the added task</param>
        /// <param name="index">index the task was added at</param>
        public void NotifyChildAdded(TaskId task, int index)
        {
            for (var i = 0; i < this.listeners.Count; i++)
            {
                this.listeners[i].ChildAdded(task, index);
            }
        }

        /// <summary>
        /// Notifies all listeners of a task's status getting updated
        /// </summary>
        /// <param name="task">task of which the status changed</param>
        /// <param name="previousState">the previous <see cref="BTTaskStatus"/></param>
        public void NotifyStatusUpdated(TaskId task, BTTaskStatus previousState)
        {
            for (var i = 0; i < this.listeners.Count; i++)
            {
                this.listeners[i].StatusUpdated(task, previousState);
            }
        }

        /// <summary>
        /// Returns the task for the given task id
        /// </summary>
        /// <param name="taskId">the task id</param>
        /// <returns>the <see cref="Task{T}"/> of the given id</returns>
        public Task<T> Get(TaskId taskId)
        {
            return this.stream[taskId.Value];
        }

        public TaskId Add(Task<T> task)
        {
            if (task == null)
            {
                throw new ArgumentException("Tried to add null task");
            }

            if (task.Id != TaskId.Invalid)
            {
                throw new IllegalStateException("Task added to stream with existing state!");
            }

            this.idPool = this.idPool.GetNext();
            task.Id = new TaskId(this.idPool.Value);

            this.EnsureSize(task.Id);
            this.stream[task.Id.Value] = task;
            return task.Id;
        }
        
        public override TaskId GetChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentException("Invalid index for GetChild() on BehaviorStream");
            }

            return this.Root;
        }

        public override void Run()
        {
        }

        public override void ChildRunning(TaskId task, TaskId reporter)
        {
            this.Running();
        }

        public override void ChildSuccess(TaskId task)
        {
            this.Success();
        }

        public override void ChildFail(TaskId task)
        {
            this.Fail();
        }

        public T GetBlackboard()
        {
            return this.blackboard;
        }

        public void SetBlackboard(T newBlackboard)
        {
            this.blackboard = newBlackboard;
        }
        
        // -------------------------------------------------------------------
        // Internal
        // -------------------------------------------------------------------
        internal Task<T>[] stream;

        internal BehaviorStreamTaskToRun? CurrentTaskToRun { get; set; }

        internal struct BehaviorStreamTaskToRun
        {
            public readonly TaskId TaskToRun;
            public readonly TaskId Control;

            public BehaviorStreamTaskToRun(TaskId task, TaskId control)
            {
                this.TaskToRun = task;
                this.Control = control;
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int AddChildToTask(TaskId child)
        {
            throw new NotImplementedException();
        }

        protected override void CopyTo(Task<T> clone)
        {
            throw new NotImplementedException();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void EnsureSize(TaskId id)
        {
            if (this.stream.Length > id.Value)
            {
                return;
            }

            // Grow the array
            Array.Resize(ref this.stream, this.stream.Length + this.GrowBy);
        }

        private bool ContinueRun()
        {
            if (this.CurrentTaskToRun == null)
            {
                return false;
            }

            BehaviorStreamTaskToRun currentRun = this.CurrentTaskToRun.Value;
            Task<T> currentTask = this.Get(currentRun.TaskToRun);
            this.CurrentTaskToRun = null;

            if (currentTask.Status == BTTaskStatus.Running)
            {
                currentTask.Run();
            }
            else
            {
                currentTask.SetControl(currentRun.Control, this);
                currentTask.Start();
                if (currentTask.CheckGuard(currentRun.Control))
                {
                    currentTask.Run();
                }
                else
                {
                    currentTask.Fail();
                }
            }

            return true;
        }
    }
}

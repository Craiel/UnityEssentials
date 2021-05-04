namespace Craiel.UnityEssentials.Runtime.AI.BTree
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Contracts;
    using Enums;
    using Exceptions;
    using GDX.AI.Sharp.Runtime.BTree;
    using Runtime.Contracts;
    using Runtime.Enums;
    using Runtime.Exceptions;
    using Runtime.Utils;
    using UnityEngine;
    using Utils;

    /// <summary>
    /// This is the abstract base class of all behavior tree tasks. The Task of a behavior tree has a status, one control and a list of children.
    /// </summary>
    /// <typeparam name="T">type of the blackboard object that tasks use to read or modify game state</typeparam>
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
    public abstract class Task<T> : IYamlSerializable
        where T : IBlackboard
    {
        public const ushort InvalidTaskId = 0;
        public const ushort FirstValidTaskId = 1;

        private float lastRunTime;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        /// <summary>
        /// Unique Id for this node in the tree, created by the Behavior Tree Builder
        /// </summary>
        public TaskId Id { get; set; }

        /// <summary>
        /// Gets the number of children of this task
        /// </summary>
        public abstract int ChildCount { get; }

        /// <summary>
        /// Gets or sets the parent of this task
        /// </summary>
        public TaskId Control { get; protected set; }

        /// <summary>
        /// Gets or sets The guard of this task
        /// </summary>
        public TaskId Guard { get; set; }

        /// <summary>
        /// Gets or sets the behavior stream this task belongs to
        /// </summary>
        public BehaviorStream<T> Stream { get; protected set; }

        /// <summary>
        /// Gets or sets the status of this task
        /// </summary>
        public BTTaskStatus Status { get; protected set; }

        /// <summary>
        /// Gets the blackboard object of the behavior tree this task belongs to
        /// </summary>
        /// <exception cref="IllegalStateException">if this task has never run</exception>
        public T Blackboard
        {
            get
            {
                if (this.Stream == null)
                {
                    throw new IllegalStateException("Task has not ben run");
                }

                return this.Stream.GetBlackboard();
            }
        }

        /// <summary>
        /// This method will add a child to the list of this task's children
        /// </summary>
        /// <param name="child">the child task which will be added</param>
        /// <returns>the index where the child has been added</returns>
        /// <exception cref="IllegalStateException">if the child cannot be added for whatever reason</exception>
        public int AddChild(TaskId child)
        {
            int index = this.AddChildToTask(child);

            if (this.Stream != null && this.Stream.HasListeners)
            {
                this.Stream.NotifyChildAdded(this.Id, index);
            }

            return index;
        }

        /// <summary>
        /// Returns the child at the given index
        /// </summary>
        /// <param name="index">index of the child</param>
        /// <returns>the child task at the specified index</returns>
        public abstract TaskId GetChild(int index);

        /// <summary>
        /// This method will set a task as this task's control (parent)
        /// </summary>
        /// <param name="parent">the parent task</param>
        /// <param name="parentStream">the stream of the parent task</param>
        public void SetControl(TaskId parent, BehaviorStream<T> parentStream)
        {
            this.Control = parent;
            this.Stream = parentStream;
        }

        /// <summary>
        /// Checks the guard of this task
        /// </summary>
        /// <param name="controlId">the parent task</param>
        /// <returns>true if guard evaluation succeeds or there's no guard; false otherwise</returns>
        /// <exception cref="IllegalStateException">if guard evaluation returns any status other than Succeeded or Failed (<see cref="BTTaskStatus"/>)</exception>
        public bool CheckGuard(TaskId controlId)
        {
            if (this.Guard == TaskId.Invalid)
            {
                return true;
            }

            Task<T> guard = this.Stream.Get(this.Guard);
            Task<T> control = this.Stream.Get(controlId);

            // Guard of guard check, recursive
            if (!guard.CheckGuard(controlId))
            {
                return false;
            }

            guard.SetControl(control.Stream.GuardEvaluator, control.Stream);
            guard.Start();
            guard.Run();
            switch (guard.Status)
            {
                case BTTaskStatus.Succeeded:
                    {
                        return true;
                    }

                case BTTaskStatus.Failed:
                    {
                        return false;
                    }

                default:
                    {
                        throw new IllegalStateException(string.Format("Illegal guard status: {0}. Guards should succeed or fail in one step", guard.Status));
                    }
            }
        }

        /// <summary>
        /// This method will be called once before this task's first run
        /// </summary>
        public virtual void Start()
        {
            if (Math.Abs(this.lastRunTime - Time.time) < EssentialMathUtils.Epsilon)
            {
                throw new IllegalStateException("Task was started multiple times in the same frame!");
            }

            this.lastRunTime = Time.time;
        }

        /// <summary>
        /// This method will be called by <see cref="Success"/>, <see cref="Fail"/> or <see cref="Cancel"/>, 
        /// meaning that this task's status has just been set to Succeeded, Failed or Cancelled respectively (<see cref="BTTaskStatus"/>)
        /// </summary>
        public virtual void End()
        {
        }

        /// <summary>
        /// This method contains the update logic of this task. The actual implementation MUST call <see cref="Running"/>, <see cref="Success"/> or <see cref="Fail"/> exactly once
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// This method will be called when one of the ancestors of this task needs to run again
        /// </summary>
        /// <param name="task">the task that needs to run again</param>
        /// <param name="reporter">the task that reports, usually one of this task's children</param>
        public abstract void ChildRunning(TaskId task, TaskId reporter);

        /// <summary>
        /// This method will be called when one of the children of this task succeeds
        /// </summary>
        /// <param name="task">the task that succeeded</param>
        public abstract void ChildSuccess(TaskId task);

        /// <summary>
        /// This method will be called when one of the children of this task fails
        /// </summary>
        /// <param name="task">the task that failed</param>
        public abstract void ChildFail(TaskId task);

        /// <summary>
        /// This method will be called in <see cref="Run"/> to inform control that this task needs to run again
        /// </summary>
        public virtual void Running()
        {
            BTTaskStatus previous = this.Status;
            this.Status = BTTaskStatus.Running;
            if (this.Stream.HasListeners)
            {
                this.Stream.NotifyStatusUpdated(this.Id, previous);
            }

            if (this.Control == TaskId.Invalid)
            {
                return;
            }

            Task<T> control = this.Stream.Get(this.Control);
            control.ChildRunning(this.Id, this.Id);
        }

        /// <summary>
        /// This method will be called in <see cref="Run"/> to inform control that this task has finished running with a success result
        /// </summary>
        public virtual void Success()
        {
            BTTaskStatus previous = this.Status;
            this.Status = BTTaskStatus.Succeeded;
            if (this.Stream.HasListeners)
            {
                this.Stream.NotifyStatusUpdated(this.Id, previous);
            }

            this.End();

            if (this.Control == TaskId.Invalid)
            {
                return;
            }

            Task<T> control = this.Stream.Get(this.Control);
            control.ChildSuccess(this.Id);
        }

        /// <summary>
        /// This method will be called in <see cref="Run"/> to inform control that this task has finished running with a failure result
        /// </summary>
        public virtual void Fail()
        {
            BTTaskStatus previous = this.Status;
            this.Status = BTTaskStatus.Failed;
            if (this.Stream.HasListeners)
            {
                this.Stream.NotifyStatusUpdated(this.Id, previous);
            }

            this.End();

            if (this.Control == TaskId.Invalid)
            {
                return;
            }

            Task<T> control = this.Stream.Get(this.Control);
            control.ChildFail(this.Id);
        }

        /// <summary>
        /// Terminates this task and all its running children. This method MUST be called only if this task is running
        /// </summary>
        public virtual void Cancel()
        {
            this.CancelRunningChildren(0);
            BTTaskStatus previous = this.Status;
            this.Status = BTTaskStatus.Canceled;
            if (this.Stream.HasListeners)
            {
                this.Stream.NotifyStatusUpdated(this.Id, previous);
            }

            this.End();
        }

        /// <summary>
        /// Resets this task to make it restart from scratch on next run
        /// </summary>
        public virtual void Reset()
        {
            if (this.Status == BTTaskStatus.Running)
            {
                this.Cancel();
            }

            for (var i = 0; i < this.ChildCount; i++)
            {
                this.Stream.Get(this.GetChild(i)).Reset();
            }

            this.Status = BTTaskStatus.Fresh;
            this.Stream = null;
            this.Control = TaskId.Invalid;
        }

        /// <summary>
        /// Clones this task to a new one. If you don't specify a clone strategy through <see cref="TaskCloner"/> the new task is instantiated via reflection and <see cref="CopyTo"/> is invoked
        /// </summary>
        /// <returns>the cloned task</returns>
        /// <exception cref="TaskCloneException">if the task cannot be successfully cloned</exception>
        public virtual Task<T> Clone()
        {
            if (TaskCloner.Current != null)
            {
                try
                {
                    return TaskCloner.Current.CloneTask(this);
                }
                catch (Exception e)
                {
                    throw new TaskCloneException(e);
                }
            }

            try
            {
                Task<T> clone = (Task<T>)Activator.CreateInstance(this.GetType());
                this.CopyTo(clone);
                clone.Guard = this.Guard;

                return clone;
            }
            catch (Exception e)
            {
                throw new TaskCloneException(e);
            }
        }

        public virtual void Serialize(YamlFluentSerializer serializer)
        {
            serializer.Begin(YamlContainerType.Dictionary)
                .Add("Id", this.Id.Value);

            if (this.Guard != TaskId.Invalid)
            {
                serializer.Add("Guard", this.Guard.Value);
            }

            serializer.End();
        }

        public virtual void Deserialize(YamlFluentDeserializer deserializer)
        {
            deserializer.BeginRead();

            ushort id;
            deserializer.Read("Id", out id);
            this.Id = new TaskId(id);

            deserializer.Read("Guard", out id);
            this.Guard = new TaskId(id);

            deserializer.EndRead();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------

        /// <summary>
        /// This method will add a child to the list of this task's children
        /// </summary>
        /// <param name="child">the child task which will be added</param>
        /// <returns>the index where the child has been added</returns>
        /// <exception cref="IllegalStateException">if the child cannot be added for whatever reason</exception>
        protected abstract int AddChildToTask(TaskId child);

        /// <summary>
        /// Copies this task to the given task. This method is invoked by CloneTask only if <see cref="TaskCloner"/> is null which is its default value
        /// </summary>
        /// <param name="clone">the task to be filled</param>
        protected abstract void CopyTo(Task<T> clone);

        /// <summary>
        /// Terminates the running children of this task starting from the specified index up to the end
        /// </summary>
        /// <param name="startIndex"> The start Index. </param>
        protected virtual void CancelRunningChildren(int startIndex)
        {
            for (var i = startIndex; i < this.ChildCount; i++)
            {
                TaskId child = this.GetChild(i);
                Task<T> childTask = this.Stream.Get(child);
                if (childTask.Status == BTTaskStatus.Running)
                {
                    childTask.Cancel();
                }
            }
        }
    }
}

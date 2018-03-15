namespace Craiel.UnityEssentials.Threading
{
    using System;
    using System.Collections.Generic;
    using Enums;
    using JetBrains.Annotations;
    using Scene;
    using Singletons;

    public class UnitySynchronizationDispatcher : UnitySingletonBehavior<UnitySynchronizationDispatcher>
    {
        private static readonly Queue<Action> Tasks = new Queue<Action>();
        private static readonly IList<Action> TaskCache = new List<Action>();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void InvokeLater(Action task)
        {
            lock (Tasks)
            {
                Tasks.Enqueue(task);
            }
        }

        public override void Initialize()
        {
            this.RegisterInController(SceneObjectController.Instance, SceneRootCategory.System, true);

            base.Initialize();
        }

        [UsedImplicitly]
        public void FixedUpdate()
        {
            TaskCache.Clear();
            lock (Tasks)
            {
                while (Tasks.Count > 0)
                {
                    TaskCache.Add(Tasks.Dequeue());
                }
            }

            foreach (Action action in TaskCache)
            {
                action.Invoke();
            }
        }
    }
}

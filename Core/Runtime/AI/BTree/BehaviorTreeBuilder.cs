namespace Craiel.GDX.AI.Sharp.Runtime.BTree
{
    using System.Collections.Generic;
    using UnityEssentials.Runtime.AI.BTree;
    using UnityEssentials.Runtime.AI.BTree.Branches;
    using UnityEssentials.Runtime.AI.BTree.Contracts;
    using UnityEssentials.Runtime.AI.BTree.Decorators;
    using UnityEssentials.Runtime.AI.BTree.Exceptions;
    using UnityEssentials.Runtime.AI.BTree.Leafs;
    using UnityEssentials.Runtime.Mathematics.Rnd;

    /// <summary>
    /// Helper class to build a behavior tree using the fluent API
    /// </summary>
    public class BehaviorTreeBuilder<T>
        where T : IBlackboard
    {
        private readonly Stack<TaskId> parentStack;
        private readonly BehaviorStream<T> stream;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BehaviorTreeBuilder()
        {
            this.parentStack = new Stack<TaskId>();
            this.stream = new BehaviorStream<T>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public TaskId CurrentParent { get; private set; }

        public TaskId CurrentDecorator { get; private set; }

        public TaskId CurrentLeaf { get; private set; }

        public static BehaviorTreeBuilder<T> Begin()
        {
            return new BehaviorTreeBuilder<T>();
        }

        public BehaviorTreeBuilder<T> Branch(BranchTask<T> task)
        {
            this.CurrentParent = this.stream.Add(task);

            if (this.CurrentDecorator != TaskId.Invalid)
            {
                // Decorate the branch and close the decorator out
                Decorator<T> decorator = (Decorator<T>)this.stream.Get(this.CurrentDecorator);
                decorator.AddChild(this.CurrentParent);

                this.CurrentDecorator = TaskId.Invalid;
            }
            else if (this.parentStack.Count > 0)
            {
                // Add as a sub-branch
                BranchTask<T> parent = (BranchTask<T>)this.stream.Get(this.parentStack.Peek());
                parent.AddChild(this.CurrentParent);
            }
            
            this.parentStack.Push(this.CurrentParent);
            this.CurrentLeaf = TaskId.Invalid;
            return this;
        }

        public BehaviorTreeBuilder<T> DynamicGuardSelector()
        {
            return this.Branch(new DynamicGuardSelector<T>());
        }

        public BehaviorTreeBuilder<T> Parallel()
        {
            return this.Branch(new Parallel<T>());
        }

        public BehaviorTreeBuilder<T> RandomSelector()
        {
            return this.Branch(new RandomSelector<T>());
        }

        public BehaviorTreeBuilder<T> RandomSequence()
        {
            return this.Branch(new RandomSequence<T>());
        }

        public BehaviorTreeBuilder<T> Selector()
        {
            return this.Branch(new Selector<T>());
        }

        public BehaviorTreeBuilder<T> Sequence()
        {
            return this.Branch(new Sequence<T>());
        }

        public BehaviorTreeBuilder<T> Leaf(LeafTask<T> task)
        {
            if (this.CurrentDecorator != TaskId.Invalid)
            {
                Decorator<T> decorator = (Decorator<T>)this.stream.Get(this.CurrentDecorator);
                decorator.AddChild(this.stream.Add(task));

                this.CurrentDecorator = TaskId.Invalid;
                this.CurrentLeaf = task.Id;
                return this;
            }

            if (this.CurrentParent == TaskId.Invalid)
            {
                throw new BehaviorTreeBuilderException("No Root node defined yet, add a branch first");
            }

            BranchTask<T> parent = (BranchTask<T>)this.stream.Get(this.CurrentParent);
            parent.AddChild(this.stream.Add(task));

            this.CurrentLeaf = task.Id;
            return this;
        }

        public BehaviorTreeBuilder<T> Fail()
        {
            return this.Leaf(new Failure<T>());
        }

        public BehaviorTreeBuilder<T> Succeed()
        {
            return this.Leaf(new Success<T>());
        }

        public BehaviorTreeBuilder<T> Wait(float seconds)
        {
            return this.Leaf(new Wait<T>(seconds));
        }

        public BehaviorTreeBuilder<T> Action(ActionDelegate action = null)
        {
            return this.Leaf(new Action<T>(action));
        }

        public BehaviorTreeBuilder<T> Decorator(Decorator<T> task)
        {
            if (this.CurrentParent == TaskId.Invalid)
            {
                throw new BehaviorTreeBuilderException("No Root node defined yet, add a branch first");
            }

            BranchTask<T> parent = (BranchTask<T>)this.stream.Get(this.CurrentParent);
            parent.AddChild(this.stream.Add(task));

            if (task.ChildCount <= 0)
            {
                // This decorator has no child yet, wait for a fluent set of a leaf node
                this.CurrentDecorator = task.Id;
            }

            this.CurrentLeaf = TaskId.Invalid;
            return this;
        }

        public BehaviorTreeBuilder<T> AlwaysFail(Task<T> child = null)
        {
            if (child == null)
            {
                return this.Decorator(new AlwaysFail<T>());
            }

            return this.Decorator(new AlwaysFail<T>(this.stream.Add(child)));
        }

        public BehaviorTreeBuilder<T> AlwaysSucceed(Task<T> child = null)
        {
            if (child == null)
            {
                return this.Decorator(new AlwaysSucceed<T>());
            }

            return this.Decorator(new AlwaysSucceed<T>(this.stream.Add(child)));
        }

        public BehaviorTreeBuilder<T> Include(string subTree = null, bool lazy = false)
        {
            return this.Decorator(new Include<T>(subTree, lazy));
        }

        public BehaviorTreeBuilder<T> Invert(Task<T> child = null)
        {
            if (child != null)
            {
                return this.Decorator(new Invert<T>(this.stream.Add(child)));
            }

            return this.Decorator(new Invert<T>());
        }

        public BehaviorTreeBuilder<T> Random(Task<T> child = null)
        {
            if (child != null)
            {
                return this.Decorator(new Random<T>(this.stream.Add(child)));
            }

            return this.Decorator(new Random<T>());
        }

        public BehaviorTreeBuilder<T> Repeat(IntegerDistribution times, Task<T> child = null)
        {
            if (child != null)
            {
                return this.Decorator(new Repeat<T>(this.stream.Add(child), times));
            }

            return this.Decorator(new Repeat<T> { Times = times });
        }

        public BehaviorTreeBuilder<T> SemaphoreGuard(string name = null, Task<T> child = null)
        {
            if (child != null)
            {
                return this.Decorator(new SemaphoreGuard<T>(name, this.stream.Add(child)));
            }

            return this.Decorator(new SemaphoreGuard<T>(name));
        }

        public BehaviorTreeBuilder<T> UntilFail(Task<T> child = null)
        {
            if (child != null)
            {
                return this.Decorator(new UntilFail<T>(this.stream.Add(child)));
            }

            return this.Decorator(new UntilFail<T>());
        }

        public BehaviorTreeBuilder<T> UntilSuccess(Task<T> child = null)
        {
            if (child != null)
            {
                return this.Decorator(new UntilSuccess<T>(this.stream.Add(child)));
            }

            return this.Decorator(new UntilSuccess<T>());
        }

        public BehaviorTreeBuilder<T> Interval(float seconds = 1.0f, Task<T> child = null)
        {
            if (child != null)
            {
                return this.Decorator(new Interval<T>(this.stream.Add(child), seconds));
            }

            return this.Decorator(new Interval<T>(seconds));
        }

        public BehaviorTreeBuilder<T> End()
        {
            if (this.parentStack.Count <= 1)
            {
                throw new BehaviorTreeBuilderException("End() called on empty stack or root node");
            }

            this.parentStack.Pop();
            this.CurrentParent = this.parentStack.Peek();
            return this;
        }

        public BehaviorStream<T> Build(T blackboard = default(T))
        {
            if (this.parentStack.Count > 1)
            {
                throw new BehaviorTreeBuilderException("Build() called with open parent nodes, you are missing End() calls");
            }

            if (this.CurrentParent == TaskId.Invalid)
            {
                throw new BehaviorTreeBuilderException("Build() called on empty tree");
            }

            this.stream.SetBlackboard(blackboard);
            this.stream.Root = this.CurrentParent;
            return this.stream;
        }
    }
}

namespace Craiel.UnityEssentials.Runtime.AI.BTree.Utils
{
    using System.Collections.Generic;
    using BTree;
    using Contracts;

    public class BehaviorTreeLibrary
    {
        public BehaviorTreeLibrary()
        {
            this.Repository = new Dictionary<string, BehaviorStream<IBlackboard>>();
        }

        public IDictionary<string, BehaviorStream<IBlackboard>> Repository { get; private set; }
    }
}

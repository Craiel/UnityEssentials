namespace Craiel.UnityEssentials.Runtime.FSM
{
    using System.Collections.Generic;
    using Contracts;

    /// <summary>
    /// A <see cref="IStateMachine{T,TS}"/> implementation that keeps track of all previous <see cref="IState{T}"/> via a stack. This makes sense for example
    /// in case of a hierarchical menu structure where each menu screen is one state and one wants to navigate back to the main menu
    /// anytime, via <see cref="IStateMachine{T,TS}.RevertToPreviousState"/>.
    /// </summary>
    /// <typeparam name="T">the type of the entity owning this state machine</typeparam>
    /// <typeparam name="TS">the type of the states of this state machine</typeparam>
    public class StackStateMachine<T, TS> : DefaultStateMachine<T, TS>
        where T : class
        where TS : class, IState<T>
    {
        private Stack<TS> stateStack;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Creates a <see cref="StackStateMachine{T,TS}"/> with no owner, initial state and global state
        /// </summary>
        /// <param name="owner">the owner of the state machine</param>
        /// <param name="initialState">the initial state</param>
        /// <param name="globalState">the global state</param>
        public StackStateMachine(T owner = default(T), TS initialState = default(TS), TS globalState = default(TS))
            : base(owner, initialState, globalState)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override TS PreviousState
        {
            get
            {
                if (this.stateStack.Count == 0)
                {
                    return null;
                }

                return this.stateStack.Peek();
            }
        }

        public override void SetInitialState(TS state)
        {
            if (this.stateStack == null)
            {
                this.stateStack = new Stack<TS>();
            }

            this.stateStack.Clear();
            this.CurrentState = state;
        }

        public override void ChangeState(TS newState)
        {
            this.ChangeState(newState, true);
        }

        public override bool RevertToPreviousState()
        {
            if (this.stateStack.Count == 0)
            {
                return false;
            }

            TS previousState = this.stateStack.Pop();
            this.ChangeState(previousState, false);
            return true;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ChangeState(TS newState, bool pushCurrentStateToStack)
        {
            if (pushCurrentStateToStack && this.CurrentState != null)
            {
                this.stateStack.Push(this.CurrentState);
            }

            // Call the exit method of the existing state
            if (this.CurrentState != null)
            {
                this.CurrentState.Exit(this.Owner);
            }

            // Change state to the new state
            this.CurrentState = newState;

            // Call the entry method of the new state
            this.CurrentState.Enter(this.Owner);
        }
    }
}

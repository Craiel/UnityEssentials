namespace Craiel.UnityEssentials.Contracts
{
    /// <summary>
    /// A state machine manages the state transitions of its entity. 
    /// Additionally, the state machine may be delegated by the entity to handle its messages.
    /// </summary>
    /// <typeparam name="T">the type of the entity owning this state machine</typeparam>
    /// <typeparam name="TS">the type of the states of this state machine</typeparam>
    public interface IStateMachine<T, TS> : ITelegraph
        where T : class
        where TS : IState<T>
    {
        /// <summary>
        /// Returns the current state of this state machine
        /// </summary>
        TS CurrentState { get; }

        /// <summary>
        /// Gets or sets the global state of this state machine.
        /// Implementation classes should invoke the <see cref="Update"/> method of the global state every time the FSM is updated. 
        /// Also, they should never invoke its <see cref="IState{T}.Enter"/> and <see cref="IState{T}.Exit"/> method.
        /// </summary>
        TS GlobalState { get; set; }

        /// <summary>
        /// Returns the last state of this state machine
        /// </summary>
        TS PreviousState { get; }

        /// <summary>
        /// Updates the state machine
        /// Implementation classes should invoke first the <see cref="Update"/> method of the global state (if any) 
        /// then the <see cref="Update"/> method of the current state.
        /// </summary>
        void Update();

        /// <summary>
        /// Performs a transition to the specified state
        /// </summary>
        /// <param name="newState">the state to transition to</param>
        void ChangeState(TS newState);

        /// <summary>
        /// Changes the state back to the previous state
        /// <code>True</code> in case there was a previous state that we were able to revert to. 
        /// In case there is no previous state, no state change occurs and <code>False</code> will be returned
        /// </summary>
        /// <returns>true if the change was successful, false otherwise</returns>
        bool RevertToPreviousState();

        /// <summary>
        /// Sets the initial state of this state machine
        /// </summary>
        /// <param name="state">the initial state</param>
        void SetInitialState(TS state);
        
        /// <summary>
        /// Indicates whether the state machine is in the given state
        /// </summary>
        /// <param name="state">the state to be compared with the current state</param>
        /// <returns>true if the current state's type is equal to the type of the class passed as a parameter</returns>
        bool IsInState(TS state);
    }
}

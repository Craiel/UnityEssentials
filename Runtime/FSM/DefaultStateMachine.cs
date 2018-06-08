using Telegram = Craiel.UnityEssentials.Runtime.Msg.Telegram;

namespace Craiel.UnityEssentials.Runtime.FSM
{
    using Contracts;

    public class DefaultStateMachine<T, TS> : IStateMachine<T, TS>
        where T : class
        where TS : class, IState<T>
    {
        private TS previousState;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------

        /// <summary>
        /// Creates a <see cref="DefaultStateMachine{T,TS}"/> for the specified owner, initial state and global state
        /// </summary>
        /// <param name="owner">the owner of the state machine</param>
        /// <param name="initialState">the initial state</param>
        /// <param name="globalState">the global state</param>
        public DefaultStateMachine(T owner = default(T), TS initialState = default(TS), TS globalState = default(TS))
        {
            this.Owner = owner;
            this.SetInitialState(initialState);
            this.GlobalState = globalState;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        /// <summary>
        /// Gets or Sets the owner of this state machine
        /// </summary>
        public T Owner { get; set; }

        public TS CurrentState { get; protected set; }

        public TS GlobalState { get; set; }

        public virtual TS PreviousState
        {
            get
            {
                return this.previousState;
            }
        }

        public virtual void SetInitialState(TS state)
        {
            this.previousState = default(TS);
            this.CurrentState = state;
        }

        public void Update()
        {
            if (this.GlobalState != null)
            {
                this.GlobalState.Update(this.Owner);
            }

            if (this.CurrentState != null)
            {
                this.CurrentState.Update(this.Owner);
            }
        }

        public virtual void ChangeState(TS newState)
        {
            this.previousState = this.CurrentState;

            if (this.CurrentState != null)
            {
                this.CurrentState.Exit(this.Owner);
            }

            this.CurrentState = newState;

            if (this.CurrentState != null)
            {
                this.CurrentState.Enter(this.Owner);
            }
        }

        public virtual bool RevertToPreviousState()
        {
            if (this.PreviousState == null)
            {
                return false;
            }

            this.ChangeState(this.PreviousState);
            return true;
        }

        public bool IsInState(TS state)
        {
            return this.CurrentState == state;
        }
        
        public bool HandleMessage(Telegram telegram)
        {
            // First see if the current state is valid and that it can handle the message
            if (this.CurrentState != null && this.CurrentState.OnMessage(this.Owner, telegram))
            {
                return true;
            }

            // If not, and if a global state has been implemented, send
            // the message to the global state
            if (this.GlobalState != null && this.GlobalState.OnMessage(this.Owner, telegram))
            {
                return true;
            }

            return false;
        }
    }
}

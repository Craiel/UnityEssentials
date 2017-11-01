namespace Assets.Scripts.Craiel.Essentials.Input
{
    using System;
    using System.Collections.Generic;
    using Contracts;
    using Enums;
    using Essentials;
    using Scene;

    public class InputHandler : UnitySingletonBehavior<InputHandler>
    {
        private static readonly IDictionary<object, InputControlState> ControlState = new Dictionary<object, InputControlState>();

        private readonly Stack<IInputState> stateStack;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public InputHandler()
        {
            this.stateStack = new Stack<IInputState>();

            foreach (object controlEnum in Enum.GetValues(EssentialsCore.InputControlType))
            {
                ControlState.Add(controlEnum, new InputControlState());
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IInputState ActiveState
        {
            get
            {
                return this.stateStack.Peek();
            }
        }

        public override void Awake()
        {
            this.RegisterInController(SceneObjectController.Instance, SceneRootCategory.System, true);

            base.Awake();
        }

        public override void Initialize()
        {
            base.Initialize();

            this.SwitchState(EssentialsCore.DefaultInputState);
        }

        public void Update()
        {
            ResetStates();
            this.stateStack.Peek().Update();
        }
        
        public InputControlState GetControl<T>(T control)
            where T : struct, IConvertible
        {
            return ControlState[control];
        }

        public InputControlState GetControl(object control)
        {
            return ControlState[control];
        }

        public void SwitchState(IInputState mode)
        {
            // Remove the current states, then push
            while (this.stateStack.Count > 0)
            {
                IInputState previousState = this.stateStack.Pop();
                previousState.Deactivate();
            }

            this.stateStack.Clear();

            this.PushState(mode);
        }

        public void PushState(IInputState state)
        {
            if (this.stateStack.Count > 0)
            {
                this.stateStack.Peek().Update();
                this.stateStack.Peek().Deactivate();
            }
            
            this.stateStack.Push(state);
            this.stateStack.Peek().Activate();
            this.stateStack.Peek().Update();
        }

        public void PopState()
        {
            if (this.stateStack.Count == 1)
            {
                throw new InvalidOperationException("Pop() called on InputHandler with only one entry, this is not allowed!");
            }

            // The update calls are important to ensure that the states are set properly
            this.stateStack.Peek().Update();
            this.stateStack.Peek().Deactivate();
            this.stateStack.Pop();
            this.stateStack.Peek().Activate();
            this.stateStack.Peek().Update();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void ResetStates()
        {
            foreach (object control in ControlState.Keys)
            {
                ControlState[control].Reset();
            }
        }
    }
}

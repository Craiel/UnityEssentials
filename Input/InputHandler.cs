namespace Craiel.UnityEssentials.Input
{
    using System;
    using System.Collections.Generic;
    using Contracts;
    using Enums;
    using NLog;
    using Scene;
    using Singletons;

    public class InputHandler : UnitySingletonBehavior<InputHandler>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();
        
        private static readonly IDictionary<InputControl, InputControlState> ControlState;

        private readonly Stack<IInputState> stateStack;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static InputHandler()
        {
            ControlState = new Dictionary<InputControl, InputControlState>();
        }
        
        public InputHandler()
        {
            this.stateStack = new Stack<IInputState>();
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

        public void RegisterControl(InputControl control)
        {
            if (string.IsNullOrEmpty(control.Id))
            {
                Logger.Error("Input Control invalid!");
                return;
            }

            InputControlState existing;
            if (ControlState.TryGetValue(control, out existing))
            {
                Logger.Error("Duplicate Input Control: " + control);
                return;
            }
            
            ControlState.Add(control, new InputControlState());
        }

        public void UnregisterControl(InputControl control)
        {
            ControlState.Remove(control);
        }

        public bool IsRegistered(InputControl control)
        {
            return ControlState.ContainsKey(control);
        }

        public InputControlState GetControl(InputControl control)
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
            foreach (InputControl control in ControlState.Keys)
            {
                ControlState[control].Reset();
            }
        }
    }
}

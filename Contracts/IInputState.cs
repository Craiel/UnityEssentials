namespace Assets.Scripts.Craiel.Essentials.Contracts
{
    using Enums;
    using Input;

    public interface IInputState
    {
        bool IsActive { get; }

        IInputState AddAxis(string axis);
        IInputState JoinWith(string axis);
        IInputState WithMode(InputAxisMode mode);
        IInputState For(InputControl target);
        
        void Activate();
        void Deactivate();

        void Update();
    }
}

namespace Assets.Scripts.Craiel.Essentials.Contracts
{
    using System;
    using Enums;

    public interface IInputState<T> : IInputState
        where T : struct, IConvertible
    {
        IInputState For(T target);
    }

    public interface IInputState
    {
        bool IsActive { get; }

        IInputState AddAxis(string axis);
        IInputState JoinWith(string axis);
        IInputState WithMode(InputAxisMode mode);
        IInputState For(object target);
        
        void Activate();
        void Deactivate();

        void Update();
    }
}

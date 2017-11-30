namespace Assets.Scripts.Craiel.Essentials.Contracts
{
    using Input;
    using UnityEngine;

    public interface IInputState
    {
        bool IsActive { get; }

        InputStateAxisMapping AddAxis(string axis);
        InputStateKeyMapping AddKey(KeyCode key);

        void Activate();
        void Deactivate();

        void Update();
    }
}

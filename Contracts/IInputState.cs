using InputStateAxisMapping = Craiel.UnityEssentials.Input.InputStateAxisMapping;
using InputStateKeyMapping = Craiel.UnityEssentials.Input.InputStateKeyMapping;

namespace Craiel.UnityEssentials.Contracts
{
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

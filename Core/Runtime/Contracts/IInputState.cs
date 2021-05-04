using InputStateAxisMapping = Craiel.UnityEssentials.Runtime.Input.InputStateAxisMapping;
using InputStateKeyMapping = Craiel.UnityEssentials.Runtime.Input.InputStateKeyMapping;

namespace Craiel.UnityEssentials.Runtime.Contracts
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

using Telegram = Craiel.UnityEssentials.Runtime.Msg.Telegram;

namespace Craiel.UnityAudio.Runtime.States
{
    using UnityEssentials.Runtime.Contracts;

    public class DynamicAudioSourceStateBase : IState<DynamicAudioSource>
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public virtual void Enter(DynamicAudioSource entity)
        {
        }

        public virtual void Update(DynamicAudioSource entity)
        {
        }

        public void Exit(DynamicAudioSource entity)
        {
        }

        public bool OnMessage(DynamicAudioSource entity, Telegram telegram)
        {
            return false;
        }
    }
}

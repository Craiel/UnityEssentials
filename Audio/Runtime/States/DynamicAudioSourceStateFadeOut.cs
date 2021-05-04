namespace Craiel.UnityAudio.Runtime.States
{
    using Enums;
    using UnityEngine;

    public class DynamicAudioSourceStateFadeOut : DynamicAudioSourceStateBase
    {
        public static readonly DynamicAudioSourceStateFadeOut Instance = new DynamicAudioSourceStateFadeOut();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Enter(DynamicAudioSource entity)
        {
            base.Enter(entity);

            entity.CurrentFadeTime = 0;
        }

        public override void Update(DynamicAudioSource entity)
        {
            base.Update(entity);

            entity.CurrentFadeTime += Time.deltaTime;
            if (entity.CurrentFadeTime < entity.Parameters.FadeOut)
            {
                SetVolume(entity, 1f - (entity.CurrentFadeTime / entity.Parameters.FadeOut));
                return;
            }

            SetVolume(entity, 0f);
            entity.SwitchState(DynamicAudioSourceState.Finished);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void SetVolume(DynamicAudioSource entity, float volume)
        {
            entity.Source.volume = volume;
        }
    }
}

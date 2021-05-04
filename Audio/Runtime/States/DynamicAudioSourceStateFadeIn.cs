namespace Craiel.UnityAudio.Runtime.States
{
    using Enums;
    using UnityEngine;

    public class DynamicAudioSourceStateFadeIn : DynamicAudioSourceStateBase
    {
        public static readonly DynamicAudioSourceStateFadeIn Instance = new DynamicAudioSourceStateFadeIn();

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
            if (entity.CurrentFadeTime < entity.Parameters.FadeIn)
            {
                SetVolumeAndPlay(entity, entity.CurrentFadeTime / entity.Parameters.FadeIn);
                return;
            }

            SetVolumeAndPlay(entity, 1f);
            entity.SwitchState(DynamicAudioSourceState.Playing);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void SetVolumeAndPlay(DynamicAudioSource entity, float volume)
        {
            entity.Source.volume = volume;
            if (!entity.Source.isPlaying)
            {
                entity.Source.Play();
            }
        }
    }
}

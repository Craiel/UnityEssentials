namespace Craiel.UnityAudio.Runtime.States
{
    using Enums;

    public class DynamicAudioSourceStatePlaying : DynamicAudioSourceStateBase
    {
        public static readonly DynamicAudioSourceStatePlaying Instance = new DynamicAudioSourceStatePlaying();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Update(DynamicAudioSource entity)
        {
            base.Update(entity);

            if (!entity.Source.isPlaying)
            {
                entity.SwitchState(DynamicAudioSourceState.Finished);
            }
        }
    }
}

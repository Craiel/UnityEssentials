namespace Craiel.UnityAudio.Runtime.UI
{
    using UnityEngine;
    using UnityGameData;
    using UnityGameData.Runtime;

    public abstract class UIAudioHelperBase : AudioAttachedBehavior
    {
        private GameDataId audioId;

        private AudioTicket audioTicket;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public GameDataRuntimeAudioRef Audio;

        public override void StopAllAudio()
        {
            AudioSystem.Instance.Stop(ref this.audioTicket);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void SetupAudio()
        {
            base.SetupAudio();

            if (this.Audio != null && this.Audio.IsValid())
            {
                this.audioId = GameRuntimeData.Instance.GetRuntimeId(this.Audio);
            }
        }

        protected override void ReleaseAudio()
        {
            base.ReleaseAudio();

            this.audioId = GameDataId.Invalid;
        }

        protected void PlayAudio()
        {
            this.StopAllAudio();
            if (this.audioId != GameDataId.Invalid)
            {
                this.audioTicket = AudioSystem.Instance.Play(this.audioId);
            }
        }
    }
}

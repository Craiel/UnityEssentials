namespace Craiel.UnityAudio.Runtime
{
    using UnityEngine;
    using UnityGameData;
    using UnityGameData.Runtime;

    public class AudioAreaEmitter : AudioAttachedBehavior
    {
        private GameDataId audioId;

        private AudioTicket audioTicket;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public GameObject Target;

        [SerializeField]
        public GameDataRuntimeAudioRef Audio;

        [SerializeField]
        public bool IsLocalized;

        [SerializeField]
        public bool ActivateOnTrigger = true;

        [SerializeField]
        public float FadeIn = 1.0f;

        [SerializeField]
        public float FadeOut = 1.0f;

        [SerializeField]
        public int Priority;

        [SerializeField]
        public bool IgnorePriority;
        
        public override void OnDestroy()
        {
            base.OnDestroy();

            this.Deactivate();
        }

        public void OnTriggerEnter(Collider triggerCollider)
        {
            if (this.Target != null && triggerCollider.gameObject != this.Target)
            {
                return;
            }

            this.Activate();
        }

        public void OnTriggerExit(Collider triggerCollider)
        {
            if (this.Target != null && triggerCollider.gameObject != this.Target)
            {
                return;
            }

            this.Deactivate();
        }

        public void Play()
        {
            this.StopAllAudio();

            var parameters = new AudioPlayParameters
            {
                FadeIn = this.FadeIn,
                FadeOut = this.FadeOut
            };

            if (this.IsLocalized)
            {
                this.audioTicket = AudioSystem.Instance.PlayStationary(this.gameObject.transform.position, this.audioId, parameters);
            }
            else
            {
                this.audioTicket = AudioSystem.Instance.Play(this.audioId, parameters);
            }
        }

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

            if (!this.ActivateOnTrigger)
            {
                this.Activate();
            }
        }

        protected override void ReleaseAudio()
        {
            this.Deactivate();

            base.ReleaseAudio();
            
            this.audioId = GameDataId.Invalid;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Activate()
        {
            if (this.Audio == null)
            {
                return;
            }

            AudioAreaSystem.Instance.Activate(this);
        }

        private void Deactivate()
        {
            if (this.Audio == null)
            {
                return;
            }

            AudioAreaSystem.Instance.Deactivate(this);
        }
    }
}
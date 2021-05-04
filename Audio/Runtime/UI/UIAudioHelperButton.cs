namespace Craiel.UnityAudio.Runtime.UI
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using UnityGameData;
    using UnityGameData.Runtime;

    public class UIAudioHelperButton : UIAudioHelperBase, IPointerEnterHandler, IPointerExitHandler
    {
        private GameDataId hoverAudioId;

        private AudioTicket hoverAudioTicket;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public Button Target;

        [SerializeField]
        public GameDataRuntimeAudioRef HoverAudio;

        public void Start()
        {
            this.Target.onClick.AddListener(this.OnTargetClick);
        }

        public override void OnDestroy()
        {
            this.Target.onClick.RemoveListener(this.OnTargetClick);

            base.OnDestroy();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.StopAllAudio();
            if (this.hoverAudioId != GameDataId.Invalid)
            {
                this.hoverAudioTicket = AudioSystem.Instance.Play(this.hoverAudioId);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.StopAllAudio();
        }

        public override void StopAllAudio()
        {
            base.StopAllAudio();

            AudioSystem.Instance.Stop(ref this.hoverAudioTicket);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void SetupAudio()
        {
            base.SetupAudio();

            if (this.HoverAudio != null && this.HoverAudio.IsValid())
            {
                this.hoverAudioId = GameRuntimeData.Instance.GetRuntimeId(this.HoverAudio);
            }
        }

        protected override void ReleaseAudio()
        {
            base.ReleaseAudio();
            
            this.hoverAudioId = GameDataId.Invalid;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnTargetClick()
        {
            this.PlayAudio();
        }
    }
}

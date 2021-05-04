namespace Craiel.UnityAudio.Runtime.UI
{
    public class UIAudioHelperPermanent : UIAudioHelperBase
    {
        private bool isStarted;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Update()
        {
            base.Update();
            
            if (this.isStarted || !this.DataLoaded)
            {
                return;
            }

            this.isStarted = true;
            this.PlayAudio();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void ReleaseAudio()
        {
            base.ReleaseAudio();

            this.isStarted = false;
        }
    }
}

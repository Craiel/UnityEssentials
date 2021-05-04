namespace Craiel.UnityEssentials.Runtime.TweenLite
{
    using UnityEngine;

    public delegate void TweenLiteFloatDelegate(float value);
    public delegate void TweenLiteFloatFinishedDelegate();

    public class TweenLiteFloat : TweenLiteNode
    {
        private readonly float start;
        private readonly float end;
        private readonly TweenLiteFloatDelegate callback;
        private readonly TweenLiteFloatFinishedDelegate finishedCallback;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TweenLiteFloat(float start, float end, TweenLiteFloatDelegate callback = null, TweenLiteFloatFinishedDelegate finishedCallback = null)
        {
            this.start = start;
            this.end = end;
            this.callback = callback;
            this.finishedCallback = finishedCallback;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override bool HasValidTarget()
        {
            return this.callback != null;
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoUpdate(float percent)
        {
            if (!this.HasValidTarget())
            {
                return;
            }

            float value = Mathf.Lerp(this.start, this.end, percent);
            this.callback?.Invoke(value);
        }
        
        protected override void Finish()
        {
            this.finishedCallback?.Invoke();
            this.IsFinished = true;
        }
    }
}
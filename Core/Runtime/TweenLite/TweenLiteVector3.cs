namespace Craiel.UnityEssentials.Runtime.TweenLite
{
    using UnityEngine;

    public delegate void TweenLiteVector3Delegate(Vector3 value);
    public delegate void TweenLiteVector3FinishedDelegate();

    public class TweenLiteVector3 : TweenLiteNode
    {
        private readonly Vector3 start;
        private readonly Vector3 end;
        private readonly TweenLiteVector3Delegate callback;
        private readonly TweenLiteVector3FinishedDelegate finishedCallback;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TweenLiteVector3(Vector3 start, Vector3 end, TweenLiteVector3Delegate callback = null, TweenLiteVector3FinishedDelegate finishedCallback = null)
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

            Vector3 value = Vector3.Lerp(this.start, this.end, percent);
            this.callback?.Invoke(value);
        }
        
        protected override void Finish()
        {
            this.finishedCallback?.Invoke();
            this.IsFinished = true;
        }
    }
}
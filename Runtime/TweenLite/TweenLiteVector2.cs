namespace Craiel.UnityEssentials.Runtime.TweenLite
{
    using UnityEngine;

    public delegate void TweenLiteVector2Delegate(Vector2 value);
    public delegate void TweenLiteVector2FinishedDelegate();

    public class TweenLiteVector2 : TweenLiteNode
    {
        private readonly Vector2 start;
        private readonly Vector2 end;
        private readonly TweenLiteVector2Delegate callback;
        private readonly TweenLiteVector2FinishedDelegate finishedCallback;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TweenLiteVector2(Vector2 start, Vector2 end, TweenLiteVector2Delegate callback = null, TweenLiteVector2FinishedDelegate finishedCallback = null)
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

            Vector2 value = Vector2.Lerp(this.start, this.end, percent);
            this.callback?.Invoke(value);
        }
        
        protected override void Finish()
        {
            this.finishedCallback?.Invoke();
            this.IsFinished = true;
        }
    }
}
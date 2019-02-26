namespace Craiel.UnityEssentials.Runtime.TweenLite
{
    using Enums;
    using UnityEngine;

    public delegate void TweenLiteColorDelegate(Color value);
    public delegate void TweenLiteColorFinishedDelegate();

    public class TweenLiteColor : TweenLiteNode
    {
        private readonly Color start;
        private readonly Color end;
        private readonly TweenLiteColorDelegate callback;
        private readonly TweenLiteColorFinishedDelegate finishedCallback;
        private readonly TweenLiteColorMode mode;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TweenLiteColor(Color start, Color end, TweenLiteColorDelegate callback = null, TweenLiteColorFinishedDelegate finishedCallback = null)
            : this(start, end, TweenLiteColorMode.All, callback, finishedCallback)
        {
        }
        
        public TweenLiteColor(Color start, Color end, TweenLiteColorMode mode = TweenLiteColorMode.All,
            TweenLiteColorDelegate callback = null, TweenLiteColorFinishedDelegate finishedCallback = null)
        {
            this.start = start;
            this.end = end;
            this.mode = mode;
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

            switch (this.mode)
            {
                case TweenLiteColorMode.Alpha:
                {
                    Color value = this.start;
                    value.a = Mathf.Lerp(this.start.a, this.end.a, percent);
                    this.callback?.Invoke(value);
                    break;
                }

                case TweenLiteColorMode.RGB:
                {
                    Color value = Color.Lerp(this.start, this.end, percent);
                    value.a = this.start.a;
                    this.callback?.Invoke(value);
                    break;
                }

                default:
                {
                    Color value = Color.Lerp(this.start, this.end, percent);
                    this.callback?.Invoke(value);
                    break;
                }
            }
        }
        
        protected override void Finish()
        {
            this.finishedCallback?.Invoke();
            this.IsFinished = true;
        }
    }
}
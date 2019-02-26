namespace Craiel.UnityEssentials.Runtime.TweenLite
{
    using EngineCore;
    using Enums;
    using UnityEngine;

    public abstract class TweenLiteNode : ITicketData
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected TweenLiteNode()
        {
            this.Ticket = TweenLiteTicket.Next();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        
        // Fields for performance
        public bool IgnoreTimeScale;
        public float Duration;
        public TweenLiteEasingMode Easing;
        
        public TweenLiteTicket Ticket { get; }
        public bool IsFinished { get; protected set; }
        public float Runtime { get; protected set; }

        public void Update()
        {
            float elapsed = this.IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            this.Runtime += elapsed;
            
            float percentage = TweenLiteSystem.ApplyEasing(this.Easing, elapsed, 0, 1, this.Duration);
            this.DoUpdate(percentage);

            if (this.Runtime >= this.Duration)
            {
                this.IsFinished = true;
                this.DoUpdate(1f);
                this.Finish();
            }
        }

        public abstract bool HasValidTarget();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        protected abstract void DoUpdate(float percentage);
        protected abstract void Finish();
    }
}
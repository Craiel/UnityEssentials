namespace Craiel.UnityEssentials.Runtime.TweenLite
{
    using System.Collections.Generic;
    using Collections;
    using EngineCore;
    using Enums;
    using Scene;
    using Singletons;
    using UnityEngine;

    public partial class TweenLiteSystem : UnitySingletonBehavior<TweenLiteSystem>
    {
        private readonly TicketProvider<TweenLiteTicket, TweenLiteNode> activeTweens;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TweenLiteSystem()
        {
            this.activeTweens = new TicketProvider<TweenLiteTicket, TweenLiteNode>();
            this.activeTweens.EnableManagedTickets(this.CheckTweenFinished, this.TweenFinished);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Awake()
        {
            SceneObjectController.Instance.RegisterObjectAsRoot(SceneRootCategory.System, this.gameObject, true);
            
            base.Awake();
        }

        public void StartTween(TweenLiteNode node)
        {
            this.activeTweens.Register(node.Ticket, node);
            this.activeTweens.Manage(node.Ticket);
        }

        public void StopTween(ref TweenLiteTicket ticket)
        {
            this.activeTweens.Unregister(ticket);
            ticket = TweenLiteTicket.Invalid;
        }

        public void Update()
        {
            this.activeTweens.Update();
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void TweenFinished(ref TweenLiteTicket ticket)
        {
            ticket = TweenLiteTicket.Invalid;
        }

        private bool CheckTweenFinished(TweenLiteTicket ticket)
        {
            if (this.activeTweens.TryGet(ticket, out TweenLiteNode data))
            {
                return data.IsFinished;
            }

            return true;
        }
    }
}
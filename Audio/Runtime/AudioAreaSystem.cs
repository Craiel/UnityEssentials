namespace Craiel.UnityAudio.Runtime
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEssentials.Runtime.Enums;
    using UnityEssentials.Runtime.Scene;
    using UnityEssentials.Runtime.Singletons;

    public class AudioAreaSystem : UnitySingletonBehavior<AudioAreaSystem>
    {
        private readonly IList<AudioAreaEmitter> activeEmitters;

        private int highestActivePriority;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public AudioAreaSystem()
        {
            this.activeEmitters = new List<AudioAreaEmitter>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Initialize()
        {
            SceneObjectController.Instance.RegisterObjectAsRoot(SceneRootCategory.System, this.gameObject, true);
            
            base.Initialize();
        }

        public void Activate(AudioAreaEmitter emitter)
        {
            if (this.activeEmitters.Contains(emitter))
            {
                return;
            }

            if (!emitter.IgnorePriority && this.activeEmitters.Count > 0)
            {
                if (this.activeEmitters.Any(x => x.Priority > emitter.Priority))
                {
                    // We have higher priority emitters, add this but do not play
                    this.activeEmitters.Add(emitter);
                    return;
                }

                // Deactivate emitters with a lower priority
                // NOTE: might be worth not using linq here, for now we will try and see how the performance goes
                IList<AudioAreaEmitter> lowerEmitters = this.activeEmitters.Where(x => x.Priority < emitter.Priority).ToList();
                foreach (AudioAreaEmitter activeEmitter in lowerEmitters)
                {
                    activeEmitter.StopAllAudio();
                }

                this.highestActivePriority = emitter.Priority;
            }

            this.activeEmitters.Add(emitter);
            emitter.Play();
        }

        public void Deactivate(AudioAreaEmitter emitter)
        {
            if (!this.activeEmitters.Contains(emitter))
            {
                return;
            }

            this.activeEmitters.Remove(emitter);
            emitter.StopAllAudio();

            if (this.activeEmitters.Count == 0)
            {
                this.highestActivePriority = 0;
                return;
            }

            int newHighestPriority = this.activeEmitters.Max(x => x.Priority);
            if (newHighestPriority < this.highestActivePriority)
            {
                this.activeEmitters.Where(x => x.Priority == newHighestPriority).ToList().ForEach(x => x.Play());
                this.highestActivePriority = newHighestPriority;
            }
        }
    }
}
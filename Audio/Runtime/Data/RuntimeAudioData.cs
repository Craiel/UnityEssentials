using AudioChannel = Craiel.UnityAudio.Runtime.Enums.AudioChannel;
using AudioFlags = Craiel.UnityAudio.Runtime.Enums.AudioFlags;
using AudioPlayBehavior = Craiel.UnityAudio.Runtime.Enums.AudioPlayBehavior;
using ResourceKey = Craiel.UnityEssentials.Runtime.Resource.ResourceKey;

namespace Craiel.UnityAudio.Runtime.Data
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityGameData;
    using UnityGameData.Runtime;

    [Serializable]
    public class RuntimeAudioData : RuntimeGameData
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public RuntimeAudioData()
        {
            this.Clips = new List<string>();
            this.ClipKeys = new List<ResourceKey>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public AudioChannel Channel;

        [SerializeField]
        public AudioPlayBehavior PlayBehavior;

        [SerializeField]
        public bool OnlyOneAtATime;

        [SerializeField]
        public AudioFlags Flags;

        [SerializeField]
        public List<string> Clips;

        public IList<ResourceKey> ClipKeys { get; private set; }

        public override void PostLoad()
        {
            base.PostLoad();

            foreach (string path in this.Clips)
            {
                this.ClipKeys.Add(ResourceKey.Create<AudioClip>(path));
            }
        }
    }
}

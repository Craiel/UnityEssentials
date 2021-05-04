namespace Craiel.UnityAudio.Runtime
{
    using System.Collections.Generic;
    using Data;
    using Enums;
    using UnityEngine;
    using UnityEngine.Audio;
    using UnityEssentials.Runtime.EngineCore;
    using UnityEssentials.Runtime.Enums;
    using UnityEssentials.Runtime.Resource;
    using UnityEssentials.Runtime.Scene;
    using UnityEssentials.Runtime.Singletons;
    using UnityGameData.Runtime;

    public class AudioSystem : UnitySingletonBehavior<AudioSystem>
    {
        private static readonly AudioPlayParameters DefaultPlayParameters = new AudioPlayParameters { UseRandomClip = true };
        
        private readonly DynamicAudioSourcePool dynamicAudioSourcePool;
        
        private readonly TicketProviderManaged<AudioTicket, DynamicAudioSource> activeAudio;
        
        private readonly IDictionary<GameDataId, IList<AudioTicket>> sourcesByDataMap;
        
        private AudioMixerController masterMixer;

        private AudioEventMapping eventMapping;
        
        private SceneObjectRoot audioPoolRoot;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public AudioSystem()
        {
            this.dynamicAudioSourcePool = new DynamicAudioSourcePool();
            this.sourcesByDataMap = new Dictionary<GameDataId, IList<AudioTicket>>();
            
            this.activeAudio = new TicketProviderManaged<AudioTicket, DynamicAudioSource>();
            this.activeAudio.EnableManagedTickets(this.IsFinished, this.Stop);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Awake()
        {
            SceneObjectController.Instance.RegisterObjectAsRoot(SceneRootCategory.System, this.gameObject, true);
            
            this.audioPoolRoot = SceneObjectController.Instance.AcquireRoot(SceneRootCategory.Dynamic, "Audio", true);

            base.Awake();
        }

        public override void Initialize()
        {
            base.Initialize();

            this.masterMixer = new AudioMixerController(AudioCore.MasterMixerResource);
            
            this.dynamicAudioSourcePool.Initialize(AudioCore.DynamicAudioSourceResource, this.UpdateAudioSource, this.audioPoolRoot.GetTransform());

            this.eventMapping = this.LoadEventMapping();
            
            AudioAreaSystem.InstantiateAndInitialize();

            AudioCore.Logger.Info("Audio Manager Initialized");
        }

        public void LateUpdate()
        {
            this.activeAudio.Update();
            
            this.dynamicAudioSourcePool.Update();
        }
        
        public bool IsFinished(AudioTicket ticket)
        {
            if (this.activeAudio.TryGet(ticket, out DynamicAudioSource source))
            {
                return !source.IsActive;
            }

            return true;
        }

        public AudioTicket Play(GameDataId id, AudioPlayParameters parameters = default (AudioPlayParameters))
        {
            var entry = GameRuntimeData.Instance.Get<RuntimeAudioData>(id);
            if (entry != null)
            {
                DynamicAudioSource source = this.PrepareAudioSource(entry);
                if (source == null)
                {
                    return AudioTicket.Invalid;
                }

                AudioMixerGroup group = this.masterMixer.GetChannel(entry.Channel);
                if (group == null)
                {
                    AudioCore.Logger.Warn("Invalid audio channel in clip {0}: {1}", entry.Id, entry.Channel);
                    return AudioTicket.Invalid;
                }

                var ticket = AudioTicket.Next();
                source.Play(ticket, entry, false, group, parameters);

                this.RegisterSource(ticket, source);
                return ticket;
            }

            return AudioTicket.Invalid;
        }

        public AudioTicket PlayAnchored(Transform anchorTransform, GameDataId id, AudioPlayParameters parameters = default(AudioPlayParameters))
        {
            var entry = GameRuntimeData.Instance.Get<RuntimeAudioData>(id);
            if (entry != null)
            {
                DynamicAudioSource source = this.PrepareAudioSource(entry);
                if (source == null)
                {
                    return AudioTicket.Invalid;
                }

                AudioMixerGroup channel = this.masterMixer.GetChannel(entry.Channel);
                if (channel == null)
                {
                    AudioCore.Logger.Warn("Invalid audio channel in clip {0}: {1}", entry.Id, entry.Channel);
                    return AudioTicket.Invalid;
                }

                var ticket = AudioTicket.Next();
                source.SetAnchor(anchorTransform);
                source.Play(ticket, entry, true, channel, parameters);

                this.RegisterSource(ticket, source);
                return ticket;
            }

            return AudioTicket.Invalid;
        }

        public AudioTicket PlayStationary(Vector3 position, GameDataId id, AudioPlayParameters parameters = default(AudioPlayParameters))
        {
            var entry = GameRuntimeData.Instance.Get<RuntimeAudioData>(id);
            if (entry == null)
            {
                return AudioTicket.Invalid;
            }
            
            DynamicAudioSource source = this.PrepareAudioSource(entry);
            if (source == null)
            {
                return AudioTicket.Invalid;
            }

            AudioMixerGroup channel = this.masterMixer.GetChannel(entry.Channel);
            if (channel == null)
            {
                AudioCore.Logger.Warn("Invalid audio channel in clip {0}: {1}", entry.Id, entry.Channel);
                return AudioTicket.Invalid;
            }

            var ticket = AudioTicket.Next();
            source.SetPosition(position);
            source.Play(ticket, entry, true, channel, parameters);

            this.RegisterSource(ticket, source);
            return ticket;
        }
        
        public void PlayAudioEventManaged(AudioEvent eventType)
        {
            this.PlayAudioEventManaged(eventType, DefaultPlayParameters);
        }
        
        public void PlayAudioEventManaged(AudioEvent eventType, AudioPlayParameters parameters)
        {
            AudioTicket ticket = this.PlayAudioEvent(eventType);
            if (ticket != AudioTicket.Invalid)
            {
                this.activeAudio.Manage(ticket);
            }
        }
        
        public AudioTicket PlayAudioEvent(AudioEvent eventType)
        {
            return this.PlayAudioEvent(eventType, DefaultPlayParameters);
        }

        public AudioTicket PlayAudioEvent(AudioEvent eventType, AudioPlayParameters parameters)
        {
            string audioDataGuid = this.eventMapping.Mapping[(int) eventType];
            if (string.IsNullOrEmpty(audioDataGuid))
            {
                AudioCore.Logger.Error("PlayAudioEvent Failed, No Guid Mapped for Event {0}", eventType);
                return AudioTicket.Invalid;
            }
            
            GameDataId audioDataId = GameRuntimeData.Instance.GetRuntimeId(audioDataGuid);
            if (audioDataId == GameDataId.Invalid)
            {
                AudioCore.Logger.Error("PlayAudioEvent Failed, DataId not found for {0}", audioDataGuid);
                return AudioTicket.Invalid;
            }

            return this.Play(audioDataId, parameters);
        }

        public void Stop(ref AudioTicket ticket)
        {
            if (this.activeAudio.TryGet(ticket, out DynamicAudioSource source))
            {
                source.Stop();
            }

            ticket = AudioTicket.Invalid;
        }

        public void StopByDataId(GameDataId id)
        {
            if (this.sourcesByDataMap.TryGetValue(id, out IList<AudioTicket> tickets))
            {
                for (var i = 0; i < tickets.Count; i++)
                {
                    AudioTicket ticket = tickets[i];
                    this.Stop(ref ticket);
                }

                tickets.Clear();
                this.sourcesByDataMap.Remove(id);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private DynamicAudioSource PrepareAudioSource(RuntimeAudioData entry)
        {
            if ((entry.Flags & AudioFlags.Unique) != 0)
            {
                if (this.sourcesByDataMap.ContainsKey(entry.Id))
                {
                    // Same audio is already playing and unique
                    return null;
                }
            }

            DynamicAudioSource source = this.dynamicAudioSourcePool.Obtain();
            source.gameObject.SetActive(true);
            return source;
        }

        private void RegisterSource(AudioTicket ticket, DynamicAudioSource source)
        {
            this.activeAudio.Register(ticket, source);

            IList<AudioTicket> ticketList;
            if (!this.sourcesByDataMap.TryGetValue(source.ActiveId, out ticketList))
            {
                ticketList = new List<AudioTicket>();
                this.sourcesByDataMap.Add(source.ActiveId, ticketList);
            }

            ticketList.Add(ticket);
        }

        private void UnregisterSource(DynamicAudioSource source)
        {
            IList<AudioTicket> ticketList;
            if (this.sourcesByDataMap.TryGetValue(source.ActiveId, out ticketList))
            {
                this.activeAudio.Unregister(source.Ticket);
                
                ticketList.Remove(source.Ticket);
                if (ticketList.Count == 0)
                {
                    this.sourcesByDataMap.Remove(source.ActiveId);
                }
            }
        }

        private bool UpdateAudioSource(DynamicAudioSource source)
        {
            if (source.IsActive)
            {
                return true;
            }

            this.UnregisterSource(source);
            return false;
        }
        
        private AudioEventMapping LoadEventMapping()
        {
            if (AudioCore.AudioEventMappingResource != ResourceKey.Invalid)
            {
                return AudioCore.AudioEventMappingResource.LoadManaged<AudioEventMapping>();
            }

            return ScriptableObject.CreateInstance<AudioEventMapping>();
        }
    }
}

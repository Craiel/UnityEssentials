namespace Craiel.UnityAudio.Runtime.Data
{
    using Enums;
    using UnityEngine;
    using UnityGameData.Runtime;

    [CreateAssetMenu(menuName = "Craiel/Audio/EventMapping")]
    public class AudioEventMapping : ScriptableObject
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public AudioEventMapping()
        {
            this.Mapping = new string[AudioEnumValues.AudioEventValues.Count];
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public string[] Mapping;
    }
}
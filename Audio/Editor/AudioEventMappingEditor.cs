namespace Craiel.UnityAudio.Editor
{
    using Runtime.Data;
    using Runtime.Enums;
    using UnityEditor;
    using UnityEngine;
    using UnityEssentials.Editor.UserInterface;
    using UnityGameData.Editor;

    [CustomEditor(typeof(AudioEventMapping))]
    public class AudioEventMappingEditor : Editor
    {
        private static readonly GameDataRefVirtualHolder[] refHolders;
        private static readonly SerializedObject[] refHolderObjects;

        private static Object activeTarget;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static AudioEventMappingEditor()
        {
            refHolders = new GameDataRefVirtualHolder[AudioEnumValues.AudioEventValues.Count];
            refHolderObjects = new SerializedObject[AudioEnumValues.AudioEventValues.Count];
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void OnInspectorGUI()
        {
            if (this.target != activeTarget)
            {
                this.Initialize();
            }
            
            this.serializedObject.Update();
            this.Draw();
            this.serializedObject.ApplyModifiedProperties();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Initialize()
        {
            SerializedProperty mappingProperty = this.serializedObject.FindProperty<AudioEventMapping>(x => x.Mapping);
            mappingProperty.arraySize = AudioEnumValues.AudioEventValues.Count;

            for (var i = 0; i < refHolders.Length; i++)
            {
                SerializedProperty element = mappingProperty.GetArrayElementAtIndex(i);
                
                refHolders[i] = CreateInstance<GameDataRefVirtualHolder>();
                refHolders[i].Ref = new GameDataAudioRef
                {
                    RefGuid = element.stringValue
                };
                
                refHolders[i].TypeFilter = typeof(GameDataAudio).Name;
                refHolderObjects[i] = new SerializedObject(refHolders[i]);
            }

            activeTarget = this.target;
        }
        
        private void Draw()
        {
            SerializedProperty mappingProperty = this.serializedObject.FindProperty<AudioEventMapping>(x => x.Mapping);
            mappingProperty.arraySize = AudioEnumValues.AudioEventValues.Count;

            for (var i = 0; i < mappingProperty.arraySize; i++)
            {
                SerializedProperty element = mappingProperty.GetArrayElementAtIndex(i);
                
                AudioEvent eventValue = (AudioEvent) i;
                if (eventValue == AudioEvent.Unknown)
                {
                    continue;
                }
                
                var prop = refHolderObjects[i].FindProperty<GameDataRefVirtualHolder>(x => x.Ref);
                EditorGUILayout.PropertyField(prop, new GUIContent(eventValue.ToString()));

                if (refHolders[i].Ref.RefGuid != element.stringValue)
                {
                    element.stringValue = refHolders[i].Ref.RefGuid;
                    EditorUtility.SetDirty(this.target);
                }
            }
        }
    }
}
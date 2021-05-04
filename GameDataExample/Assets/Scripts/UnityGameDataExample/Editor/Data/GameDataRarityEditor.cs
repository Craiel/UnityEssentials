namespace UnityGameDataExample.Data.Editor
{
    using Craiel.UnityEssentials.Editor.UserInterface;
    using Craiel.UnityGameData.Editor.Common;
    using UnityEditor;

    [CustomEditor(typeof(GameDataRarity))]
    [CanEditMultipleObjects]
    public class GameDataRarityEditor : GameDataObjectEditor
    {
        private static bool visualFoldout = true;
        private static bool propertiesFoldout = true;

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoDrawFull()
        {
            this.DrawVisual();
            this.DrawProperties();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DrawVisual()
        {
            if (this.DrawFoldout("Visual", ref visualFoldout))
            {
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty<GameDataRarity>(x => x.Color), true);
            }
        }

        private void DrawProperties()
        {
            if (this.DrawFoldout("Properties", ref propertiesFoldout))
            {
                GameDataRarity typedTarget = (GameDataRarity)this.target;
                
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty<GameDataRarity>(x => x.MinLevel), true);
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty<GameDataRarity>(x => x.Rarity), true);
                
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty<GameDataRarity>(x => x.CanDrop), true);
                if (typedTarget.CanDrop)
                {
                    EditorGUILayout.PropertyField(this.serializedObject.FindProperty<GameDataRarity>(x => x.DropRarityValue), true);
                }

                EditorGUILayout.PropertyField(this.serializedObject.FindProperty<GameDataRarity>(x => x.CanBuyFromVendor), true);
                if (typedTarget.CanBuyFromVendor)
                {
                    EditorGUILayout.PropertyField(this.serializedObject.FindProperty<GameDataRarity>(x => x.BuyRarityValue), true);
                }

                EditorGUILayout.PropertyField(this.serializedObject.FindProperty<GameDataRarity>(x => x.CurrencyMultiplier), true);
            }
        }
    }
}
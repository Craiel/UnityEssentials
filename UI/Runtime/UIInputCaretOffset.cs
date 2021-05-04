namespace Craiel.UnityEssentialsUI.Runtime
{
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(InputField))]
    public class UIInputCaretOffset : MonoBehaviour
    {
        private Vector2 initialPosition;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField] 
        public Vector2 Offset;
        
        public void OnTransformChildrenChanged()
        {
            Invoke("ApplyOffset", 0.5f);
        }

        public void ApplyOffset()
        {
            foreach (Transform child in this.transform)
            {
                if (child.gameObject.name.ToLower().Contains("caret"))
                {
                    RectTransform rect = (child as RectTransform);

                    this.initialPosition = rect.anchoredPosition;
                    rect.anchoredPosition = this.initialPosition + this.Offset;
                    break;
                }
            }
        }
    }
}
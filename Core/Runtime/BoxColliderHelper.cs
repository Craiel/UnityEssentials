namespace Craiel.UnityEssentials.Runtime
{
    using UnityEngine;

    [ExecuteInEditMode]
    public class BoxColliderHelper : MonoBehaviour
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField] 
        public bool SnapToGrid;

        [SerializeField]
        public Rect ColliderRect;

        public void OnEnable()
        {
            this.UpdateColliderRect();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void UpdateColliderRect()
        {
            var boxCollider = this.GetComponent<BoxCollider2D>();
            if (boxCollider == null)
            {
                return;
            }

            var position = new Vector2(this.transform.position.x, this.transform.position.y);
            this.ColliderRect = new Rect( position + boxCollider.offset, boxCollider.size);
        }
    }
}
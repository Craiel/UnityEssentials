namespace Craiel.UnityEssentialsUI.Runtime
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu(EssentialCoreUI.ComponentMenuFolder + "/Flippable")]
    [DisallowMultipleComponent]
    public class UIFlippable : MonoBehaviour, IMeshModifier
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField] 
        public bool Horizontal;
        
        [SerializeField] 
        public bool Vertical;
        
#if UNITY_EDITOR
        protected void OnValidate()
        {
            this.GetComponent<Graphic>().SetVerticesDirty();
        }
#endif
        
        public void ModifyMesh(VertexHelper vertexHelper)
        {
            if (!this.enabled)
            {
                return;
            }
			
            List<UIVertex> list = new List<UIVertex>();
            vertexHelper.GetUIVertexStream(list);
			
            this.ModifyVertices(list);
			
            vertexHelper.Clear();
            vertexHelper.AddUIVertexTriangleStream(list);
        }

        public void ModifyMesh(Mesh mesh)
        {
            if (!this.enabled)
            {
                return;
            }

            List<UIVertex> list = new List<UIVertex>();
            using (VertexHelper vertexHelper = new VertexHelper(mesh))
            {
                vertexHelper.GetUIVertexStream(list);
            }

            this.ModifyVertices(list);

            using (VertexHelper vertexHelper2 = new VertexHelper())
            {
                vertexHelper2.AddUIVertexTriangleStream(list);
                vertexHelper2.FillMesh(mesh);
            }
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        private void ModifyVertices(IList<UIVertex> vertices)
        {
            if (!this.enabled)
            {
                return;
            }

            RectTransform rectTransform = this.transform as RectTransform;
            if (rectTransform == null)
            {
                return;
            }

            Rect rect = rectTransform.rect;
            for (int i = 0; i < vertices.Count; ++i)
            {
                UIVertex v = vertices[i];
				
                // Modify positions
                v.position = new Vector3(
                    (this.Horizontal ? (v.position.x + (rect.center.x - v.position.x) * 2) : v.position.x),
                    (this.Vertical ?  (v.position.y + (rect.center.y - v.position.y) * 2) : v.position.y),
                    v.position.z
                );
				
                // Apply
                vertices[i] = v;
            }
        }
    }
}
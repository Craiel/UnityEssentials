namespace Craiel.UnityEssentialsUI.Runtime
{
    using UnityEngine;

    [AddComponentMenu(EssentialCoreUI.ComponentMenuFolder + "/Canvas Helper")]
    [RequireComponent(typeof(Canvas))]
    public class UICanvasHelper : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Canvas canvas;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField] 
        public Camera Camera;

        [SerializeField]
        public bool AnchorToCamera;
        
        [SerializeField]
        [Range(0f, 1f)]
        public float AnchorVertical = 0.5f;
        
        [SerializeField]
        [Range(0f, 1f)]
        public float AnchorHorizontal = 0.5f;

        [SerializeField]
        public bool ResizeToCameraAspect;

        [SerializeField]
        public bool ScaleByCamera;
        
        public void Awake()
        {
            this.rectTransform = this.transform as RectTransform;
            this.canvas = this.GetComponent<Canvas>();

            if (this.canvas == null)
            {
                EssentialCoreUI.Logger.Error("UICanvasHelper on object without canvas: {0}", this.gameObject);
            }
        }

        public void Update()
        {
            if (this.AnchorToCamera)
            {
                this.DoAnchorToCamera();
            }

            if (this.ResizeToCameraAspect)
            {
                this.DoResizeToCameraAspect();
            }

            if (this.ScaleByCamera)
            {
                this.DoScaleByCamera();
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoAnchorToCamera()
        {
            if (this.Camera == null)
            {
                return;
            }
            
            Vector3 newPosition = this.Camera.ViewportToWorldPoint(new Vector3(this.AnchorHorizontal, this.AnchorVertical, this.Camera.farClipPlane));
            newPosition.z = this.rectTransform.position.z;
            this.rectTransform.position = newPosition;
        }

        private void DoResizeToCameraAspect()
        {
            if (this.Camera == null)
            {
                return;
            }
            
            this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.Camera.aspect * this.rectTransform.rect.size.y);
        }

        private void DoScaleByCamera()
        {
            if (this.Camera == null || this.canvas == null)
            {
                return;
            }
            
            float cameraHeight;
            float distanceToMain = Vector3.Distance(this.Camera.transform.position, this.transform.position);

            if (this.Camera.orthographic)
            {
                cameraHeight = this.Camera.orthographicSize * 2.0f;
            }
            else
            {
                cameraHeight = 2.0f * distanceToMain * Mathf.Tan((this.Camera.fieldOfView * 0.5f) * Mathf.Deg2Rad);
            }

            float scaleFactor = Screen.height / this.rectTransform.rect.height;
            float scale = (cameraHeight / Screen.height) * scaleFactor;

            this.transform.localScale = new Vector3(scale, scale, 1.0f);
        }
    }
}
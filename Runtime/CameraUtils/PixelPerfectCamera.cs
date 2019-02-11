namespace Craiel.UnityEssentials.Runtime.CameraUtils
{
    using System;
    using UnityEngine;
    using Utils;

    [DisallowMultipleComponent]
    [AddComponentMenu("Rendering/Pixel Perfect Camera")]
    [RequireComponent(typeof(Camera))]
    public class PixelPerfectCamera : MonoBehaviour
    {
        private Camera targetCamera;
        private PixelPerfectCameraInternal internalCamera;

        private bool isInitialized;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int PixelRatio
        {
            get
            {
                if (!this.isInitialized)
                {
                    return 0;
                }
                
                return this.internalCamera.Zoom;
            }
        }

        [SerializeField] 
        public int AssetsPPU = 100;

        [SerializeField] 
        public int RefResolutionX = 320;

        [SerializeField] 
        public int RefResolutionY = 180;

        [SerializeField] 
        public bool UpscaleRT;

        [SerializeField] 
        public bool PixelSnapping;

        [SerializeField] 
        public bool CropFrameX;

        [SerializeField] 
        public bool CropFrameY;

        [SerializeField] 
        public bool StretchFill;
        
        public Vector3 RoundToPixel(Vector3 position)
        {
            float unitsPerPixel = this.internalCamera.UnitsPerPixel;
            if (Math.Abs(unitsPerPixel) < EssentialMathUtils.Epsilon)
            {
                return position;
            }

            Vector3 result;
            result.x = Mathf.Round(position.x / unitsPerPixel) * unitsPerPixel;
            result.y = Mathf.Round(position.y / unitsPerPixel) * unitsPerPixel;
            result.z = Mathf.Round(position.z / unitsPerPixel) * unitsPerPixel;

            return result;
        }

        public void Awake()
        {
            this.Initialize();
        }

        public void LateUpdate()
        {
            this.internalCamera.CalculateCameraProperties(Screen.width, Screen.height);

            // To be effective immediately this frame, forceIntoRenderTexture should be set before any camera rendering callback.
            // An exception of this is when the editor is paused, where we call LateUpdate() manually in OnPreCall().
            // In this special case, you'll see one frame of glitch when toggling renderUpscaling on and off.
            this.targetCamera.forceIntoRenderTexture =
                this.internalCamera.HasPostProcessLayer || this.internalCamera.UseOffscreenRT;
        }

        public void OnPreCull()
        {
#if UNITY_EDITOR
            // LateUpdate() is not called while the editor is paused, but OnPreCull() is.
            // So call LateUpdate() manually here.
            if (UnityEditor.EditorApplication.isPaused)
            {
                this.LateUpdate();
            }
#endif

            this.PixelSnap();

            if (this.internalCamera.PixelRect != Rect.zero)
            {
                this.targetCamera.pixelRect = this.internalCamera.PixelRect;
            }
            else
            {
                this.targetCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            }

            this.targetCamera.orthographicSize = this.internalCamera.OrthoSize;
        }

        public void OnPreRender()
        {
            // Clear the screen to black so that we can see black bars.
            // Need to do it before anything is drawn if we're rendering directly to the screen.
            if (this.internalCamera.CropFrameXOrY && !this.targetCamera.forceIntoRenderTexture && !this.targetCamera.allowMSAA)
            {
                GL.Clear(false, true, Color.black);
            }

            // Experimental.U2D.PixelPerfectRendering.pixelSnapSpacing = this.internalCamera.unitsPerPixel;
        }

        public void OnPostRender()
        {
            // Experimental.U2D.PixelPerfectRendering.pixelSnapSpacing = 0.0f;

            // Clear the screen to black so that we can see black bars.
            // If a temporary offscreen RT is used, we do the clear after we're done with that RT to avoid an unnecessary RT switch. 
            if (this.targetCamera.activeTexture != null)
            {
                Graphics.SetRenderTarget(null as RenderTexture);
                GL.Viewport(new Rect(0.0f, 0.0f, Screen.width, Screen.height));
                GL.Clear(false, true, Color.black);
            }

            if (!this.internalCamera.UseOffscreenRT)
            {
                return;
            }

            RenderTexture activeRT = this.targetCamera.activeTexture;
            if (activeRT != null)
            {
                activeRT.filterMode = this.internalCamera.UseStretchFill ? FilterMode.Bilinear : FilterMode.Point;
            }

            this.targetCamera.pixelRect =
                this.internalCamera.CalculatePostRenderPixelRect(this.targetCamera.aspect, Screen.width, Screen.height);
        }

#if UNITY_EDITOR
        public void OnEnable()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorApplication.playModeStateChanged += this.OnPlayModeChanged;
            }
        }
#endif

        public void OnDisable()
        {
            this.targetCamera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            this.targetCamera.orthographicSize = this.internalCamera.OriginalOrthoSize;
            this.targetCamera.forceIntoRenderTexture = this.internalCamera.HasPostProcessLayer;
            this.targetCamera.ResetAspect();
            this.targetCamera.ResetWorldToCameraMatrix();

            this.isInitialized = false;

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                UnityEditor.EditorApplication.playModeStateChanged -= this.OnPlayModeChanged;
            }
#endif
        }

        // Show on-screen warning about invalid render resolutions.
        public void OnGUI()
        {
            if (!Debug.isDebugBuild && !Application.isEditor)
            {
                return;
            }

#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying && !this.runInEditMode)
            {
                return;
            }
#endif

            Color oldColor = GUI.color;
            GUI.color = Color.red;

            Vector2Int renderResolution = Vector2Int.zero;
            renderResolution.x = this.internalCamera.UseOffscreenRT
                ? this.internalCamera.OffscreenRTWidth
                : this.targetCamera.pixelWidth;
            renderResolution.y = this.internalCamera.UseOffscreenRT
                ? this.internalCamera.OffscreenRTHeight
                : this.targetCamera.pixelHeight;

            if (renderResolution.x % 2 != 0 || renderResolution.y % 2 != 0)
            {
                string warning =
                    string.Format(
                        "Rendering at an odd-numbered resolution ({0} * {1}). Pixel Perfect Camera may not work properly in this situation.",
                        renderResolution.x, renderResolution.y);
                GUILayout.Box(warning);
            }

            if (Screen.width < this.RefResolutionX || Screen.height < this.RefResolutionY)
            {
                GUILayout.Box(
                    "Screen resolution is smaller than the reference resolution. Image may appear stretched or cropped.");
            }

            GUI.color = oldColor;
        }

#if UNITY_EDITOR
        public void OnPlayModeChanged(UnityEditor.PlayModeStateChange state)
        {
            // Stop running in edit mode when entering play mode.
            if (state == UnityEditor.PlayModeStateChange.ExitingEditMode)
            {
                this.runInEditMode = false;
                this.OnDisable();
            }
        }

        public void EnableEditMode()
        {
            this.Awake();
            this.runInEditMode = true;
        }

        public void DisableEditMode()
        {
            this.runInEditMode = false;
            this.OnDisable();
        }
#endif
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void PixelSnap()
        {
            Vector3 cameraPosition = this.targetCamera.transform.position;
            Vector3 roundedCameraPosition = this.RoundToPixel(cameraPosition);
            Vector3 offset = roundedCameraPosition - cameraPosition;
            offset.z = -offset.z;
            Matrix4x4 offsetMatrix = Matrix4x4.TRS(-offset, Quaternion.identity, new Vector3(1.0f, 1.0f, -1.0f));

            this.targetCamera.worldToCameraMatrix = offsetMatrix * this.targetCamera.transform.worldToLocalMatrix;
        }

        private void Initialize()
        {
            this.targetCamera = this.GetComponent<Camera>();
            this.internalCamera = new PixelPerfectCameraInternal(this)
            {
                OriginalOrthoSize = this.targetCamera.orthographicSize,
                HasPostProcessLayer = this.GetComponent("PostProcessLayer") != null
            }; 

            // query the component by name to avoid hard dependency

            if (this.targetCamera.targetTexture != null)
            {
                Debug.LogWarning("Render to texture is not supported by Pixel Perfect Camera.", this.targetCamera);
            }

            this.isInitialized = true;
        }
    }
}
using InputControl = Craiel.UnityEssentials.Input.InputControl;
using InputHandler = Craiel.UnityEssentials.Input.InputHandler;

namespace Craiel.UnityEssentials.Controllers
{
    using Contracts;
    using UnityEngine;

    public class MouseLookController : MonoBehaviour
    {
        private static readonly InputControl InputMouseLock = new InputControl("MLC_MouseLock");
        private static readonly InputControl InputMouseX = new InputControl("MLC_MouseX");
        private static readonly InputControl InputMouseY = new InputControl("MLC_MouseY");
        
        private Vector2 mouseAbsolute;
        private Vector2 smoothMouse;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public bool IsEnabled;

        [SerializeField]
        public bool CursorOwnership;

        public Vector2 ClampInDegrees = new Vector2(360, 180);
        public Vector2 Sensitivity = new Vector2(2, 2);
        public Vector2 Smoothing = new Vector2(3, 3);
        public Vector2 TargetDirection;
        public Vector2 TargetCharacterDirection;

        public static void ConfigureInput(IInputState targetState)
        {
            InputHandler.Instance.RegisterControl(InputMouseLock);
            InputHandler.Instance.RegisterControl(InputMouseX);
            InputHandler.Instance.RegisterControl(InputMouseY);

            targetState.AddAxis("Cancel").For(InputMouseLock);
            targetState.AddAxis("Mouse X").For(InputMouseX);
            targetState.AddAxis("Mouse Y").For(InputMouseY);
        }
        
        public void Start()
        {
            // Set target direction to the camera's initial orientation.
            this.TargetDirection = this.transform.localRotation.eulerAngles;
        }

        public void Update()
        {
            if (InputHandler.Instance.GetControl(InputMouseLock).IsDown)
            {
                this.IsEnabled = !this.IsEnabled;
            }
            
            if (!this.IsEnabled)
            {
                if (this.CursorOwnership)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }

                return;
            }

            if (this.CursorOwnership)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            float xMovement = InputHandler.Instance.GetControl(InputMouseX).Value;
            float yMovement = InputHandler.Instance.GetControl(InputMouseY).Value;

            var targetOrientation = Quaternion.Euler(this.TargetDirection);
            
            var mouseDelta = new Vector2(xMovement, yMovement);
            
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(this.Sensitivity.x * this.Smoothing.x, this.Sensitivity.y * this.Smoothing.y));
            
            this.smoothMouse.x = Mathf.Lerp(this.smoothMouse.x, mouseDelta.x, 1f / this.Smoothing.x);
            this.smoothMouse.y = Mathf.Lerp(this.smoothMouse.y, mouseDelta.y, 1f / this.Smoothing.y);
            
            this.mouseAbsolute += this.smoothMouse;

            if (this.ClampInDegrees.x < 360)
            {
                this.mouseAbsolute.x = Mathf.Clamp(this.mouseAbsolute.x, -this.ClampInDegrees.x * 0.5f,
                    this.ClampInDegrees.x * 0.5f);
            }

            if (this.ClampInDegrees.y < 360)
            {
                this.mouseAbsolute.y = Mathf.Clamp(this.mouseAbsolute.y, -this.ClampInDegrees.y * 0.5f,
                    this.ClampInDegrees.y * 0.5f);
            }

            var xRotation = Quaternion.AngleAxis(-this.mouseAbsolute.y, targetOrientation * Vector3.right);
            this.transform.localRotation = xRotation * targetOrientation;

            var yRotation = Quaternion.AngleAxis(this.mouseAbsolute.x, this.transform.InverseTransformDirection(Vector3.up));
            this.transform.localRotation *= yRotation;
        }
    }
}
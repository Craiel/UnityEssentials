namespace Assets.Scripts.Craiel.Essentials.Controllers
{
    using System;
    using Contracts;
    using GDX.AI.Sharp.Mathematics;
    using Input;
    using UnityEngine;

    public class FreeMovementController : MonoBehaviour
    {
        private static readonly InputControl InputAccellerate = new InputControl("FMC_Acellerate");
        private static readonly InputControl InputSlowDown = new InputControl("FMC_SlowDown");
        private static readonly InputControl InputRise = new InputControl("FMC_Rise");
        private static readonly InputControl InputMoveX = new InputControl("FMC_MoveX");
        private static readonly InputControl InputMoveZ = new InputControl("FMC_MoveZ");
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public float Speed = 0.5f;

        [SerializeField]
        public float AccelerationAmount = 3f;

        [SerializeField]
        public float AccelerationRatio = 1f;

        [SerializeField]
        public float SlowDownRatio = 0.5f;

        public static void ConfigureInput(IInputState targetState)
        {
            InputHandler.Instance.RegisterControl(InputAccellerate);
            InputHandler.Instance.RegisterControl(InputSlowDown);
            InputHandler.Instance.RegisterControl(InputRise);
            InputHandler.Instance.RegisterControl(InputMoveX);
            InputHandler.Instance.RegisterControl(InputMoveZ);

            targetState.AddAxis("Shift").For(InputAccellerate);
            targetState.AddAxis("Ctrl").For(InputSlowDown);
            targetState.AddAxis("Jump").For(InputRise);
            targetState.AddAxis("Horizontal").For(InputMoveZ);
            targetState.AddAxis("Vertical").For(InputMoveX);
        }
        
        public void Update()
        {
            InputControlState sprintControl = InputHandler.Instance.GetControl(InputAccellerate);
            if (sprintControl.IsDown)
            {
                this.Speed *= this.AccelerationRatio;
            } else if (sprintControl.IsUp)
            {
                this.Speed /= this.AccelerationRatio;
            }

            InputControlState ctrlControl = InputHandler.Instance.GetControl(InputSlowDown);
            if (ctrlControl.IsDown)
            {
                this.Speed *= this.SlowDownRatio;
            } else if (ctrlControl.IsUp)
            {
                this.Speed /= this.SlowDownRatio;
            }

            float zMovement = InputHandler.Instance.GetControl(InputMoveX).Value;
            if (Math.Abs(zMovement) > MathUtils.Epsilon)
            {
                this.transform.position += this.transform.forward * this.Speed * zMovement;
            }

            float xMovement = InputHandler.Instance.GetControl(InputMoveZ).Value;
            if (Math.Abs(xMovement) > MathUtils.Epsilon)
            {
                this.transform.position += this.transform.right * this.Speed * xMovement;
            }

            InputControlState jumpControl = InputHandler.Instance.GetControl(InputRise);
            if (jumpControl.IsHeld)
            {
                transform.Translate(Vector3.up * this.Speed * 0.5f);
            }
        }

        public void OnDrawGizmos()
        {
            GizmoUtils.DrawLine(this.transform.position, this.transform.position + (this.transform.forward * 2), Color.red);
        }
    }
}
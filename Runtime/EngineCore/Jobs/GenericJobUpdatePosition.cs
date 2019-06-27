namespace Craiel.UnityEssentials.Runtime.EngineCore.Jobs
{
    using Unity.Collections;
    using UnityEngine;
    using UnityEngine.Jobs;

    public struct GenericJobUpdatePosition : IJobParallelForTransform
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [ReadOnly]
        public NativeArray<Vector3> VelocityData;

        public float DeltaTime;

        public void Execute(int i, TransformAccess transform)
        {
            transform.position += this.VelocityData[i] * this.DeltaTime;
        }
    }
}
namespace Craiel.UnityEssentials.Runtime.EngineCore.Jobs
{
    using Unity.Collections;
    using Unity.Jobs;
    using UnityEngine;

    public struct GenericJobAccelerate : IJobParallelFor
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public NativeArray<Vector3> VelocityData;

        public Vector3 Acceleration;

        public float DeltaTime;

        public void Execute(int i)
        {
            this.VelocityData[i] += this.Acceleration * this.DeltaTime;
        }
    }
}
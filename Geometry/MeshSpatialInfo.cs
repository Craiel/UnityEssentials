namespace Craiel.UnityEssentials.Geometry
{
    internal class MeshSpatialInfo
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MeshSpatialInfo(uint? vertex = null, uint? normal = null)
        {
            this.Vertex = vertex;
            this.Normal = normal;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public uint? Vertex { get; set; }
        public ushort VertexUseCounter { get; set; }

        public uint? Normal { get; set; }
        public ushort NormalUseCounter { get; set; }
    }
}

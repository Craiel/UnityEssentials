namespace Craiel.UnityEssentials.Runtime.Geometry
{
    using System.Collections.Generic;
    using NLog;
    using Spatial;
    using UnityEngine;
    using Utils;

    public class DynamicMesh : Geometry.Mesh
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Octree<MeshSpatialInfo> mergeTree;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DynamicMesh(float initialSize = 1f)
            : this(Vector3.zero, initialSize)
        {
        }

        public DynamicMesh(Vector3 initialPosition, float initialSize = 1f)
        {
            this.mergeTree = new Octree<MeshSpatialInfo>(initialSize, initialPosition, 1f);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Join(IList<Vector3> vertices, IList<Vector3> normals, IDictionary<uint, uint[]> normalMapping, IList<Triangle3Indexed> triangles, Vector3 offset)
        {
            IList<Vector3> cleanVertices;
            IList<Triangle3Indexed> cleanTriangles;
            MeshUtils.CleanOrphanVertices(vertices, triangles, out cleanVertices, out cleanTriangles);

            this.DoJoin(cleanVertices, normals, normalMapping, cleanTriangles, offset);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoJoin(IList<Vector3> vertices, IList<Vector3> normals, IDictionary<uint, uint[]> normalMapping, IList<Triangle3Indexed> triangles, Vector3 offset)
        {
            bool[] vertexInvalidMap = new bool[vertices.Count];
            uint[] indexMap = new uint[vertices.Count];
            uint[] normalMap = new uint[normals.Count];
            
            Logger.Info("- {0} vertices", vertices.Count);
            bool check = this.Vertices.Count > 0;
            int skipped = 0;
            for (var i = 0; i < vertices.Count; i++)
            {
                Vector3 finalVertex = vertices[i] + offset;
                if (finalVertex.magnitude > EssentialMathUtils.MaxFloat)
                {
                    // Create a zero vertex, we will skip the triangles anyway
                    indexMap[i] = MeshUtils.AddNewVertex(this.Vertices, Vector3.zero, this.mergeTree);
                    vertexInvalidMap[i] = false;
                    Logger.Warn("- Vertex out of Safe Range: " + finalVertex);
                    skipped++;
                    continue;
                }

                if (!check)
                {
                    indexMap[i] = MeshUtils.AddNewVertex(this.Vertices, finalVertex, this.mergeTree);
                    continue;
                }
                
                uint index;
                if (MeshUtils.IndexOfVertex(this.mergeTree, finalVertex, out index))
                {
                    indexMap[i] = index;
                    skipped++;
                    continue;
                }

                indexMap[i] = MeshUtils.AddNewVertex(this.Vertices, finalVertex, this.mergeTree);
            }

            if (skipped > 0)
            {
                Logger.Info("  {0} duplicates", skipped);
            }

            Logger.Info("- {0} normals", normals.Count);
            bool checkNormals = this.Normals.Count > 0;
            skipped = 0;
            for (var i = 0; i < normals.Count; i++)
            {
                var normal = normals[i];

                if (!checkNormals)
                {
                    normalMap[i] = MeshUtils.AddNewNormal(this.Normals, normal, this.mergeTree);
                    continue;
                }

                uint index;
                if (MeshUtils.IndexOfNormal(this.mergeTree, normal, out index))
                {
                    normalMap[i] = index;
                    skipped++;
                    continue;
                }

                normalMap[i] = MeshUtils.AddNewNormal(this.Normals, normal, this.mergeTree);
            }

            if (skipped > 0)
            {
                Logger.Info("  {0} duplicates", skipped);
            }

            Logger.Info("- {0} triangles", triangles.Count);
            for (var i = 0; i < triangles.Count; i++)
            {
                Triangle3Indexed triangle = triangles[i];

                if (vertexInvalidMap[triangle.A] || vertexInvalidMap[triangle.B] || vertexInvalidMap[triangle.C])
                {
                    Logger.Warn("Triangle had invalid index mapping, possibly skipped vertices, skipping triangle");
                    continue;
                }

                if (check)
                {
                    // Re-index the triangle
                    this.Triangles.Add(new Triangle3Indexed(indexMap[triangle.A], indexMap[triangle.B], indexMap[triangle.C]));
                }
                else
                {
                    this.Triangles.Add(triangle);
                }

                // Remap the normals for this triangle and 
                if (normalMapping.Count > 0)
                {
                    uint[] map = new uint[3];
                    for (var n = 0; n < 3; n++)
                    {
                        uint normalIndex = normalMapping[(uint)i][n];
                        map[n] = normalMap[normalIndex];
                    }

                    this.NormalMapping.Add((uint)this.Triangles.Count - 1, map);
                }
            }
            
            this.RecalculateBounds();
        }
    }
}

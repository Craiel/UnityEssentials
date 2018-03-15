using CollectionExtensions = Craiel.UnityEssentials.Extensions.CollectionExtensions;
using Geometry_Mesh = Craiel.UnityEssentials.Geometry.Mesh;

namespace Craiel.UnityEssentials.Geometry
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class StaticMesh : Geometry_Mesh
    {
        public bool HasGeometry { get; private set; }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Clear()
        {
            base.Clear();
            this.HasGeometry = false;
        }

        public override void Join(IList<Vector3> vertices, IList<Vector3> normals, IDictionary<uint, uint[]> normalMapping, IList<Triangle3Indexed> triangles, Vector3 offset)
        {
            if (this.HasGeometry)
            {
                throw new InvalidOperationException("Join attempted on Static Mesh with geometry data, call clear first before setting new data!");
            }

            if (offset == Vector3.zero)
            {
                CollectionExtensions.AddRange(this.Vertices, vertices);
            }
            else
            {
                foreach (Vector3 vertex in vertices)
                {
                    this.Vertices.Add(vertex + offset);
                }
            }
            
            CollectionExtensions.AddRange(this.Normals, normals);
            CollectionExtensions.AddRange(this.NormalMapping, normalMapping);
            CollectionExtensions.AddRange(this.Triangles, triangles);

            this.RecalculateBounds();
        }
    }
}

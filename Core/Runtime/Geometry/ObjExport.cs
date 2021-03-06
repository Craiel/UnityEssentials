namespace Craiel.UnityEssentials.Runtime.Geometry
{
    using System;
    using System.IO;
    using UnityEngine;

    public static class ObjExport
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        // see https://en.wikipedia.org/wiki/Wavefront_.obj_file
        public static void Export(Mesh mesh, StreamWriter target)
        {
            EssentialsCore.Logger.Info("Saving to stream");

            int lineCount = 4;

            target.WriteLine(string.Format("g {0}", mesh.Name ?? "No Name"));

            EssentialsCore.Logger.Info("  - {0} vertices", mesh.Vertices.Count);
            foreach (Vector3 vertex in mesh.Vertices)
            {
                target.WriteLine(string.Format("v {0} {1} {2}", vertex.x, vertex.y, vertex.z));
                lineCount++;
            }

            target.WriteLine();

            EssentialsCore.Logger.Info("  - {0} normals", mesh.Normals.Count);
            foreach (Vector3 normal in mesh.Normals)
            {
                target.WriteLine(string.Format("vn {0} {1} {2}", normal.x, normal.y, normal.z));
                lineCount++;
            }

            target.WriteLine();

            EssentialsCore.Logger.Info("  - {0} triangles", mesh.Triangles.Count);
            for (var i = 0; i < mesh.Triangles.Count; i++)
            {
                var triangle = mesh.Triangles[i];

                // Currently we do not support texture coordinates
                if (mesh.Normals.Count > 0)
                {
                    target.WriteLine(string.Format("f {0}//{1} {2}//{3} {4}//{5}", triangle.A + 1,
                        mesh.NormalMapping[(uint) i][0] + 1, triangle.B + 1, mesh.NormalMapping[(uint) i][1] + 1,
                        triangle.C + 1, mesh.NormalMapping[(uint) i][2] + 1));
                }
                else
                {
                    target.WriteLine(string.Format("f {0} {1} {2}", triangle.A + 1, triangle.B + 1, triangle.C + 1));
                }

                lineCount++;
            }

            EssentialsCore.Logger.Info("  {0} lines", lineCount);
        }
    }
}

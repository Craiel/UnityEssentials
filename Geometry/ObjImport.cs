using Geometry_Mesh = Craiel.UnityEssentials.Geometry.Mesh;

namespace Craiel.UnityEssentials.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using NLog;
    using UnityEngine;

    public static class ObjImport
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private const string CommentIndicator = "#";

        private const char PolygonFaceSeparator = '/';

        private static readonly char[] LineSplitChars = { ' ' };

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Import(Geometry_Mesh target, Stream stream)
        {
            var context = new ParsingContext();

            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    context.CurrentLine = reader.ReadLine();
                    if (string.IsNullOrEmpty(context.CurrentLine))
                    {
                        continue;
                    }

                    // Prepare the line by trimming extra characters and comments
                    TrimComments(context);
                    context.CurrentLine = context.CurrentLine.Trim();
                    context.CurrentLineNumber++;

                    context.CurrentSegments = context.CurrentLine.Split(
                        LineSplitChars,
                        StringSplitOptions.RemoveEmptyEntries);
                    if (context.CurrentSegments.Length == 0)
                    {
                        continue;
                    }

                    ProcessLine(context);
                }
            }

            target.Name = context.Name;
            target.Join(context.TempVertices, context.Normals, context.NormalMapping, context.Triangles, Vector3.zero);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void TrimComments(ParsingContext context)
        {
            int index = context.CurrentLine.IndexOf(CommentIndicator, StringComparison.Ordinal);
            if (index >= 0)
            {
                context.CurrentLine = context.CurrentLine.Substring(0, index);
            }
        }

        private static void ProcessGeometricVertex(ParsingContext context)
        {
            if (context.CurrentSegments.Length < 4)
            {
                Logger.Error("Invalid Segment count for Geometric Vertex, Expected 4 but got {0}, line {1}", context.CurrentSegments.Length, context.CurrentLineNumber);
                return;
            }

            Vector3 vertex;
            if (!TryParseVector3(context.CurrentSegments[1], context.CurrentSegments[2], context.CurrentSegments[3], out vertex))
            {
                Logger.Error("Invalid vertex format in line {0}: {1}", context.CurrentLineNumber, context.CurrentLine);
                return;
            }

            context.TempVertices.Add(vertex);
        }

        private static void ProcessVertexNormal(ParsingContext context)
        {
            if (context.CurrentSegments.Length < 4)
            {
                Logger.Error("Invalid Segment count for Vertex Normal, Expected 4 but got {0}, line {1}", context.CurrentSegments.Length, context.CurrentLineNumber);
                return;
            }

            Vector3 normal;
            if (!TryParseVector3(context.CurrentSegments[1], context.CurrentSegments[2], context.CurrentSegments[3], out normal))
            {
                Logger.Error("Invalid vertex normal format in line {0}: {1}", context.CurrentLineNumber, context.CurrentLine);
                return;
            }

            context.TempNormals.Add(normal);
        }

        private static void ReadPolyFacePoint(string[] face, out int vertex, out int? texture, out int? normal)
        {
            texture = null;
            normal = null;

            switch (face.Length)
            {
                case 1:
                    {
                        vertex = int.Parse(face[0]);
                        return;
                    }

                case 2:
                    {
                        vertex = int.Parse(face[0]);
                        texture = int.Parse(face[1]);
                        return;
                    }

                case 3:
                    {
                        vertex = int.Parse(face[0]);

                        if (!string.IsNullOrEmpty(face[1]))
                        {
                            // Texture is optional when having normals
                            texture = int.Parse(face[1]);
                        }

                        normal = int.Parse(face[2]);
                        return;
                    }

                default:
                    {
                        throw new InvalidOperationException("Invalid PolyFace Data");
                    }
            }
        }

        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1501:StatementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
        private static void ProcessPolygonFace(ParsingContext context)
        {
            if (context.CurrentSegments.Length < 4)
            {
                Logger.Error("Invalid Segment count for Polygon Face, Expected  at least 4 but got {0}, line {1}", context.CurrentSegments.Length, context.CurrentLineNumber);
                return;
            }

            if (context.CurrentSegments.Length == 4)
            {
                int v0, v1, v2;
                int? n0, n1, n2;
                int? t0, t1, t2;

                string[] f0 = context.CurrentSegments[1].Split(PolygonFaceSeparator);
                string[] f1 = context.CurrentSegments[2].Split(PolygonFaceSeparator);
                string[] f2 = context.CurrentSegments[3].Split(PolygonFaceSeparator);

                ReadPolyFacePoint(f0, out v0, out n0, out t0);
                ReadPolyFacePoint(f1, out v1, out n1, out t1);
                ReadPolyFacePoint(f2, out v2, out n2, out t2);

                v0 -= 1;
                v1 -= 1;
                v2 -= 1;
                if (n0 != null) n0 -= 1;
                if (n1 != null) n1 -= 1;
                if (n2 != null) n2 -= 1;
                if (t0 != null) t0 -= 1;
                if (t1 != null) t1 -= 1;
                if (t2 != null) t2 -= 1;

                context.Triangles.Add(new Triangle3Indexed(v0, v1, v2));

                // Check if we have some normals but not all
                if (n0 != null || n1 != null || n2 != null)
                {
                    if (n0 == null || n1 == null || n2 == null)
                    {
                        throw new InvalidOperationException("Partial normals are not allowed");
                    }
                }

                if (context.TempNormals.Count > 0 && t0 != null && t1 != null && t2 != null)
                {
                    context.Normals.Add(context.TempNormals[n0.Value]);
                    context.Normals.Add(context.TempNormals[n1.Value]);
                    context.Normals.Add(context.TempNormals[n2.Value]);

                    context.NormalMapping.Add(
                            (uint)context.Triangles.Count - 1,
                            new[] { (uint)context.Normals.Count - 3, (uint)context.Normals.Count - 2, (uint)context.Normals.Count - 1 });
                }
            }
            else
            {
                int v0, n0;
                if (!int.TryParse(context.CurrentSegments[1].Split(PolygonFaceSeparator)[0], out v0)) { return; }
                if (!int.TryParse(context.CurrentSegments[1].Split(PolygonFaceSeparator)[2], out n0)) { return; }

                v0 -= 1;
                n0 -= 1;

                for (int i = 2; i < context.CurrentSegments.Length - 1; i++)
                {
                    int vi, vii;
                    int ni, nii;
                    if (!int.TryParse(context.CurrentSegments[i].Split(PolygonFaceSeparator)[0], out vi)) { continue; }
                    if (!int.TryParse(context.CurrentSegments[i + 1].Split(PolygonFaceSeparator)[0], out vii)) { continue; }
                    if (!int.TryParse(context.CurrentSegments[i].Split(PolygonFaceSeparator)[2], out ni)) { continue; }
                    if (!int.TryParse(context.CurrentSegments[i + 1].Split(PolygonFaceSeparator)[2], out nii)) { continue; }

                    vi -= 1;
                    vii -= 1;
                    ni -= 1;
                    nii -= 1;

                    context.Triangles.Add(new Triangle3Indexed(v0, vi, vii));

                    if (context.TempNormals.Count > 0)
                    {
                        context.Normals.Add(context.TempNormals[n0]);
                        context.Normals.Add(context.TempNormals[ni]);
                        context.Normals.Add(context.TempNormals[nii]);
                        context.NormalMapping.Add(
                            (uint)context.Triangles.Count - 1,
                            new[] { (uint)context.Normals.Count - 3, (uint)context.Normals.Count - 2, (uint)context.Normals.Count - 1 });
                    }
                }
            }
        }

        private static void ProcessLine(ParsingContext context)
        {
            switch (context.CurrentSegments[0])
            {
                case "g":
                    {
                        if (context.CurrentSegments.Length == 2)
                        {
                            context.Name = context.CurrentSegments[1];
                        }

                        break;
                    }

                case "v":
                    {
                        ProcessGeometricVertex(context);
                        break;
                    }

                case "vn":
                    {
                        ProcessVertexNormal(context);
                        break;
                    }

                case "f":
                    {
                        ProcessPolygonFace(context);
                        break;
                    }
            }
        }

        private static bool TryParseVector3(string x, string y, string z, out Vector3 v)
        {
            v = Vector3.zero;

            if (!float.TryParse(x, NumberStyles.Any, CultureInfo.InvariantCulture, out v.x))
            {
                return false;
            }

            if (!float.TryParse(y, NumberStyles.Any, CultureInfo.InvariantCulture, out v.y))
            {
                return false;
            }

            if (!float.TryParse(z, NumberStyles.Any, CultureInfo.InvariantCulture, out v.z))
            {
                return false;
            }

            return true;
        }

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        private class ParsingContext
        {
            public ParsingContext()
            {
                this.Triangles = new List<Triangle3Indexed>();
                this.Normals = new List<Vector3>();
                this.NormalMapping = new Dictionary<uint, uint[]>();

                this.TempNormals = new List<Vector3>();
                this.TempVertices = new List<Vector3>();
            }

            public string Name { get; set; }

            public IList<Triangle3Indexed> Triangles { get; private set; }
            public IList<Vector3> Normals { get; private set; }
            public IDictionary<uint, uint[]> NormalMapping { get; private set; }

            public IList<Vector3> TempVertices { get; private set; }
            public IList<Vector3> TempNormals { get; private set; }

            public int CurrentLineNumber { get; set; }
            public string CurrentLine { get; set; }
            public string[] CurrentSegments { get; set; }
        }
    }
}

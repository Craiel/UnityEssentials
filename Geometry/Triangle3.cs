namespace Craiel.UnityEssentials.Geometry
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using UnityEngine;

    /// <summary>
    /// A 3d triangle.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Triangle3 : IEquatable<Triangle3>
    {
        /// <summary>
        /// The first point.
        /// </summary>
        public Vector3 A;

        /// <summary>
        /// The second point.
        /// </summary>
        public Vector3 B;

        /// <summary>
        /// The third point.
        /// </summary>
        public Vector3 C;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle3"/> struct.
        /// </summary>
        /// <param name="a">The second point.</param>
        /// <param name="b">The first point.</param>
        /// <param name="c">The third point.</param>
        public Triangle3(Vector3 a, Vector3 b, Vector3 c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        /// <summary>
        /// Gets the directed line segment from <see cref="A"/> to <see cref="B"/>.
        /// </summary>
        public Vector3 AB
        {
            get { return this.B - this.A; }
        }

        /// <summary>
        /// Gets the directed line segment from <see cref="A"/> to <see cref="C"/>.
        /// </summary>
        public Vector3 AC
        {
            get { return this.C - this.A; }
        }

        /// <summary>
        /// Gets the directed line segment from <see cref="B"/> to <see cref="A"/>.
        /// </summary>
        public Vector3 BA
        {
            get { return this.A - this.B; }
        }

        /// <summary>
        /// Gets the directed line segment from <see cref="B"/> to <see cref="C"/>.
        /// </summary>
        public Vector3 BC
        {
            get { return this.C - this.B; }
        }

        /// <summary>
        /// Gets the directed line segment from <see cref="C"/> to <see cref="A"/>.
        /// </summary>
        public Vector3 CA
        {
            get { return this.A - this.C; }
        }

        /// <summary>
        /// Gets the directed line segment from <see cref="C"/> to <see cref="B"/>.
        /// </summary>
        public Vector3 CB
        {
            get { return this.B - this.C; }
        }

        /// <summary>
        /// Gets the area of the triangle.
        /// </summary>
        public float Area
        {
            get { return Vector3.Cross(this.AB, this.AC).magnitude * 0.5f; }
        }

        /// <summary>
        /// Gets the perimeter of the triangle.
        /// </summary>
        public float Perimeter
        {
            get { return this.AB.magnitude + this.AC.magnitude + this.BC.magnitude; }
        }

        /// <summary>
        /// Gets the centroid of the triangle.
        /// </summary>
        public Vector3 Centroid
        {
            get
            {
                const float OneThird = 1f / 3f;
                return this.A * OneThird + this.B * OneThird + this.C * OneThird;
            }
        }

        /// <summary>
        /// Gets the <see cref="Triangle3"/>'s surface normal. Assumes clockwise ordering of A, B, and C.
        /// </summary>
        public Vector3 Normal
        {
            get { return Vector3.Normalize(Vector3.Cross(this.AB, this.AC)); }
        }

        /// <summary>
        /// Compares two <see cref="Triangle3"/>'s for equality.
        /// </summary>
        /// <param name="left">The first triangle.</param>
        /// <param name="right">The second triangle.</param>
        /// <returns>A value indicating whether the two triangles are equal.</returns>
        public static bool operator ==(Triangle3 left, Triangle3 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="Triangle3"/>'s for inequality.
        /// </summary>
        /// <param name="left">The first triangle.</param>
        /// <param name="right">The second triangle.</param>
        /// <returns>A value indicating whether the two triangles are not equal.</returns>
        public static bool operator !=(Triangle3 left, Triangle3 right)
        {
            return !left.Equals(right);
        }
        
        /// <summary>
        /// Calculates the bounding box of a triangle.
        /// </summary>
        /// <param name="tri">A triangle.</param>
        /// <returns>The triangle's bounding box.</returns>
        public static Bounds GetBoundingBox(Triangle3 tri)
        {
            Bounds bounds;
            GetBoundingBox(ref tri, out bounds);
            return bounds;
        }

        /// <summary>
        /// Calculates the bounding box of a triangle.
        /// </summary>
        /// <param name="tri">A triangle.</param>
        /// <param name="bounds">The triangle's bounding box.</param>
        public static void GetBoundingBox(ref Triangle3 tri, out Bounds bounds)
        {
            GetBoundingBox(ref tri.A, ref tri.B, ref tri.C, out bounds);
        }

        /// <summary>
        /// Calculates the bounding box of a triangle from its vertices.
        /// </summary>
        /// <param name="a">The first vertex.</param>
        /// <param name="b">The second vertex.</param>
        /// <param name="c">The third vertex.</param>
        /// <param name="bounds">The bounding box between the points.</param>
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1501:StatementMustNotBeOnSingleLine", Justification = "Reviewed. Suppression is OK here.")]
        public static void GetBoundingBox(ref Vector3 a, ref Vector3 b, ref Vector3 c, out Bounds bounds)
        {
            Vector3 min = a;
            Vector3 max = a;

            if (b.x < min.x) { min.x = b.x;}
            if (b.y < min.y) { min.y = b.y;}
            if (b.z < min.z) { min.z = b.z;}
            if (c.x < min.x) { min.x = c.x;}
            if (c.y < min.y) { min.y = c.y;}
            if (c.z < min.z) { min.z = c.z;}

            if (b.x > max.x) { max.x = b.x;}
            if (b.y > max.y) { max.y = b.y;}
            if (b.z > max.z) { max.z = b.z;}
            if (c.x > max.x) { max.x = c.x;}
            if (c.y > max.y) { max.y = c.y;}
            if (c.z > max.z) { max.z = c.z;}

            bounds = new Bounds();
            bounds.SetMinMax(min, max);
        }

        /// <summary>
        /// Gets the area of the triangle projected onto the XZ-plane.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="c">The third point.</param>
        /// <param name="area">The calculated area.</param>
        public static void Area2D(ref Vector3 a, ref Vector3 b, ref Vector3 c, out float area)
        {
            float abx = b.x - a.x;
            float abz = b.z - a.z;
            float acx = c.x - a.x;
            float acz = c.z - a.z;
            area = acx * abz - abx * acz;
        }

        /// <summary>
        /// Gets the area of the triangle projected onto the XZ-plane.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="c">The third point.</param>
        /// <returns>The calculated area.</returns>
        public static float Area2D(Vector3 a, Vector3 b, Vector3 c)
        {
            float result;
            Area2D(ref a, ref b, ref c, out result);
            return result;
        }

        /// <summary>
        /// Checks for equality with another <see cref="Triangle3"/>.
        /// </summary>
        /// <param name="other">The other triangle.</param>
        /// <returns>A value indicating whether other is equivalent to the triangle.</returns>
        public bool Equals(Triangle3 other)
        {
            return
                this.A.Equals(other.A) &&
                this.B.Equals(other.B) &&
                this.C.Equals(other.C);
        }

        /// <summary>
        /// Checks for equality with another object.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>A value indicating whether other is equivalent to the triangle.</returns>
        public override bool Equals(object obj)
        {
            Triangle3? other = obj as Triangle3?;

            if (other.HasValue)
            {
                return this.Equals(other.Value);
            }

            return false;
        }

        /// <summary>
        /// Gets a unique hash code for the triangle.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.A.GetHashCode();
                hashCode = (hashCode * 397) ^ this.B.GetHashCode();
                hashCode = (hashCode * 397) ^ this.C.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Converts the triangle's data into a human-readable format.
        /// </summary>
        /// <returns>A string containing the triangle's data.</returns>
        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", this.A, this.B, this.C);
        }
    }
}

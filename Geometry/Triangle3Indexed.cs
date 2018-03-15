namespace Craiel.UnityEssentials.Geometry
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A 3d triangle with indexed vertices
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Triangle3Indexed : IEquatable<Triangle3Indexed>
    {
        /// <summary>
        /// The first point.
        /// </summary>
        public int A;

        /// <summary>
        /// The second point.
        /// </summary>
        public int B;

        /// <summary>
        /// The third point.
        /// </summary>
        public int C;

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle3Indexed"/> struct.
        /// </summary>
        /// <param name="a">The second point.</param>
        /// <param name="b">The first point.</param>
        /// <param name="c">The third point.</param>
        public Triangle3Indexed(int a, int b, int c)
        {
            this.A = a;
            this.B = b;
            this.C = c;
        }

        public Triangle3Indexed(uint a, uint b, uint c)
            : this((int)a, (int)b, (int)c)
        {
        }

        /// <summary>
        /// Compares two <see cref="Triangle3Indexed"/>'s for equality.
        /// </summary>
        /// <param name="left">The first triangle.</param>
        /// <param name="right">The second triangle.</param>
        /// <returns>A value indicating whether the two triangles are equal.</returns>
        public static bool operator ==(Triangle3Indexed left, Triangle3Indexed right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Compares two <see cref="Triangle3Indexed"/>'s for inequality.
        /// </summary>
        /// <param name="left">The first triangle.</param>
        /// <param name="right">The second triangle.</param>
        /// <returns>A value indicating whether the two triangles are not equal.</returns>
        public static bool operator !=(Triangle3Indexed left, Triangle3Indexed right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Checks for equality with another <see cref="Triangle3Indexed"/>.
        /// </summary>
        /// <param name="other">The other triangle.</param>
        /// <returns>A value indicating whether other is equivalent to the triangle.</returns>
        public bool Equals(Triangle3Indexed other)
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
            Triangle3Indexed? other = obj as Triangle3Indexed?;

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

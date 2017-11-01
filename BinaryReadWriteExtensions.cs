namespace Assets.Scripts.Craiel.Essentials
{
    using System.IO;
    using UnityEngine;

    public static class BinaryReadWriteExtensions
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Write(this BinaryWriter writer, Bounds bounds)
        {
            writer.Write(bounds.min);
            writer.Write(bounds.max);
        }

        public static void Write(this BinaryWriter writer, Vector3 vector)
        {
            writer.Write(vector.x);
            writer.Write(vector.y);
            writer.Write(vector.z);
        }

        public static void Write(this BinaryWriter writer, Vector2 vector)
        {
            writer.Write(vector.x);
            writer.Write(vector.y);
        }
        
        public static void Write(this BinaryWriter writer, Quaternion quaternion)
        {
            writer.Write(quaternion.x);
            writer.Write(quaternion.y);
            writer.Write(quaternion.z);
            writer.Write(quaternion.w);
        }
        
        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            return new Vector3(x, y, z);
        }

        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            return new Vector2(x, y);
        }

        public static Quaternion ReadQuaternion(this BinaryReader reader)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();
            float w = reader.ReadSingle();
            return new Quaternion(x, y, z, w);
        }

        public static Bounds ReadBoundingBox(this BinaryReader reader)
        {
            Vector3 min = reader.ReadVector3();
            Vector3 max = reader.ReadVector3();

            Bounds result = new Bounds();
            result.SetMinMax(min, max);
            return result;
        }
    }
}

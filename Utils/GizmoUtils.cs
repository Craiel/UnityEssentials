namespace Craiel.UnityEssentials.Utils
{
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;

    public static class GizmoUtils
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void DrawLabel(Vector3 position, string label, Color? color = null, int size = 16, FontStyle fontStyle = FontStyle.Normal)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label) { fontSize = size, fontStyle = fontStyle };
            if (color != null)
            {
                style.normal.textColor = color.Value;
            }

#if UNITY_EDITOR
            Handles.Label(position, label, style);
#endif
        }

        public static void DrawSphere(Vector3 center, float radius, Color? color = null, bool wireFrame = true)
        {
            Gizmos.color = color ?? EssentialsCore.DefaultGizmoColor;

            if (wireFrame)
            {
                Gizmos.DrawWireSphere(center, radius);
            }
            else
            {
                Gizmos.DrawSphere(center, radius);
            }
        }

        public static void DrawCubeMinMax(Vector3 min, Vector3 max, Color? color = null, bool wireFrame = true)
        {
            Bounds bounds = new Bounds();
            bounds.SetMinMax(min, max);
            DrawCube(bounds.center, bounds.size, color, wireFrame);
        }

        public static void DrawCube(Vector3 center, Vector3 size, Color? color = null, bool wireFrame = true)
        {
            Gizmos.color = color ?? EssentialsCore.DefaultGizmoColor;

            if (wireFrame)
            {
                Gizmos.DrawWireCube(center, size);
            }
            else
            {
                Gizmos.DrawCube(center, size);
            }
        }

        public static void DrawCube(Bounds bounds, Color? color = null, bool wireFrame = true)
        {
            DrawCube(bounds.center, bounds.size, color, wireFrame);
        }

        public static void DrawLine(Vector3 from, Vector3 to, Color? color = null)
        {
            Gizmos.color = color ?? EssentialsCore.DefaultGizmoColor;
            Gizmos.DrawLine(from, to);
        }

        public static void DrawCrossXZ(Vector3 center, float sizeX, float sizeZ, Color? color = null)
        {
            Gizmos.color = color ?? EssentialsCore.DefaultGizmoColor;

            Gizmos.DrawLine(
                new Vector3(center.x - sizeX, center.y, center.z),
                new Vector3(center.x + sizeX, center.y, center.z));
            Gizmos.DrawLine(
                new Vector3(center.x, center.y, center.z - sizeZ),
                new Vector3(center.x, center.y, center.z + sizeZ));
        }

        public static void DrawCrossXY(Vector3 center, float sizeX, float sizeY, Color? color = null)
        {
            Gizmos.color = color ?? EssentialsCore.DefaultGizmoColor;

            Gizmos.DrawLine(
                new Vector3(center.x - sizeX, center.y, center.z),
                new Vector3(center.x + sizeX, center.y, center.z));
            Gizmos.DrawLine(
                new Vector3(center.x, center.y - sizeY, center.z),
                new Vector3(center.x, center.y + sizeY, center.z));
        }

        public static void DrawCrossYZ(Vector3 center, float sizeY, float sizeZ, Color? color = null)
        {
            Gizmos.color = color ?? EssentialsCore.DefaultGizmoColor;

            Gizmos.DrawLine(
                new Vector3(center.x, center.y - sizeY, center.z),
                new Vector3(center.x, center.y + sizeY, center.z));
            Gizmos.DrawLine(
                new Vector3(center.x, center.y, center.z - sizeZ),
                new Vector3(center.x, center.y, center.z + sizeZ));
        }

        public static void BeginDrawWithMatrix(Matrix4x4 matrix)
        {
            Gizmos.matrix = matrix;
        }

        public static void EndDrawWithMatrix()
        {
            Gizmos.matrix = Matrix4x4.identity;
        }

        public static void BeginDrawRotated(Vector3 center, Quaternion rotation)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(center, rotation, Vector3.one);

            BeginDrawWithMatrix(rotationMatrix);
        }

        public static void EndDrawRotated()
        {
            EndDrawWithMatrix();
        }
    }
}

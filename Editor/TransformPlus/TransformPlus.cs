namespace Craiel.UnityEssentials.Editor.TransformPlus
{
    using Runtime.Utils;
    using UnityEngine;

    public static class TransformPlus
    {
        private static readonly Vector3 SnapPositionGrid = Vector3.one;
        private static readonly Vector3 SnapPositionOrigin = Vector3.zero;
        private static readonly Vector3 SnapRotationGrid = new Vector3(90, 90, 90);
        private static readonly Vector3 SnapRotationOrigin = Vector3.zero;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool IsReadOnly { get; private set; }
        
        public static Transform Current { get; private set; }
        
        public static TransformPlusSpace Space { get; set; }

        public static Vector3 Position
        {
            get
            {
                if (Current == null)
                {
                    return Vector3.zero;
                }
                
                switch (Space)
                {
                    case TransformPlusSpace.Local:
                    {
                        return Current.localPosition;
                    }

                    case TransformPlusSpace.World:
                    {
                        return Current.position;
                    }

                    default:
                    {
                        return Vector3.zero;
                    }
                }
            }
        }
        
        public static Vector3 Rotation
        {
            get
            {
                if (Current == null)
                {
                    return Vector3.zero;
                }
                
                switch (Space)
                {
                    case TransformPlusSpace.Local:
                    {
                        return Current.localEulerAngles;
                    }

                    case TransformPlusSpace.World:
                    {
                        return Current.eulerAngles;
                    }

                    default:
                    {
                        return Vector3.zero;
                    }
                }
            }
        }
        
        public static Vector3 Scale
        {
            get
            {
                if (Current == null)
                {
                    return Vector3.zero;
                }
                
                switch (Space)
                {
                    case TransformPlusSpace.Local:
                    {
                        return Current.localScale;
                    }

                    case TransformPlusSpace.World:
                    {
                        return Current.lossyScale;
                    }

                    default:
                    {
                        return Vector3.zero;
                    }
                }
            }
        }

        public static void UpdateFromScene()
        {
            if (Current == null)
            {
                return;
            }

            Current.hasChanged = false;
        }

        public static void SetCurrent(Transform target)
        {
            if (Current == target)
            {
                return;
            }

            Current = target;
            if (target == null)
            {
                return;
            }
        }

        public static void Reset()
        {
            SetCurrent(null);
            
            Space = TransformPlusSpace.Local;
        }

        public static void SetPosition(Vector3 newPosition)
        {
            if (Current == null)
            {
                return;
            }

            switch (Space)
            {
                case TransformPlusSpace.Local:
                {
                    Current.localPosition = newPosition;
                    Current.hasChanged = false;
                    break;
                }

                case TransformPlusSpace.World:
                {
                    Current.position = newPosition;
                    Current.hasChanged = false;
                    break;
                }
            }
        }
        
        public static void SetPositionSnapped(Vector3 newPosition)
        {
            Vector3 snapped = newPosition;
            snapped -= SnapPositionOrigin;
            snapped.x = SnapPositionGrid.x > 0 ? Mathf.Round(snapped.x / SnapPositionGrid.x) * SnapPositionGrid.x : snapped.x;
            snapped.y = SnapPositionGrid.y > 0 ? Mathf.Round(snapped.y / SnapPositionGrid.y) * SnapPositionGrid.y : snapped.y;
            snapped.z = SnapPositionGrid.z > 0 ? Mathf.Round(snapped.z / SnapPositionGrid.z) * SnapPositionGrid.z : snapped.z;
            snapped += SnapPositionOrigin;
            
            SetPosition(snapped);
        }

        public static void SetRotation(Vector3 eulerAngles)
        {
            if (Current == null)
            {
                return;
            }

            switch (Space)
            {
                case TransformPlusSpace.Local:
                {
                    Current.localEulerAngles = eulerAngles;
                    Current.hasChanged = false;
                    break;
                }

                case TransformPlusSpace.World:
                {
                    Current.eulerAngles = eulerAngles;
                    Current.hasChanged = false;
                    break;
                }
            }
        }

        public static void SetRotationSnapped(Vector3 eulerAngles)
        {
            Vector3 snapped = eulerAngles;
            snapped -= SnapRotationOrigin;
            snapped.x = SnapRotationGrid.x > 0 ? Mathf.Round(snapped.x / SnapRotationGrid.x) * SnapRotationGrid.x : snapped.x;
            snapped.y = SnapRotationGrid.y > 0 ? Mathf.Round(snapped.y / SnapRotationGrid.y) * SnapRotationGrid.y : snapped.y;
            snapped.z = SnapRotationGrid.z > 0 ? Mathf.Round(snapped.z / SnapRotationGrid.z) * SnapRotationGrid.z : snapped.z;
            snapped += SnapRotationOrigin;
            
            SetRotation(snapped);
        }
        
        public static void SetScale(Vector3 newScale)
        {
            if (Current == null)
            {
                return;
            }

            switch (Space)
            {
                case TransformPlusSpace.Local:
                {
                    Current.localScale = newScale;
                    Current.hasChanged = false;
                    break;
                }

                case TransformPlusSpace.World:
                {
                    // N/A
                    break;
                }
            }
        }
    }
}
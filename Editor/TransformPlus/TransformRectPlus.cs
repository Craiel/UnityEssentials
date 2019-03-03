namespace Craiel.UnityEssentials.Editor.TransformPlus
{
    using UnityEngine;

    public static class TransformRectPlus
    {
        private static Vector2 pivotCache;
        private static Vector2 anchorMinCache;
        private static Vector2 anchorMaxCache;
        private static Vector2 anchorPositionCache;
        private static Vector2 sizeCache;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool IsReadOnly { get; private set; }
        
        public static RectTransform Current { get; private set; }
        
        public static bool HasPivotCopy { get; private set; }
        
        public static bool HasAnchorMinCopy { get; private set; }
        
        public static bool HasAnchorMaxCopy { get; private set; }
        
        public static bool HasAnchorPositionCopy { get; private set; }
        
        public static bool HasSizeCopy { get; private set; }

        public static Vector2 Pivot
        {
            get
            {
                if (Current == null)
                {
                    return new Vector2(0.5f, 0.5f);
                }

                return Current.pivot;
            }
        }
        
        public static Vector2 AnchorMin
        {
            get
            {
                if (Current == null)
                {
                    return Vector2.zero;
                }
                
                return Current.anchorMin;
            }
        }
        
        public static Vector2 AnchorMax
        {
            get
            {
                if (Current == null)
                {
                    return Vector2.zero;
                }
                
                return Current.anchorMax;
            }
        }

        public static Vector2 AnchorPosition
        {
            get
            {
                if (Current == null)
                {
                    return Vector2.zero;
                }

                return Current.anchoredPosition;
            }
        }

        public static Vector2 Size
        {
            get
            {
                if (Current == null)
                {
                    return Vector2.zero;
                }

                return Current.sizeDelta;
            }
        }
        
        public static void SetCurrent(RectTransform target)
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
            
            pivotCache = Vector2.zero;
            sizeCache = Vector2.zero;
            anchorMaxCache = Vector2.zero;
            anchorMinCache = Vector2.zero;
            anchorPositionCache = Vector2.zero;

            HasPivotCopy = false;
            HasSizeCopy = false;
            HasAnchorMaxCopy = false;
            HasAnchorMinCopy = false;
            HasAnchorPositionCopy = false;
        }
        
        public static void UpdateFromScene()
        {
            if (Current == null)
            {
                return;
            }

            Current.hasChanged = false;
        }

        public static void CopyPivot()
        {
            pivotCache = Pivot;
            HasPivotCopy = true;
        }

        public static void CopyAnchorMin()
        {
            anchorMinCache = AnchorMin;
            HasAnchorMinCopy = true;
        }

        public static void CopyAnchorMax()
        {
            anchorMaxCache = AnchorMax;
            HasAnchorMaxCopy = true;
        }

        public static void CopyAnchorPosition()
        {
            anchorPositionCache = AnchorPosition;
            HasAnchorPositionCopy = true;
        }

        public static void CopySize()
        {
            sizeCache = Size;
            HasSizeCopy = true;
        }

        public static void PastePivot()
        {
            if (HasPivotCopy && Current != null)
            {
                Current.pivot = pivotCache;
            }
        }

        public static void PasteAnchorMin()
        {
            if (HasAnchorMinCopy && Current != null)
            {
                Current.anchorMin = anchorMinCache;
            }
        }
        
        public static void PasteAnchorMax()
        {
            if (HasAnchorMaxCopy && Current != null)
            {
                Current.anchorMax = anchorMaxCache;
            }
        }
        
        public static void PasteAnchorPosition()
        {
            if (HasAnchorPositionCopy && Current != null)
            {
                Current.anchoredPosition = anchorPositionCache;
            }
        }
        
        public static void PasteSize()
        {
            if (HasSizeCopy && Current != null)
            {
                Current.sizeDelta = sizeCache;
            }
        }

        public static void ResetPivot()
        {
            if (Current != null)
            {
                Current.pivot = new Vector2(0.5f, 0.5f);
            }
        }

        public static void ResetAnchorMin()
        {
            if (Current != null)
            {
                Current.anchorMin = Vector2.zero;
            }
        }

        public static void ResetAnchorMax()
        {
            if (Current != null)
            {
                Current.anchorMax = Vector2.one;
            }
        }

        public static void ResetAnchorPosition()
        {
            if (Current != null)
            {
                Current.anchoredPosition = Vector2.zero;
            }
        }

        public static void ResetSize()
        {
            if (Current != null)
            {
                Current.sizeDelta = Vector2.zero;
            }
        }
    }
}
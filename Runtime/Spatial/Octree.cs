namespace Craiel.UnityEssentials.Runtime.Spatial
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Utils;

    internal static class OctreeConstants
    {
        internal const int RecursionCheckDepth = 20;

        internal const int OctreeFloatPrecision = 4;
    }
    
    public class Octree<T>
        where T : class
    {
        private OctreeNode<T> root;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Octree(float initialSize, Vector3 initialPosition, float minNodeSize)
        {
            if (minNodeSize > initialSize)
            {
                EssentialsCore.Logger.Info("Minimum node size must be bigger or equal initial size: {0} > {1}", minNodeSize, initialSize);
                minNodeSize = initialSize;
            }

            this.InitialSize = initialSize;
            this.MinNodeSize = minNodeSize;
            this.InitialPosition = initialPosition;
            
            this.root = new OctreeNode<T>(this, initialSize, minNodeSize, initialPosition);

            this.AutoGrow = true;
            this.AutoShrink = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float InitialSize { get; private set; }

        public float MinNodeSize { get; private set; }

        public Vector3 InitialPosition { get; private set; }
        
        public int Count { get; private set; }

        public bool AutoGrow { get; set; }

        public bool AutoShrink { get; set; }

        public bool AutoMerge
        {
            get
            {
                return this.root.AutoMerge;
            }

            set
            {
                this.root.AutoMerge = value;
            }
        }

        public bool Add(T obj, Vector3 objPos)
        {
#if DEBUG
            if (Math.Abs(objPos.magnitude) > EssentialMathUtils.MaxFloat)
            {
                EssentialsCore.Logger.Error("Add Operation failed, coordinates are outside of safe range");
                return false;
            }
#endif

            Vector3 positionVector = EssentialMathUtils.WithMaxPrecision(objPos, (int) OctreeConstants.OctreeFloatPrecision);

            if (positionVector.x < this.root.Bounds.min.x || positionVector.y < this.root.Bounds.min.y ||
                positionVector.z < this.root.Bounds.min.z)
            {
                EssentialsCore.Logger.Error("Object position outside of octree lower bounds!");
                return false;
            }
            
            // Add object or expand the octree until it can be added
            int recursionCheck = 0;
            while (!this.root.Add(obj, positionVector))
            {
                if (this.AutoGrow)
                {
                    this.Grow();
                }
                else
                {
                    return false;
                }

                if (++recursionCheck > OctreeConstants.RecursionCheckDepth)
                {
                    EssentialsCore.Logger.Info("Add Operation exceeded recursion check");
                    return false;
                }
            }

            this.Count++;
            return true;
        }

        public bool Remove(T obj)
        {
            // See if we can shrink the octree down now that we've removed the item
            if (this.root.Remove(obj))
            {
                this.Count--;

                if (this.AutoShrink)
                {
                    this.Shrink();
                }

                return true;
            }

            return false;
        }

        public int CountObjects()
        {
            return this.root.CountObjects();
        }

        public bool GetAt(Vector3 position, out OctreeResult<T> result)
        {
            return this.root.GetAt(EssentialMathUtils.WithMaxPrecision(position, (int) OctreeConstants.OctreeFloatPrecision), out result);
        }

        public int GetNearby(Ray ray, float maxDistance, ref IList<OctreeResult<T>> results)
        {
            results.Clear();
            this.root.GetNearby(ref ray, ref maxDistance, ref results);
            return results.Count;
        }

        public void Grow()
        {
            float newSize = this.root.Size * 2f;

            var newRoot =
                new OctreeNode<T>(this, newSize, this.MinNodeSize, this.InitialPosition)
                {
                    AutoMerge = this.root.AutoMerge
                };

            newRoot.Split();
            int rootPos = OctreeNode<T>.GetChildIndex(newRoot.Center, this.root.Center);
            newRoot.SetChild(rootPos, this.root);
            this.root = newRoot;
        }

        public bool Shrink()
        {
            return this.root.Shrink(this.InitialSize, ref this.root);
        }

        public void Merge()
        {
            this.root.ForceMerge();
        }
    }
}

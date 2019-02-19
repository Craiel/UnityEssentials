namespace Craiel.UnityEssentials.Runtime.EngineCore.Construct
{
    using System;
    using UnityEngine;

    public class ConstructManager : MonoBehaviour
    {
        private bool isDone;
        private bool isError;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Awake()
        {
            this.isDone = false;
        }
        
        public void Update()
        {
            if (this.isDone || this.isError)
            {
                // We are in transition, nothing to do
                return;
            }

            if (this.CleanupConstruct(ConstructLogic.Settings))
            {
                return;
            }

            try
            {
                if (this.BuildTargetScene(ConstructLogic.Settings))
                {
                    return;
                }
            }
            catch (Exception)
            {
                this.isError = true;
                throw;
            }

            this.isDone = true;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool CleanupConstruct(ConstructSettings settings)
        {
            settings.Clear();
            return false;
        }

        private bool BuildTargetScene(ConstructSettings settings)
        {
            // Create the root node
            settings.Root = new GameObject("CONSTRUCT");
            DontDestroyOnLoad(settings.Root);
            
            
            return false;
        }
    }
}
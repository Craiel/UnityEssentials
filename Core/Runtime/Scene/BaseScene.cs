﻿namespace Craiel.UnityEssentials.Runtime.Scene
{
    using System;
    using System.Collections.Generic;
    using Enums;
    using Resource;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public delegate bool PartialSceneLoadingDelegate(SceneTransitionStep step);

    public abstract class BaseScene<TSceneEnum>
        where TSceneEnum: struct, IConvertible
    {
        private readonly IDictionary<string, AsyncOperation> additiveLoadLevelOperations;

        private readonly List<PartialSceneLoadingDelegate> pendingLoadingActions;

        private AsyncOperation loadLevelOperation;
        private LoadSceneMode loadMode = LoadSceneMode.Single;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected BaseScene(string name, string levelName)
        {
            this.Name = name;
            this.LevelName = levelName;
            this.AdditiveLevelNames = new List<string>();
            this.pendingLoadingActions = new List<PartialSceneLoadingDelegate>();
            this.UseAsyncLoading = true;

            this.additiveLoadLevelOperations = new Dictionary<string, AsyncOperation>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; private set; }
        
        public abstract TSceneEnum Type { get; }

        public string LevelName { get; private set; }

        public IList<string> AdditiveLevelNames { get; private set; } 

        public bool HadErrors { get; private set; }
        
        public bool ContinueLoad(SceneTransitionStep step)
        {
            if (this.HadErrors)
            {
                // On errors we keep the loading active indefinitly
                return true;
            }

            try
            {
                switch (step)
                {
                    case SceneTransitionStep.Initialize:
                        {
                            return this.TransitionInitialize(true);
                        }

                    case SceneTransitionStep.PreLoad:
                        {
                            return this.ScenePreLoad();
                        }

                    case SceneTransitionStep.LoadRegisterResources1:
                        {
                            return this.SceneRegisterResources1();
                        }

                    case SceneTransitionStep.LoadResources1:
                        {
                            return this.SceneLoadResources1();
                        }

                    case SceneTransitionStep.LoadRegisterResources2:
                        {
                            return this.SceneRegisterResources2();
                        }

                    case SceneTransitionStep.LoadResources2:
                        {
                            return this.SceneLoadResources2();
                        }

                    case SceneTransitionStep.LoadRegisterResources3:
                        {
                            return this.SceneRegisterResources3();
                        }

                    case SceneTransitionStep.LoadResources3:
                        {
                            return this.SceneLoadResources3();
                        }

                    case SceneTransitionStep.Load:
                        {
                            return this.SceneLoad();
                        }

                    case SceneTransitionStep.PostLoad:
                        {
                            return this.ScenePostLoad();
                        }

                    case SceneTransitionStep.Finalize:
                        {
                            return this.TransitionFinalize(true);
                        }

                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
            catch (Exception e)
            {
                this.HadErrors = true;
                EssentialsCore.Logger.Error("Error in Load of Scene {0}({1}): {2}", this.GetType(), step, e);
                return true;
            }
        }

        public bool ContinueDestroy(SceneTransitionStep step)
        {
            if (this.HadErrors)
            {
                // On errors we keep the loading active indefinitly
                return true;
            }

            try
            {
                switch (step)
                {
                    case SceneTransitionStep.Initialize:
                        {
                            return this.TransitionInitialize(false);
                        }

                    case SceneTransitionStep.PreDestroy:
                        {
                            return this.ScenePreDestroy();
                        }

                    case SceneTransitionStep.Destroy:
                        {
                            return this.SceneDestroy();
                        }

                    case SceneTransitionStep.PostDestroy:
                        {
                            return this.ScenePostDestroy();
                        }

                    case SceneTransitionStep.Finalize:
                        {
                            return this.TransitionFinalize(false);
                        }

                    default:
                        {
                            throw new InvalidOperationException();
                        }
                }
            }
            catch (Exception e)
            {
                this.HadErrors = true;
                EssentialsCore.Logger.Error("Error in Destroy of Scene {0}({1}): {2}", this.GetType(), step, e);
                return true;
            }
        }

        public virtual void SetData(object[] data)
        {
        }

        public void SetLoadMode(LoadSceneMode mode)
        {
            this.loadMode = mode;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected bool UseAsyncLoading { get; set; }

        protected virtual bool TransitionInitialize(bool isLoadTransition)
        {
            return false;
        }

        protected virtual bool TransitionFinalize(bool isLoadTransition)
        {
            return false;
        }

        protected virtual bool SceneRegisterResources1()
        {
            return false;
        }

        protected virtual bool SceneLoadResources1()
        {
            if (this.LoadBundlesAndResources())
            {
                return true;
            }

            return false;
        }

        protected virtual bool SceneRegisterResources2()
        {
            return false;
        }

        protected virtual bool SceneLoadResources2()
        {
            if (this.LoadBundlesAndResources())
            {
                return true;
            }

            return false;
        }

        protected virtual bool SceneRegisterResources3()
        {
            return false;
        }

        protected virtual bool SceneLoadResources3()
        {
            if (this.LoadBundlesAndResources())
            {
                return true;
            }

            return false;
        }

        protected virtual bool ScenePreLoad()
        {
            this.loadLevelOperation = null;

            return false;
        }

        protected virtual bool SceneLoad()
        {
            if (this.loadLevelOperation == null)
            {
                this.loadLevelOperation = SceneManager.LoadSceneAsync(this.LevelName, this.loadMode);
                return true;
            }

            if (!this.loadLevelOperation.isDone)
            {
                return true;
            }

            foreach (string additiveLevelName in this.AdditiveLevelNames)
            {
                AsyncOperation operation;
                if (this.additiveLoadLevelOperations.TryGetValue(additiveLevelName, out operation))
                {
                    if (!operation.isDone)
                    {
                        // At least one additive is still loading
                        return true;
                    }
                }
                else
                {
                    // Start the next additive async load
                    operation = SceneManager.LoadSceneAsync(additiveLevelName, LoadSceneMode.Additive);
                    this.additiveLoadLevelOperations.Add(additiveLevelName, operation);
                    return true;
                }
            }

            return false;
        }

        protected virtual bool ScenePostLoad()
        {
            return false;
        }

        protected virtual bool ScenePreDestroy()
        {
            return false;
        }

        protected virtual bool SceneDestroy()
        {
            return false;
        }

        protected virtual bool ScenePostDestroy()
        {
            return false;
        }

        protected void RegisterPendingLoadAction(Action action)
        {
            // Wrap the simple action into our delegate
            this.pendingLoadingActions.Add(
                x =>
                    {
                        action();
                        return false;
                    });
        }

        protected void RegisterPendingLoadAction(PartialSceneLoadingDelegate action)
        {
            this.pendingLoadingActions.Add(action);
        }

        protected bool ContinuePendingLoadingActions(SceneTransitionStep step)
        {
            if (this.pendingLoadingActions.Count > 0)
            {
                if (!this.pendingLoadingActions[0](step))
                {
                    this.pendingLoadingActions.RemoveAt(0);
                }

                return this.pendingLoadingActions.Count > 0;
            }

            return false;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool LoadBundlesAndResources()
        {
            if (this.UseAsyncLoading)
            {
                if (BundleProvider.Instance.ContinueLoad())
                {
                    return true;
                }

                if (ResourceProvider.Instance.ContinueLoad())
                {
                    return true;
                }

                return false;
            }

            BundleProvider.Instance.LoadImmediate();
            ResourceProvider.Instance.LoadImmediate();
            return false;
        }
    }
}

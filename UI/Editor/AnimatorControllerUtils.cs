namespace Craiel.UnityEssentialsUI.Editor
{
	using System.Collections.Generic;
	using Runtime;
	using UnityEditor;
	using UnityEditor.Animations;
	using UnityEngine;

	public static class AnimatorControllerUtils
    {
	    // -------------------------------------------------------------------
	    // Public
	    // -------------------------------------------------------------------
		public static AnimatorController Generate(SerializedProperty triggersProperty, string preferredName)
		{
			List<string> triggerList = new List<string>();
			
			SerializedProperty serializedProperty = triggersProperty.Copy();
			SerializedProperty endProperty = serializedProperty.GetEndProperty();
			
			while (serializedProperty.NextVisible(true) && !SerializedProperty.EqualContents(serializedProperty, endProperty))
			{
				triggerList.Add(!string.IsNullOrEmpty(serializedProperty.stringValue) ? serializedProperty.stringValue : serializedProperty.name);
			}
			
			return Generate(preferredName, triggerList.ToArray());
		}

        public static AnimatorController Generate(string preferredName, params string[] triggers)
        {
            return Generate(preferredName, false, triggers);
        }

        public static AnimatorController Generate(string preferredName, bool initialState, params string[] triggers)
		{
			if (string.IsNullOrEmpty(preferredName))
			{
				preferredName = EssentialCoreUI.DefaultAnimatorControllerName;
			}
			
			string saveControllerPath = GetSaveControllerPath(preferredName);

			if (string.IsNullOrEmpty(saveControllerPath))
			{
				return null;
			}
			
			AnimatorController animatorController = AnimatorController.CreateAnimatorControllerAtPath(saveControllerPath);

			if (initialState)
			{
				GenerateInitialState(animatorController);
			}

			if (triggers != null)
			{
				for (var i = 0; i < triggers.Length; i++)
				{
					GenerateTriggerTransition(triggers[i], animatorController);
				}
			}

			return animatorController;
		}
        
        public static void AddBool(string name, AnimatorController controller)
        {
	        foreach (AnimatorControllerParameter param in controller.parameters)
	        {
		        if (param.name.Equals(name))
		        {
			        return;
		        }
	        }

	        controller.AddParameter(name, AnimatorControllerParameterType.Bool);
        }
		
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
		private static string GetSaveControllerPath(string name)
		{
			string message = string.Format("Create a new Animator controller with name '{0}':", name);
			return EditorUtility.SaveFilePanelInProject(EssentialCoreUI.DefaultAnimatorControllerName, name, "controller", message);
		}
		
		private static AnimationClip GenerateTriggerTransition(string name, AnimatorController controller)
		{
			AnimationClip animationClip = AnimatorController.AllocateAnimatorClip(name);
			AssetDatabase.AddObjectToAsset(animationClip, controller);
			AnimatorState animatorState = controller.AddMotion(animationClip);
			controller.AddParameter(name, AnimatorControllerParameterType.Trigger);
			AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
			AnimatorStateTransition animatorStateTransition = stateMachine.AddAnyStateTransition(animatorState);
			animatorStateTransition.AddCondition(AnimatorConditionMode.If, 0f, name);
			return animationClip;
		}

        private static AnimationClip GenerateInitialState(AnimatorController controller)
        {
            AnimationClip animationClip = AnimatorController.AllocateAnimatorClip("Initial");
            AssetDatabase.AddObjectToAsset(animationClip, controller);
            controller.AddMotion(animationClip);
            return animationClip;
        }
    }
}
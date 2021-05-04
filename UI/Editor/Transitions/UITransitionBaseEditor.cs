namespace Craiel.UnityEssentialsUI.Editor.Transitions
{
	using System.Collections.Generic;
	using Runtime.Enums;
	using Runtime.Transitions;
	using TMPro;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEssentials.Editor;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(UITransitionBase))]
    public class UITransitionBaseEditor : EssentialEditorIM
    {
	    // -------------------------------------------------------------------
	    // Public
	    // -------------------------------------------------------------------
	    public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

			var typedTarget = this.target as UITransitionBase;
			
			EditorGUILayout.Space();
			this.DrawProperty<UITransitionBase>(x => x.TransitionMode);
			EditorGUI.indentLevel++;
			
			// Check if the transition requires a graphic
			switch (typedTarget.TransitionMode)
			{
				case UITransitionMode.ColorTint:
				case UITransitionMode.SpriteSwap:
				case UITransitionMode.TextColor:
				case UITransitionMode.TextColorTMP:
				{
					this.DrawGraphic(typedTarget);
					break;
				}

				case UITransitionMode.Animation:
				{
					this.DrawAnimation(typedTarget);
					break;
				}
			}

			if (this.HasToggle)
			{
				this.DrawToggle(typedTarget);
			}

			this.serializedObject.ApplyModifiedProperties();
		}
	    
	    // -------------------------------------------------------------------
	    // Protected
	    // -------------------------------------------------------------------
	    protected bool HasToggle { get; set; }
	    
	    protected bool DrawAllColors { get; set; }

	    // -------------------------------------------------------------------
	    // Private
	    // -------------------------------------------------------------------
	    private void DrawGraphic(UITransitionBase typedTarget)
	    {
		    this.DrawProperty<UITransitionBase>(x => x.TargetGraphic);

		    switch (typedTarget.TransitionMode)
		    {
			    case UITransitionMode.ColorTint:
			    {
				    if (typedTarget.TargetGraphic == null)
				    {
					    EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.", MessageType.Warning);
				    }
				    else
				    {
					    EditorGUI.BeginChangeCheck();
					    this.DrawProperty<UITransitionBase>(x => x.NormalColor);
					    if (EditorGUI.EndChangeCheck())
					    {
						    typedTarget.TargetGraphic.canvasRenderer.SetColor(typedTarget.NormalColor);
					    }

					    this.DrawProperty<UITransitionBase>(x => x.PressedColor);

					    if (this.DrawAllColors)
					    {
						    this.DrawProperty<UITransitionBase>(x => x.HighlightedColor);
						    this.DrawProperty<UITransitionBase>(x => x.SelectedColor);
					    }

					    this.DrawProperty<UITransitionBase>(x => x.ColorMultiplier);
					    this.DrawProperty<UITransitionBase>(x => x.Duration);
				    }
				    
				    break;
			    }

			    case UITransitionMode.TextColor:
			    {
				    if (typedTarget.TargetGraphic == null || typedTarget.TargetGraphic is Text == false)
				    {
					    EditorGUILayout.HelpBox("You must have a Text target in order to use a text color transition.", MessageType.Warning);
				    }
				    else
				    {
					    EditorGUI.BeginChangeCheck();
					    this.DrawProperty<UITransitionBase>(x => x.NormalColor);
					    if (EditorGUI.EndChangeCheck())
					    {
						    typedTarget.TargetGraphic.canvasRenderer.SetColor(typedTarget.NormalColor);
					    }

					    this.DrawProperty<UITransitionBase>(x => x.PressedColor);

					    if (this.DrawAllColors)
					    {
						    this.DrawProperty<UITransitionBase>(x => x.HighlightedColor);
						    this.DrawProperty<UITransitionBase>(x => x.SelectedColor);
					    }

					    this.DrawProperty<UITransitionBase>(x => x.Duration);
				    }
				    
				    break;
			    }

			    case UITransitionMode.TextColorTMP:
			    {
				    if (typedTarget.TargetGraphic == null || typedTarget.TargetGraphic is TextMeshProUGUI == false)
				    {
					    EditorGUILayout.HelpBox("You must have a Text target in order to use a text color transition.", MessageType.Warning);
				    }
				    else
				    {
					    EditorGUI.BeginChangeCheck();
					    this.DrawProperty<UITransitionBase>(x => x.NormalColor);
					    if (EditorGUI.EndChangeCheck())
					    {
						    typedTarget.TargetGraphic.canvasRenderer.SetColor(typedTarget.NormalColor);
					    }

					    this.DrawProperty<UITransitionBase>(x => x.PressedColor);

					    if (this.DrawAllColors)
					    {
						    this.DrawProperty<UITransitionBase>(x => x.HighlightedColor);
						    this.DrawProperty<UITransitionBase>(x => x.SelectedColor);
					    }

					    this.DrawProperty<UITransitionBase>(x => x.Duration);
				    }
				    
				    break;
			    }

			    case UITransitionMode.SpriteSwap:
			    {
				    if (typedTarget.TargetGraphic == null || typedTarget.TargetGraphic is Image == false)
				    {
					    EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.", MessageType.Warning);
				    }
				    else
				    {
					    this.DrawProperty<UITransitionBase>(x => x.PressedColor);
					    
					    if (this.DrawAllColors)
					    {
						    this.DrawProperty<UITransitionBase>(x => x.HighlightedColor);
						    this.DrawProperty<UITransitionBase>(x => x.SelectedColor);
					    }
				    }
				    
				    break;
			    }
		    }
	    }

	    private void DrawAnimation(UITransitionBase typedTarget)
	    {
		    this.DrawProperty<UITransitionBase>(x => x.TargetGameObject);

		    if (typedTarget.TargetGameObject == null)
		    {
			    EditorGUILayout.HelpBox("You must have a Game Object target in order to use a animation transition.", MessageType.Warning);
			    return;
		    }
		    
		    this.DrawProperty<UITransitionBase>(x => x.NormalColor);
		    this.DrawProperty<UITransitionBase>(x => x.HighlightedColor);
		    this.DrawProperty<UITransitionBase>(x => x.SelectedColor);
		    this.DrawProperty<UITransitionBase>(x => x.PressedColor);

		    Animator animator = typedTarget.TargetGameObject.GetComponent<Animator>();

		    if (animator == null || animator.runtimeAnimatorController == null)
		    {
			    Rect controlRect = EditorGUILayout.GetControlRect();
			    controlRect.xMin = (controlRect.xMin + EditorGUIUtility.labelWidth);

			    if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton))
			    {
				    // Generate the animator controller
				    UnityEditor.Animations.AnimatorController animatorController = this.GenerateAnimatorController(typedTarget);

				    if (animatorController != null)
				    {
					    if (animator == null)
					    {
						    animator = typedTarget.TargetGameObject.AddComponent<Animator>();
					    }

					    UnityEditor.Animations.AnimatorController.SetAnimatorController(animator, animatorController);
				    }
			    }
		    }
	    }
	    
        private void DrawToggle(UITransitionBase typedTarget)
        {
	        this.DrawProperty<UITransitionBase>(x => x.UseToggle);

	        if (typedTarget.UseToggle)
	        {
		        this.DrawProperty<UITransitionBase>(x => x.TargetToggle);

		        switch (typedTarget.TransitionMode)
		        {
			        case UITransitionMode.ColorTint:
			        case UITransitionMode.TextColor:
			        case UITransitionMode.TextColorTMP:
			        {
				        this.DrawProperty<UITransitionBase>(x => x.ActiveColor);
				        break;
			        }

			        case UITransitionMode.SpriteSwap:
			        {
				        this.DrawProperty<UITransitionBase>(x => x.ActiveSprite);
				        break;
			        }

			        case UITransitionMode.Animation:
			        {
				        this.DrawProperty<UITransitionBase>(x => x.ActiveBool);
				        break;
			        }
		        }
	        }
        }
		
		private UnityEditor.Animations.AnimatorController GenerateAnimatorController(UITransitionBase typedTarget)
		{
			return AnimatorControllerUtils.Generate(typedTarget.TargetGameObject.name,
				string.IsNullOrEmpty(typedTarget.NormalTrigger) ? "Normal" : typedTarget.NormalTrigger,
				string.IsNullOrEmpty(typedTarget.HighlightedTrigger) ? "Highlighted" : typedTarget.HighlightedTrigger,
				string.IsNullOrEmpty(typedTarget.SelectedTrigger) ? "Selected" : typedTarget.SelectedTrigger,
				string.IsNullOrEmpty(typedTarget.PressedTrigger) ? "Pressed" : typedTarget.PressedTrigger);
		}
    }
}
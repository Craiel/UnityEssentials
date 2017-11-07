namespace Assets.Scripts.Craiel.Essentials.Editor
{
	using System;
	using UnityEditor;
	using UnityEngine;

	public static class SerializedPropertyExtensions
	{
		public static void ResetValue(this SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.Integer:
				case SerializedPropertyType.LayerMask:
				case SerializedPropertyType.ArraySize:
				case SerializedPropertyType.Character:
				{
					property.intValue = 0;
					break;
				}

				case SerializedPropertyType.Boolean:
				{
					property.boolValue = false;
					break;
				}

				case SerializedPropertyType.Float:
				{
					property.floatValue = 0f;
					break;
				}

				case SerializedPropertyType.String:
				{
					property.stringValue = string.Empty;
					break;
				}

				case SerializedPropertyType.Color:
				{
					property.colorValue = Color.black;
					break;
				}

				case SerializedPropertyType.ObjectReference:
				{
					property.objectReferenceValue = null;
					break;
				}

				case SerializedPropertyType.Enum:
				{
					property.enumValueIndex = 0;
					break;
				}

				case SerializedPropertyType.Vector2:
				{
					property.vector2Value = default(Vector2);
					break;
				}

				case SerializedPropertyType.Vector3:
				{
					property.vector3Value = default(Vector3);
					break;
				}

				case SerializedPropertyType.Vector4:
				{
					property.vector4Value = default(Vector4);
					break;
				}

				case SerializedPropertyType.Rect:
				{
					property.rectValue = default(Rect);
					break;
				}

				case SerializedPropertyType.AnimationCurve:
				{
					property.animationCurveValue = AnimationCurve.Linear(0f, 0f, 1f, 1f);
					break;
				}

				case SerializedPropertyType.Bounds:
				{
					property.boundsValue = default(Bounds);
					break;
				}
					
				case SerializedPropertyType.Gradient:
					//!TODO: Amend when Unity add a public API for setting the gradient.
					break;
			}

			if (property.isArray)
			{
				property.arraySize = 0;
			}

			property.ResetChildPropertyValues();
		}

		private static void ResetChildPropertyValues(this SerializedProperty element)
		{
			if (!element.hasChildren)
			{
				return;
			}

			var childProperty = element.Copy();
			int elementPropertyDepth = element.depth;
			bool enterChildren = true;

			while (childProperty.Next(enterChildren) && childProperty.depth > elementPropertyDepth)
			{
				enterChildren = false;
				childProperty.ResetValue();
			}
		}

		public static void CopyPropertyValue(this SerializedProperty source, SerializedProperty target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			source = source.Copy();
			target = target.Copy();

			CopyPropertyValueSingular(target, source);

			if (source.hasChildren)
			{
				int elementPropertyDepth = source.depth;
				while (source.Next(true) && target.Next(true) && source.depth > elementPropertyDepth)
				{
					source.CopyPropertyValue(target);
				}
			}
		}

		private static void CopyPropertyValueSingular(this SerializedProperty source, SerializedProperty target)
		{
			switch (target.propertyType)
			{
				case SerializedPropertyType.Integer:
				case SerializedPropertyType.LayerMask:
				case SerializedPropertyType.ArraySize:
				case SerializedPropertyType.Character:
				{
					target.intValue = source.intValue;
					break;
				}

				case SerializedPropertyType.Boolean:
				{
					target.boolValue = source.boolValue;
					break;
				}

				case SerializedPropertyType.Float:
				{
					target.floatValue = source.floatValue;
					break;
				}

				case SerializedPropertyType.String:
				{
					target.stringValue = source.stringValue;
					break;
				}

				case SerializedPropertyType.Color:
				{
					target.colorValue = source.colorValue;
					break;
				}

				case SerializedPropertyType.ObjectReference:
				{
					target.objectReferenceValue = source.objectReferenceValue;
					break;
				}

				case SerializedPropertyType.Enum:
				{
					target.enumValueIndex = source.enumValueIndex;
					break;
				}

				case SerializedPropertyType.Vector2:
				{
					target.vector2Value = source.vector2Value;
					break;
				}

				case SerializedPropertyType.Vector3:
				{
					target.vector3Value = source.vector3Value;
					break;
				}

				case SerializedPropertyType.Vector4:
				{
					target.vector4Value = source.vector4Value;
					break;
				}

				case SerializedPropertyType.Rect:
				{
					target.rectValue = source.rectValue;
					break;
				}

				case SerializedPropertyType.AnimationCurve:
				{
					target.animationCurveValue = source.animationCurveValue;
					break;
				}

				case SerializedPropertyType.Bounds:
				{
					target.boundsValue = source.boundsValue;
					break;
				}
					
				case SerializedPropertyType.Gradient:
					//!TODO: Amend when Unity add a public API for setting the gradient.
					break;
			}
		}
	}
}

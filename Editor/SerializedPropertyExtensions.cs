namespace Craiel.UnityEssentials.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Runtime;
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

		public static T GetValue<T>(this SerializedProperty property)
		{
			return GetNestedObject<T>(property.propertyPath, GetSerializedPropertyRootComponent(property));
		}

		public static bool SetValue<T>(this SerializedProperty property, T value)
		{
			object obj = GetSerializedPropertyRootComponent(property);
			//Iterate to parent object of the value, necessary if it is a nested object
			string[] fieldStructure = property.propertyPath.Split('.');
			for (int i = 0; i < fieldStructure.Length - 1; i++)
			{
				obj = GetFieldOrPropertyValue<object>(fieldStructure[i], obj);
			}

			string fieldName = fieldStructure.Last();

			return SetFieldOrPropertyValue(fieldName, obj, value);
		}

		public static Component GetSerializedPropertyRootComponent(SerializedProperty property)
		{
			return (Component)property.serializedObject.targetObject;
		}

		public static T GetNestedObject<T>(string path, object obj, bool includeAllBases = false)
		{
			foreach (string part in path.Split('.'))
			{
				obj = GetFieldOrPropertyValue<object>(part, obj, includeAllBases);
			}
			return (T)obj;
		}

		public static T GetFieldOrPropertyValue<T>(string fieldName, object obj, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		{
			FieldInfo field = obj.GetType().GetField(fieldName, bindings);
			if (field != null) return (T)field.GetValue(obj);

			PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
			if (property != null) return (T)property.GetValue(obj, null);

			if (includeAllBases)
			{
 				foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
				{
					field = type.GetField(fieldName, bindings);
					if (field != null) return (T)field.GetValue(obj);

					property = type.GetProperty(fieldName, bindings);
					if (property != null) return (T)property.GetValue(obj, null);
				}
			}

			return default(T);
		}

		public static bool SetFieldOrPropertyValue(string fieldName, object obj, object value, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		{
			FieldInfo field = obj.GetType().GetField(fieldName, bindings);
			if (field != null)
			{
				field.SetValue(obj, value);
				return true;
			}

			PropertyInfo property = obj.GetType().GetProperty(fieldName, bindings);
			if (property != null)
			{
				property.SetValue(obj, value, null);
				return true;
			}

			if (includeAllBases)
			{
				foreach (Type type in GetBaseClassesAndInterfaces(obj.GetType()))
				{
					field = type.GetField(fieldName, bindings);
					if (field != null)
					{
						field.SetValue(obj, value);
						return true;
					}

					property = type.GetProperty(fieldName, bindings);
					if (property != null)
					{
						property.SetValue(obj, value, null);
						return true;
					}
				}
			}

			return false;
		}

		public static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type, bool includeSelf = false)
		{
			List<Type> allTypes = new List<Type>();

			if (includeSelf) allTypes.Add(type);

			if (type.BaseType == TypeCache<object>.Value)
			{
				allTypes.AddRange(type.GetInterfaces());
			}
			else
			{
				allTypes.AddRange(
					Enumerable
						.Repeat(type.BaseType, 1)
						.Concat(type.GetInterfaces())
						.Concat(type.BaseType.GetBaseClassesAndInterfaces())
						.Distinct());
			}

			return allTypes;
		}
	}
}

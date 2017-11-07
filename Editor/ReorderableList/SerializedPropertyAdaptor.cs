namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList
{
	using System;
	using Assets.Scripts.Craiel.Essentials.Editor.ReorderableList.Contracts;
	using UnityEditor;
	using UnityEngine;
	using MathUtils = GDX.AI.Sharp.Mathematics.MathUtils;

	public class SerializedPropertyAdaptor : IReorderableListAdaptor
	{
		// -------------------------------------------------------------------
		// Constructor
		// -------------------------------------------------------------------
		public SerializedPropertyAdaptor(SerializedProperty arrayProperty, float fixedItemHeight)
		{
			if (arrayProperty == null)
			{
				throw new ArgumentNullException("arrayProperty");
			}
			
			if (!arrayProperty.isArray)
			{
				throw new InvalidOperationException("Specified serialized propery is not an array.");
			}

			this.ArrayProperty = arrayProperty;
			this.FixedItemHeight = fixedItemHeight;
		}

		public SerializedPropertyAdaptor(SerializedProperty arrayProperty) : this(arrayProperty, 0f)
		{
		}

		// -------------------------------------------------------------------
		// Public
		// -------------------------------------------------------------------
		public float FixedItemHeight { get; private set; }

		public SerializedProperty this[int index]
		{
			get { return this.ArrayProperty.GetArrayElementAtIndex(index); }
		}

		public SerializedProperty ArrayProperty { get; private set; }

		public int Count
		{
			get { return this.ArrayProperty.arraySize; }
		}

		public virtual bool CanDrag(int index)
		{
			return true;
		}

		public virtual bool CanRemove(int index)
		{
			return true;
		}

		public void Add()
		{
			int newIndex = this.ArrayProperty.arraySize;
			++this.ArrayProperty.arraySize;
			this.ArrayProperty.GetArrayElementAtIndex(newIndex).ResetValue();
		}

		public void Insert(int index)
		{
			this.ArrayProperty.InsertArrayElementAtIndex(index);
			this.ArrayProperty.GetArrayElementAtIndex(index).ResetValue();
		}

		public void Duplicate(int index)
		{
			this.ArrayProperty.InsertArrayElementAtIndex(index);
		}

		public void Remove(int index)
		{
			// Unity doesn't remove element when it contains an object reference.
			var elementProperty = this.ArrayProperty.GetArrayElementAtIndex(index);
			if (elementProperty.propertyType == SerializedPropertyType.ObjectReference)
			{
				elementProperty.objectReferenceValue = null;
			}

			this.ArrayProperty.DeleteArrayElementAtIndex(index);
		}

		public void Move(int sourceIndex, int destIndex)
		{
			if (destIndex > sourceIndex)
			{
				--destIndex;
			}
			
			this.ArrayProperty.MoveArrayElement(sourceIndex, destIndex);
		}

		public void Clear()
		{
			this.ArrayProperty.ClearArray();
		}

		public virtual void BeginGUI()
		{
		}

		public virtual void EndGUI()
		{
		}

		public virtual void DrawItemBackground(Rect position, int index)
		{
		}

		public virtual void DrawItem(Rect position, int index)
		{
			EditorGUI.PropertyField(position, this[index], GUIContent.none, false);
		}

		public virtual float GetItemHeight(int index)
		{
			return Math.Abs(this.FixedItemHeight) > MathUtils.Epsilon
					? this.FixedItemHeight
					: EditorGUI.GetPropertyHeight(this[index], GUIContent.none, false);
		}
	}
}

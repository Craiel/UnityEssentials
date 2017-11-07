namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList
{
	using System.Collections.Generic;
	using Assets.Scripts.Craiel.Essentials.Editor.ReorderableList.Contracts;
	using UnityEditor;
	using UnityEngine;

	public static class ReorderableListGUI
	{
		public const float DefaultItemHeight = 18;
		
		private static readonly GUIContent TempGuiContent = new GUIContent();

		// -------------------------------------------------------------------
		// Constructor
		// -------------------------------------------------------------------
		static ReorderableListGUI()
		{
			DefaultListControl = new ReorderableListControl
			{
				// Duplicate default styles to prevent user scripts from interferring with
				// the default list control instance.
				ContainerStyle = new GUIStyle(ReorderableListStyles.Container),
				FooterButtonStyle = new GUIStyle(ReorderableListStyles.FooterButton),
				ItemButtonStyle = new GUIStyle(ReorderableListStyles.ItemButton)
			};

			IndexOfChangedItem = -1;
		}

		// -------------------------------------------------------------------
		// Public
		// -------------------------------------------------------------------
		public static int IndexOfChangedItem { get; internal set; }

		public static int CurrentListControlId
		{
			get { return ReorderableListControl.CurrentListControlId; }
		}

		public static Rect CurrentListPosition
		{
			get { return ReorderableListControl.CurrentListPosition; }
		}

		public static int CurrentItemIndex
		{
			get { return ReorderableListControl.CurrentItemIndex; }
		}

		public static Rect CurrentItemTotalPosition
		{
			get { return ReorderableListControl.CurrentItemTotalPosition; }
		}

		public static T DefaultItemDrawer<T>(Rect position, T item)
		{
			GUI.Label(position, "Item drawer not implemented.");
			return item;
		}

		public static string TextFieldItemDrawer(Rect position, string item)
		{
			if (item == null)
			{
				item = "";
				GUI.changed = true;
			}
			return EditorGUI.TextField(position, item);
		}

		public static void Title(GUIContent title)
		{
			Rect position = GUILayoutUtility.GetRect(title, ReorderableListStyles.Title);
			Title(position, title);
			GUILayout.Space(-1);
		}

		public static void Title(string title)
		{
			TempGuiContent.text = title;
			Title(TempGuiContent);
		}

		public static void Title(Rect position, GUIContent title)
		{
			if (Event.current.type == EventType.Repaint)
			{
				ReorderableListStyles.Title.Draw(position, title, false, false, false, false);
			}
		}

		public static void Title(Rect position, string text)
		{
			TempGuiContent.text = text;
			Title(position, TempGuiContent);
		}
		
		public static void ListField<T>(IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem,
			ReorderableListControl.DrawEmpty drawEmpty, float itemHeight, ReorderableListFlags flags)
		{
			DoListField(list, drawItem, drawEmpty, itemHeight, flags);
		}

		public static void ListFieldAbsolute<T>(Rect position, IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem,
			ReorderableListControl.DrawEmptyAbsolute drawEmpty, float itemHeight, ReorderableListFlags flags)
		{
			DoListFieldAbsolute(position, list, drawItem, drawEmpty, itemHeight, flags);
		}

		public static void ListField<T>(IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem,
			ReorderableListControl.DrawEmpty drawEmpty, float itemHeight)
		{
			DoListField(list, drawItem, drawEmpty, itemHeight, ReorderableListFlags.DisableReordering);
		}

		public static void ListFieldAbsolute<T>(Rect position, IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem,
			ReorderableListControl.DrawEmptyAbsolute drawEmpty, float itemHeight)
		{
			DoListFieldAbsolute(position, list, drawItem, drawEmpty, itemHeight, ReorderableListFlags.DisableReordering);
		}

		public static void ListField<T>(IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem,
			ReorderableListControl.DrawEmpty drawEmpty, ReorderableListFlags flags)
		{
			DoListField(list, drawItem, drawEmpty, DefaultItemHeight, flags);
		}

		public static void ListFieldAbsolute<T>(Rect position, IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem,
			ReorderableListControl.DrawEmptyAbsolute drawEmpty, ReorderableListFlags flags)
		{
			DoListFieldAbsolute(position, list, drawItem, drawEmpty, DefaultItemHeight, flags);
		}

		public static void ListField<T>(IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem,
			ReorderableListControl.DrawEmpty drawEmpty)
		{
			DoListField(list, drawItem, drawEmpty, DefaultItemHeight, ReorderableListFlags.DisableReordering);
		}

		public static void ListFieldAbsolute<T>(Rect position, IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem,
			ReorderableListControl.DrawEmptyAbsolute drawEmpty)
		{
			DoListFieldAbsolute(position, list, drawItem, drawEmpty, DefaultItemHeight, ReorderableListFlags.DisableReordering);
		}

		public static void ListField<T>(IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem, float itemHeight,
			ReorderableListFlags flags)
		{
			DoListField(list, drawItem, null, itemHeight, flags);
		}

		public static void ListFieldAbsolute<T>(Rect position, IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem,
			float itemHeight, ReorderableListFlags flags)
		{
			DoListFieldAbsolute(position, list, drawItem, null, itemHeight, flags);
		}

		public static void ListField<T>(IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem, float itemHeight)
		{
			DoListField(list, drawItem, null, itemHeight, ReorderableListFlags.DisableReordering);
		}

		public static void ListFieldAbsolute<T>(Rect position, IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem,
			float itemHeight)
		{
			DoListFieldAbsolute(position, list, drawItem, null, itemHeight, ReorderableListFlags.DisableReordering);
		}

		public static void ListField<T>(IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem,
			ReorderableListFlags flags)
		{
			DoListField(list, drawItem, null, DefaultItemHeight, flags);
		}

		public static void ListFieldAbsolute<T>(Rect position, IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem,
			ReorderableListFlags flags)
		{
			DoListFieldAbsolute(position, list, drawItem, null, DefaultItemHeight, flags);
		}

		public static void ListField<T>(IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem)
		{
			DoListField(list, drawItem, null, DefaultItemHeight, ReorderableListFlags.DisableReordering);
		}

		public static void ListFieldAbsolute<T>(Rect position, IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem)
		{
			DoListFieldAbsolute(position, list, drawItem, null, DefaultItemHeight, ReorderableListFlags.DisableReordering);
		}

		public static float CalculateListFieldHeight(int itemCount, float itemHeight, ReorderableListFlags flags)
		{
			// We need to push/pop flags so that nested controls are properly calculated.
			var restoreFlags = DefaultListControl.Flags;
			try
			{
				DefaultListControl.Flags = flags;
				return DefaultListControl.CalculateListHeight(itemCount, itemHeight);
			}
			finally
			{
				DefaultListControl.Flags = restoreFlags;
			}
		}

		public static float CalculateListFieldHeight(int itemCount, ReorderableListFlags flags)
		{
			return CalculateListFieldHeight(itemCount, DefaultItemHeight, flags);
		}

		public static float CalculateListFieldHeight(int itemCount, float itemHeight)
		{
			return CalculateListFieldHeight(itemCount, itemHeight, ReorderableListFlags.DisableReordering);
		}

		public static float CalculateListFieldHeight(int itemCount)
		{
			return CalculateListFieldHeight(itemCount, DefaultItemHeight, ReorderableListFlags.DisableReordering);
		}
		
		public static void ListField(SerializedProperty arrayProperty, ReorderableListControl.DrawEmpty drawEmpty,
			ReorderableListFlags flags)
		{
			DoListField(arrayProperty, 0, drawEmpty, flags);
		}

		public static void ListFieldAbsolute(Rect position, SerializedProperty arrayProperty,
			ReorderableListControl.DrawEmptyAbsolute drawEmpty, ReorderableListFlags flags)
		{
			DoListFieldAbsolute(position, arrayProperty, 0, drawEmpty, flags);
		}

		public static void ListField(SerializedProperty arrayProperty, ReorderableListControl.DrawEmpty drawEmpty)
		{
			DoListField(arrayProperty, 0, drawEmpty, ReorderableListFlags.DisableReordering);
		}

		public static void ListFieldAbsolute(Rect position, SerializedProperty arrayProperty,
			ReorderableListControl.DrawEmptyAbsolute drawEmpty)
		{
			DoListFieldAbsolute(position, arrayProperty, 0, drawEmpty, ReorderableListFlags.DisableReordering);
		}

		public static void ListField(SerializedProperty arrayProperty, ReorderableListFlags flags)
		{
			DoListField(arrayProperty, 0, null, flags);
		}

		public static void ListFieldAbsolute(Rect position, SerializedProperty arrayProperty, ReorderableListFlags flags)
		{
			DoListFieldAbsolute(position, arrayProperty, 0, null, flags);
		}

		public static void ListField(SerializedProperty arrayProperty)
		{
			DoListField(arrayProperty, 0, null, ReorderableListFlags.DisableReordering);
		}

		public static void ListFieldAbsolute(Rect position, SerializedProperty arrayProperty)
		{
			DoListFieldAbsolute(position, arrayProperty, 0, null, ReorderableListFlags.DisableReordering);
		}

		public static float CalculateListFieldHeight(SerializedProperty arrayProperty, ReorderableListFlags flags)
		{
			// We need to push/pop flags so that nested controls are properly calculated.
			var restoreFlags = DefaultListControl.Flags;
			try
			{
				DefaultListControl.Flags = flags;
				return DefaultListControl.CalculateListHeight(new SerializedPropertyAdaptor(arrayProperty));
			}
			finally
			{
				DefaultListControl.Flags = restoreFlags;
			}
		}

		public static float CalculateListFieldHeight(SerializedProperty arrayProperty)
		{
			return CalculateListFieldHeight(arrayProperty, ReorderableListFlags.DisableReordering);
		}

		public static void ListField(SerializedProperty arrayProperty, float fixedItemHeight,
			ReorderableListControl.DrawEmpty drawEmpty, ReorderableListFlags flags)
		{
			DoListField(arrayProperty, fixedItemHeight, drawEmpty, flags);
		}

		public static void ListFieldAbsolute(Rect position, SerializedProperty arrayProperty, float fixedItemHeight,
			ReorderableListControl.DrawEmptyAbsolute drawEmpty, ReorderableListFlags flags)
		{
			DoListFieldAbsolute(position, arrayProperty, fixedItemHeight, drawEmpty, flags);
		}

		public static void ListField(SerializedProperty arrayProperty, float fixedItemHeight,
			ReorderableListControl.DrawEmpty drawEmpty)
		{
			DoListField(arrayProperty, fixedItemHeight, drawEmpty, ReorderableListFlags.DisableReordering);
		}

		public static void ListFieldAbsolute(Rect position, SerializedProperty arrayProperty, float fixedItemHeight,
			ReorderableListControl.DrawEmptyAbsolute drawEmpty)
		{
			DoListFieldAbsolute(position, arrayProperty, fixedItemHeight, drawEmpty, ReorderableListFlags.DisableReordering);
		}

		public static void ListField(SerializedProperty arrayProperty, float fixedItemHeight, ReorderableListFlags flags)
		{
			DoListField(arrayProperty, fixedItemHeight, null, flags);
		}

		public static void ListFieldAbsolute(Rect position, SerializedProperty arrayProperty, float fixedItemHeight,
			ReorderableListFlags flags)
		{
			DoListFieldAbsolute(position, arrayProperty, fixedItemHeight, null, flags);
		}

		public static void ListField(SerializedProperty arrayProperty, float fixedItemHeight)
		{
			DoListField(arrayProperty, fixedItemHeight, null, ReorderableListFlags.DisableReordering);
		}

		public static void ListFieldAbsolute(Rect position, SerializedProperty arrayProperty, float fixedItemHeight)
		{
			DoListFieldAbsolute(position, arrayProperty, fixedItemHeight, null, ReorderableListFlags.DisableReordering);
		}

		private static void DoListField(IReorderableListAdaptor adaptor, ReorderableListControl.DrawEmpty drawEmpty,
			ReorderableListFlags flags = 0)
		{
			ReorderableListControl.DrawControlFromState(adaptor, drawEmpty, flags);
		}

		private static void DoListFieldAbsolute(Rect position, IReorderableListAdaptor adaptor,
			ReorderableListControl.DrawEmptyAbsolute drawEmpty, ReorderableListFlags flags = ReorderableListFlags.DisableReordering)
		{
			ReorderableListControl.DrawControlFromState(position, adaptor, drawEmpty, flags);
		}

		public static void ListField(IReorderableListAdaptor adaptor, ReorderableListControl.DrawEmpty drawEmpty,
			ReorderableListFlags flags)
		{
			DoListField(adaptor, drawEmpty, flags);
		}

		public static void ListFieldAbsolute(Rect position, IReorderableListAdaptor adaptor,
			ReorderableListControl.DrawEmptyAbsolute drawEmpty, ReorderableListFlags flags)
		{
			DoListFieldAbsolute(position, adaptor, drawEmpty, flags);
		}

		public static void ListField(IReorderableListAdaptor adaptor, ReorderableListControl.DrawEmpty drawEmpty)
		{
			DoListField(adaptor, drawEmpty);
		}

		public static void ListFieldAbsolute(Rect position, IReorderableListAdaptor adaptor,
			ReorderableListControl.DrawEmptyAbsolute drawEmpty)
		{
			DoListFieldAbsolute(position, adaptor, drawEmpty);
		}

		public static void ListField(IReorderableListAdaptor adaptor, ReorderableListFlags flags)
		{
			DoListField(adaptor, null, flags);
		}

		public static void ListFieldAbsolute(Rect position, IReorderableListAdaptor adaptor, ReorderableListFlags flags)
		{
			DoListFieldAbsolute(position, adaptor, null, flags);
		}

		public static void ListField(IReorderableListAdaptor adaptor)
		{
			DoListField(adaptor, null);
		}

		public static void ListFieldAbsolute(Rect position, IReorderableListAdaptor adaptor)
		{
			DoListFieldAbsolute(position, adaptor, null);
		}

		public static float CalculateListFieldHeight(IReorderableListAdaptor adaptor, ReorderableListFlags flags)
		{
			// We need to push/pop flags so that nested controls are properly calculated.
			var restoreFlags = DefaultListControl.Flags;
			try
			{
				DefaultListControl.Flags = flags;
				return DefaultListControl.CalculateListHeight(adaptor);
			}
			finally
			{
				DefaultListControl.Flags = restoreFlags;
			}
		}

		public static float CalculateListFieldHeight(IReorderableListAdaptor adaptor)
		{
			return CalculateListFieldHeight(adaptor, ReorderableListFlags.DisableReordering);
		}
		
		// -------------------------------------------------------------------
		// Private
		// -------------------------------------------------------------------
		private static ReorderableListControl DefaultListControl { get; set; }

		private static void DoListField<T>(IList<T> list, ReorderableListControl.ItemDrawer<T> drawItem,
			ReorderableListControl.DrawEmpty drawEmpty, float itemHeight, ReorderableListFlags flags)
		{
			var adaptor = new GenericListAdaptor<T>(list, drawItem, itemHeight);
			ReorderableListControl.DrawControlFromState(adaptor, drawEmpty, flags);
		}

		private static void DoListFieldAbsolute<T>(Rect position, IList<T> list,
			ReorderableListControl.ItemDrawer<T> drawItem, ReorderableListControl.DrawEmptyAbsolute drawEmpty, float itemHeight,
			ReorderableListFlags flags)
		{
			var adaptor = new GenericListAdaptor<T>(list, drawItem, itemHeight);
			ReorderableListControl.DrawControlFromState(position, adaptor, drawEmpty, flags);
		}

		private static void DoListField(SerializedProperty arrayProperty, float fixedItemHeight,
			ReorderableListControl.DrawEmpty drawEmpty, ReorderableListFlags flags)
		{
			var adaptor = new SerializedPropertyAdaptor(arrayProperty, fixedItemHeight);
			ReorderableListControl.DrawControlFromState(adaptor, drawEmpty, flags);
		}

		private static void DoListFieldAbsolute(Rect position, SerializedProperty arrayProperty, float fixedItemHeight,
			ReorderableListControl.DrawEmptyAbsolute drawEmpty, ReorderableListFlags flags)
		{
			var adaptor = new SerializedPropertyAdaptor(arrayProperty, fixedItemHeight);
			ReorderableListControl.DrawControlFromState(position, adaptor, drawEmpty, flags);
		}
	}
}
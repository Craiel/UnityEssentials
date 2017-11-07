namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList.ElementAdder
{
	using Contracts;
	using UnityEditor;
	using UnityEngine;

	internal sealed class GenericElementAdderMenu : IElementAdderMenu
	{

		private readonly GenericMenu innerMenu = new GenericMenu();

		// -------------------------------------------------------------------
		// Public
		// -------------------------------------------------------------------
		public void AddItem(GUIContent content, GenericMenu.MenuFunction handler)
		{
			this.innerMenu.AddItem(content, false, handler);
		}

		public void AddDisabledItem(GUIContent content)
		{
			this.innerMenu.AddDisabledItem(content);
		}

		public void AddSeparator(string path = "")
		{
			this.innerMenu.AddSeparator(path);
		}

		public bool IsEmpty
		{
			get { return this.innerMenu.GetItemCount() == 0; }
		}

		public void DropDown(Rect position)
		{
			this.innerMenu.DropDown(position);
		}
	}
}

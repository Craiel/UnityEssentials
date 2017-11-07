namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList.Contracts
{
	using UnityEngine;

	public interface IElementAdderMenu
	{
		bool IsEmpty { get; }
		
		void DropDown(Rect position);
	}
}

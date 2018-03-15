namespace Craiel.UnityEssentials.Editor.ReorderableList.Contracts
{
	using UnityEngine;

	public interface IElementAdderMenu
	{
		bool IsEmpty { get; }
		
		void DropDown(Rect position);
	}
}

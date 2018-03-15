namespace Craiel.UnityEssentials.Editor.ReorderableList.Contracts
{
	using UnityEngine;

	public interface IElementAdderMenuCommand<in T>
	{
		GUIContent Content { get; }
		
		bool CanExecute(IElementAdder<T> elementAdder);
		
		void Execute(IElementAdder<T> elementAdder);
	}

}

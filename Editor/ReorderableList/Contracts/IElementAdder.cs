namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList.Contracts
{
	using System;

	public interface IElementAdder<out T>
	{
		T Object { get; }

		bool CanAddElement(Type type);
		object AddElement(Type type);
	}
}
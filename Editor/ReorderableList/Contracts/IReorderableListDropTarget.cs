namespace Craiel.UnityEssentials.Editor.ReorderableList.Contracts
{
	public interface IReorderableListDropTarget
	{
		bool CanDropInsert(int insertionIndex);
		
		void ProcessDropInsertion(int insertionIndex);
	}
}

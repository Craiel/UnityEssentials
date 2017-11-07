namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList.Contracts
{
	using UnityEngine;

	public interface IReorderableListAdaptor
	{
		int Count { get; }

		bool CanDrag(int index);
		bool CanRemove(int index);

		void Add();

		void Insert(int index);

		void Duplicate(int index);

		void Remove(int index);

		void Move(int sourceIndex, int destIndex);

		void Clear();

		void BeginGUI();
		void EndGUI();

		void DrawItemBackground(Rect position, int index);

		void DrawItem(Rect position, int index);
		float GetItemHeight(int index);
	}
}
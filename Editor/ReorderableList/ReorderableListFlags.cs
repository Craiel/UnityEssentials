namespace Assets.Scripts.Craiel.Essentials.Editor.ReorderableList
{
	using System;

	[Flags]
	public enum ReorderableListFlags
	{
		DisableReordering = 0,
		HideAddButton = 1 << 0,
		HideRemoveButtons = 1 << 1,
		DisableContextMenu = 1 << 2,
		DisableDuplicateCommand = 1 << 3,
		DisableAutoFocus = 1 << 4,
		ShowIndices = 1 << 5,
		DisableAutoScroll = 1 << 6,
		ShowSizeField = 1 << 7
	}
}

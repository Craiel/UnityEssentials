namespace Craiel.UnityEssentials.Editor.ReorderableList
{
	using System;
	using System.Collections.Generic;
	using Assets.Scripts.Craiel.Essentials.Editor.ReorderableList;
	using Contracts;
	using UnityEngine;

	public class GenericListAdaptor<T> : IReorderableListAdaptor
	{
		private readonly IList<T> list;

		private readonly ReorderableListControl.ItemDrawer<T> itemDrawer;

		public float FixedItemHeight;

		// -------------------------------------------------------------------
		// Constructor
		// -------------------------------------------------------------------
		public GenericListAdaptor(IList<T> list, ReorderableListControl.ItemDrawer<T> itemDrawer, float itemHeight)
		{
			this.list = list;
			this.itemDrawer = itemDrawer ?? ReorderableListGUI.DefaultItemDrawer;
			this.FixedItemHeight = itemHeight;
		}
		
		// -------------------------------------------------------------------
		// Public
		// -------------------------------------------------------------------
		public IList<T> List
		{
			get { return this.list; }
		}

		public T this[int index]
		{
			get { return this.list[index]; }
		}

		public int Count
		{
			get { return this.list.Count; }
		}

		public virtual bool CanDrag(int index)
		{
			return true;
		}

		public virtual bool CanRemove(int index)
		{
			return true;
		}

		public virtual void Add()
		{
			this.list.Add(default(T));
		}

		public virtual void Insert(int index)
		{
			this.list.Insert(index, default(T));
		}

		public virtual void Duplicate(int index)
		{
			T newItem = this.list[index];

			ICloneable existingItem = newItem as ICloneable;
			if (existingItem != null)
			{
				newItem = (T) existingItem.Clone();
			}

			this.list.Insert(index + 1, newItem);
		}

		public virtual void Remove(int index)
		{
			this.list.RemoveAt(index);
		}

		public virtual void Move(int sourceIndex, int destIndex)
		{
			if (destIndex > sourceIndex)
			{
				--destIndex;
			}

			T item = this.list[sourceIndex];
			this.list.RemoveAt(sourceIndex);
			this.list.Insert(destIndex, item);
		}

		public virtual void Clear()
		{
			this.list.Clear();
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
			this.list[index] = this.itemDrawer(position, this.list[index]);
		}

		public virtual float GetItemHeight(int index)
		{
			return this.FixedItemHeight;
		}
	}
}

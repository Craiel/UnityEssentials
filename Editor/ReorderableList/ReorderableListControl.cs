namespace Craiel.UnityEssentials.Editor.ReorderableList
{
	using System;
	using System.Collections.Generic;
	using Contracts;
	using Runtime.Utils;
	using UnityEditor;
	using UnityEngine;

	public class ReorderableListControl
	{
		protected static readonly GUIContent CommandMoveToTop = new GUIContent("Move to Top");

		protected static readonly GUIContent CommandMoveToBottom = new GUIContent("Move to Bottom");

		protected static readonly GUIContent CommandInsertAbove = new GUIContent("Insert Above");

		protected static readonly GUIContent CommandInsertBelow = new GUIContent("Insert Below");

		protected static readonly GUIContent CommandDuplicate = new GUIContent("Duplicate");

		protected static readonly GUIContent CommandRemove = new GUIContent("Remove");

		protected static readonly GUIContent CommandClearAll = new GUIContent("Clear All");
		
		protected static readonly GenericMenu.MenuFunction2 DefaultContextHandler = DefaultContextMenuHandler;
		
		private static readonly int ListControlHint = "_ReorderableListControl_".GetHashCode();
		
		private static readonly Stack<ListInfo> CurrentListStack;

		private static readonly Stack<ItemInfo> CurrentItemStack;
		
		private static readonly GUIContent TempContent = new GUIContent();
		private static readonly GUIContent SizePrefixLabel = new GUIContent("Size");
		
		private static readonly Dictionary<int, float> ContainerHeightCache = new Dictionary<int, float>();
		
		private static GUIStyle rightAlignedLabelStyle;
		
		private static float anchorMouseOffset;

		private static int anchorIndex = -1;

		private static int targetIndex = -1;

		private static int autoFocusControlId;

		private static int autoFocusIndex = -1;
		
		private static int simulateMouseDragControlId;
		
		private static Rect dragItemPosition;

		private static Rect removeButtonPosition;
		
		private static bool trackingCancelBlockContext;
		
		private static int dropTargetNestedCounter;
		
		private static int contextControlId;

		private static int contextItemIndex;

		private static string contextCommandName;
		
		private ReorderableListFlags flags;
		
		private float verticalSpacing = 10f;
		
		private event AddMenuClickedEventHandler addMenuClicked;
		private int addMenuClickedSubscriberCount;
		
		private int controlId;

		private Rect visibleRect;

		private float indexLabelWidth;

		private bool tracking;

		private bool allowReordering;

		private bool allowDropInsertion;

		private int insertionIndex;

		private float insertionPosition;

		private int newSizeInput;
		
		
		// -------------------------------------------------------------------
		// Constructor
		// -------------------------------------------------------------------
		static ReorderableListControl()
		{
			CurrentListStack = new Stack<ListInfo>();
			CurrentListStack.Push(default(ListInfo));

			CurrentItemStack = new Stack<ItemInfo>();
			CurrentItemStack.Push(new ItemInfo(-1, default(Rect)));

			if (EditorGUIUtility.isProSkin)
			{
				AnchorBackgroundColor = new Color(85f / 255f, 85f / 255f, 85f / 255f, 0.85f);
				TargetBackgroundColor = new Color(0, 0, 0, 0.5f);
			}
			else
			{
				AnchorBackgroundColor = new Color(225f / 255f, 225f / 255f, 225f / 255f, 0.85f);
				TargetBackgroundColor = new Color(0, 0, 0, 0.5f);
			}
		}
		
		public ReorderableListControl()
		{
			this.HorizontalLineAtEnd = false;
			this.HorizontalLineAtStart = false;
			this.ContainerStyle = ReorderableListStyles.Container;
			this.FooterButtonStyle = ReorderableListStyles.FooterButton;
			this.ItemButtonStyle = ReorderableListStyles.ItemButton;

			this.HorizontalLineColor = ReorderableListStyles.HorizontalLineColor;
		}

		public ReorderableListControl(ReorderableListFlags flags)
			: this()
		{
			this.Flags = flags;
		}
		
		// -------------------------------------------------------------------
		// Public
		// -------------------------------------------------------------------
		public event AddMenuClickedEventHandler AddMenuClicked
		{
			add
			{
				if (value == null)
				{
					return;
				}
				
				this.addMenuClicked += value;
				++this.addMenuClickedSubscriberCount;
				this.HasAddMenuButton = this.addMenuClickedSubscriberCount != 0;
			}
			remove
			{
				if (value == null)
				{
					return;
				}
				
				this.addMenuClicked -= value;
				--this.addMenuClickedSubscriberCount;
				this.HasAddMenuButton = this.addMenuClickedSubscriberCount != 0;
			}
		}
		
		public event ItemInsertedEventHandler ItemInserted;
		public event ItemRemovingEventHandler ItemRemoving;
		public event ItemMovingEventHandler ItemMoving;
		public event ItemMovedEventHandler ItemMoved;
		
		public delegate T ItemDrawer<T>(Rect position, T item);

		public delegate void DrawEmpty();

		public delegate void DrawEmptyAbsolute(Rect position);

		public static readonly Color AnchorBackgroundColor;

		public static readonly Color TargetBackgroundColor;
		
		public static int CurrentListControlId
		{
			get { return CurrentListStack.Peek().ControlId; }
		}

		public static Rect CurrentListPosition
		{
			get { return CurrentListStack.Peek().Position; }
		}

		internal static int CurrentItemIndex
		{
			get { return CurrentItemStack.Peek().ItemIndex; }
		}

		public static Rect CurrentItemTotalPosition
		{
			get { return CurrentItemStack.Peek().ItemPosition; }
		}
		
		public ReorderableListFlags Flags
		{
			get { return this.flags; }
			set { this.flags = value; }
		}
		
		public float VerticalSpacing
		{
			get { return this.verticalSpacing; }
			set { this.verticalSpacing = value; }
		}
		
		public GUIStyle ContainerStyle { get; set; }

		public GUIStyle FooterButtonStyle { get; set; }

		public GUIStyle ItemButtonStyle { get; set; }

		public Color HorizontalLineColor { get; set; }

		public bool HorizontalLineAtStart { get; set; }

		public bool HorizontalLineAtEnd { get; set; }

		public static void DrawControlFromState(IReorderableListAdaptor adaptor, DrawEmpty drawEmpty,
			ReorderableListFlags flags)
		{
			int controlId = GetReorderableListControlId();

			var control = GUIUtility.GetStateObject(typeof(ReorderableListControl), controlId) as ReorderableListControl;
			control.Flags = flags;
			control.Draw(controlId, adaptor, drawEmpty);
		}

		public static void DrawControlFromState(Rect position, IReorderableListAdaptor adaptor, DrawEmptyAbsolute drawEmpty,
			ReorderableListFlags flags)
		{
			int controlId = GetReorderableListControlId();

			var control = GUIUtility.GetStateObject(typeof(ReorderableListControl), controlId) as ReorderableListControl;
			control.Flags = flags;
			control.Draw(position, controlId, adaptor, drawEmpty);
		}
		
		public void Draw(IReorderableListAdaptor adaptor, DrawEmpty drawEmpty)
		{
			this.Draw(GetReorderableListControlId(), adaptor, drawEmpty);
		}

		public void Draw(IReorderableListAdaptor adaptor)
		{
			this.Draw(GetReorderableListControlId(), adaptor, null);
		}
		
		public void Draw(Rect position, IReorderableListAdaptor adaptor, DrawEmptyAbsolute drawEmpty)
		{
			this.Draw(position, GetReorderableListControlId(), adaptor, drawEmpty);
		}

		public void Draw(Rect position, IReorderableListAdaptor adaptor)
		{
			this.Draw(position, GetReorderableListControlId(), adaptor, null);
		}

		public void DrawSizeField(Rect position, GUIContent label, IReorderableListAdaptor adaptor)
		{
			int sizeControlId = GUIUtility.GetControlID(FocusType.Passive);
			string sizeControlName = "ReorderableListControl.Size." + sizeControlId;
			GUI.SetNextControlName(sizeControlName);

			if (GUI.GetNameOfFocusedControl() == sizeControlName)
			{
				if (Event.current.rawType == EventType.KeyDown)
				{
					switch (Event.current.keyCode)
					{
						case KeyCode.Return:
						case KeyCode.KeypadEnter:
							this.ResizeList(adaptor, this.newSizeInput);
							Event.current.Use();
							break;
					}
				}
				this.newSizeInput = EditorGUI.IntField(position, label, this.newSizeInput);
			}
			else
			{
				EditorGUI.IntField(position, label, adaptor.Count);
				this.newSizeInput = adaptor.Count;
			}
		}

		public void DrawSizeField(Rect position, string label, IReorderableListAdaptor adaptor)
		{
			TempContent.text = label;
			this.DrawSizeField(position, TempContent, adaptor);
		}

		public void DrawSizeField(Rect position, IReorderableListAdaptor adaptor)
		{
			this.DrawSizeField(position, SizePrefixLabel, adaptor);
		}

		public void DrawSizeField(GUIContent label, IReorderableListAdaptor adaptor)
		{
			Rect position = GUILayoutUtility.GetRect(0, EditorGUIUtility.singleLineHeight);
			this.DrawSizeField(position, label, adaptor);
		}

		public void DrawSizeField(string label, IReorderableListAdaptor adaptor)
		{
			TempContent.text = label;
			this.DrawSizeField(TempContent, adaptor);
		}

		public void DrawSizeField(IReorderableListAdaptor adaptor)
		{
			this.DrawSizeField(SizePrefixLabel, adaptor);
		}
		
		public bool DoCommand(string commandName, int itemIndex, IReorderableListAdaptor adaptor)
		{
			if (!this.HandleCommand(contextCommandName, itemIndex, adaptor))
			{
				Debug.LogWarning("Unknown context command.");
				return false;
			}
			
			return true;
		}

		public bool DoCommand(GUIContent command, int itemIndex, IReorderableListAdaptor adaptor)
		{
			return this.DoCommand(command.text, itemIndex, adaptor);
		}

		public float CalculateListHeight(IReorderableListAdaptor adaptor)
		{
			this.FixStyles();

			float totalHeight = this.ContainerStyle.padding.vertical - 1 + this.VerticalSpacing;

			// Take list items into consideration.
			int count = adaptor.Count;
			for (int i = 0; i < count; ++i)
			{
				totalHeight += adaptor.GetItemHeight(i);
			}
			
			// Add spacing between list items.
			totalHeight += 4 * count;

			// Add height of footer buttons.
			if (this.HasFooterControls)
			{
				totalHeight += this.FooterButtonStyle.fixedHeight;
			}

			return totalHeight;
		}

		public float CalculateListHeight(int itemCount, float itemHeight)
		{
			this.FixStyles();

			float totalHeight = this.ContainerStyle.padding.vertical - 1 + this.VerticalSpacing;

			// Take list items into consideration.
			totalHeight += (itemHeight + 4) * itemCount;

			// Add height of footer buttons.
			if (this.HasFooterControls)
			{
				totalHeight += this.FooterButtonStyle.fixedHeight;
			}

			return totalHeight;
		}
		
		// -------------------------------------------------------------------
		// Protected
		// -------------------------------------------------------------------
		protected virtual void OnAddMenuClicked(AddMenuClickedEventArgs args)
		{
			if (this.addMenuClicked != null)
			{
				this.addMenuClicked(this, args);
			}
		}

		protected virtual void OnItemInserted(ItemInsertedEventArgs args)
		{
			if (this.ItemInserted != null)
			{
				this.ItemInserted(this, args);
			}
		}

		protected virtual void OnItemRemoving(ItemRemovingEventArgs args)
		{
			if (this.ItemRemoving != null)
			{
				this.ItemRemoving(this, args);
			}
		}
		
		protected virtual void OnItemMoving(ItemMovingEventArgs args)
		{
			if (this.ItemMoving != null)
			{
				this.ItemMoving(this, args);
			}
		}

		protected virtual void OnItemMoved(ItemMovedEventArgs args)
		{
			if (this.ItemMoved != null)
			{
				this.ItemMoved(this, args);
			}
		}

		protected virtual void DrawDropIndicator(Rect position)
		{
			ReorderableListGUIHelper.Separator(position);
		}

		protected virtual void AddItemsToMenu(GenericMenu menu, int itemIndex, IReorderableListAdaptor adaptor)
		{
			if ((this.Flags & ReorderableListFlags.DisableReordering) == 0)
			{
				if (itemIndex > 0)
				{
					menu.AddItem(CommandMoveToTop, false, DefaultContextHandler, CommandMoveToTop);
				}
				else
				{
					menu.AddDisabledItem(CommandMoveToTop);
				}

				if (itemIndex + 1 < adaptor.Count)
				{
					menu.AddItem(CommandMoveToBottom, false, DefaultContextHandler, CommandMoveToBottom);
				}
				else
				{
					menu.AddDisabledItem(CommandMoveToBottom);
				}

				if (this.HasAddButton)
				{
					menu.AddSeparator(string.Empty);

					menu.AddItem(CommandInsertAbove, false, DefaultContextHandler, CommandInsertAbove);
					menu.AddItem(CommandInsertBelow, false, DefaultContextHandler, CommandInsertBelow);

					if ((this.Flags & ReorderableListFlags.DisableDuplicateCommand) == 0)
					{
						menu.AddItem(CommandDuplicate, false, DefaultContextHandler, CommandDuplicate);
					}
				}
			}

			if (this.HasRemoveButtons)
			{
				if (menu.GetItemCount() > 0)
				{
					menu.AddSeparator(string.Empty);
				}

				menu.AddItem(CommandRemove, false, DefaultContextHandler, CommandRemove);
				menu.AddSeparator(string.Empty);
				menu.AddItem(CommandClearAll, false, DefaultContextHandler, CommandClearAll);
			}
		}

		protected virtual bool HandleCommand(string commandName, int itemIndex, IReorderableListAdaptor adaptor)
		{
			switch (commandName)
			{
				case "Move to Top":
				{
					this.MoveItem(adaptor, itemIndex, 0);
					return true;
				}
					
				case "Move to Bottom":
				{
					this.MoveItem(adaptor, itemIndex, adaptor.Count);
					return true;
				}

				case "Insert Above":
				{
					this.InsertItem(adaptor, itemIndex);
					return true;
				}
					
				case "Insert Below":
				{
					this.InsertItem(adaptor, itemIndex + 1);
					return true;
				}
					
				case "Duplicate":
				{
					this.DuplicateItem(adaptor, itemIndex);
					return true;
				}

				case "Remove":
				{
					this.RemoveItem(adaptor, itemIndex);
					return true;
				}
					
				case "Clear All":
				{
					this.ClearAll(adaptor);
					return true;
				}

				default:
				{
					return false;
				}
			}
		}

		protected void MoveItem(IReorderableListAdaptor adaptor, int sourceIndex, int destIndex)
		{
			// Raise event before moving item so that the operation can be cancelled.
			var movingEventArgs = new ItemMovingEventArgs(adaptor, sourceIndex, destIndex);
			this.OnItemMoving(movingEventArgs);
			if (!movingEventArgs.Cancel)
			{
				adaptor.Move(sourceIndex, destIndex);

				// Item was actually moved!
				int newIndex = destIndex;
				if (newIndex > sourceIndex)
				{
					--newIndex;
				}
				this.OnItemMoved(new ItemMovedEventArgs(adaptor, sourceIndex, newIndex));

				GUI.changed = true;
			}
			
			ReorderableListGUI.IndexOfChangedItem = -1;
		}

		protected void AddItem(IReorderableListAdaptor adaptor)
		{
			adaptor.Add();
			this.AutoFocusItem(contextControlId, adaptor.Count - 1);

			GUI.changed = true;
			ReorderableListGUI.IndexOfChangedItem = -1;

			var args = new ItemInsertedEventArgs(adaptor, adaptor.Count - 1, false);
			this.OnItemInserted(args);
		}

		protected void InsertItem(IReorderableListAdaptor adaptor, int itemIndex)
		{
			adaptor.Insert(itemIndex);
			this.AutoFocusItem(contextControlId, itemIndex);

			GUI.changed = true;
			ReorderableListGUI.IndexOfChangedItem = -1;

			var args = new ItemInsertedEventArgs(adaptor, itemIndex, false);
			this.OnItemInserted(args);
		}

		protected void DuplicateItem(IReorderableListAdaptor adaptor, int itemIndex)
		{
			adaptor.Duplicate(itemIndex);
			this.AutoFocusItem(contextControlId, itemIndex + 1);

			GUI.changed = true;
			ReorderableListGUI.IndexOfChangedItem = -1;

			var args = new ItemInsertedEventArgs(adaptor, itemIndex + 1, true);
			this.OnItemInserted(args);
		}

		protected bool RemoveItem(IReorderableListAdaptor adaptor, int itemIndex)
		{
			var args = new ItemRemovingEventArgs(adaptor, itemIndex);
			this.OnItemRemoving(args);
			if (args.Cancel)
			{
				return false;
			}

			adaptor.Remove(itemIndex);

			GUI.changed = true;
			ReorderableListGUI.IndexOfChangedItem = -1;

			return true;
		}

		protected bool ClearAll(IReorderableListAdaptor adaptor)
		{
			if (adaptor.Count == 0)
			{
				return true;
			}

			var args = new ItemRemovingEventArgs(adaptor, 0);
			int count = adaptor.Count;
			for (int i = 0; i < count; ++i)
			{
				args.ItemIndex = i;
				this.OnItemRemoving(args);
				if (args.Cancel)
				{
					return false;
				}
			}

			adaptor.Clear();

			GUI.changed = true;
			ReorderableListGUI.IndexOfChangedItem = -1;

			return true;
		}

		protected bool ResizeList(IReorderableListAdaptor adaptor, int newCount)
		{
			if (newCount < 0)
			{
				// Do nothing when new count is negative.
				return true;
			}

			int removeCount = Mathf.Max(0, adaptor.Count - newCount);
			int addCount = Mathf.Max(0, newCount - adaptor.Count);

			while (removeCount-- > 0)
			{
				if (!this.RemoveItem(adaptor, adaptor.Count - 1))
				{
					return false;
				}
			}
			while (addCount-- > 0)
			{
				this.AddItem(adaptor);
			}

			return true;
		}
		
		// -------------------------------------------------------------------
		// Private
		// -------------------------------------------------------------------
		private bool HasFooterControls
		{
			get { return this.HasSizeField || this.HasAddButton || this.HasAddMenuButton; }
		}

		private bool HasSizeField
		{
			get { return (this.flags & ReorderableListFlags.ShowSizeField) != 0; }
		}

		private bool HasAddButton
		{
			get { return (this.flags & ReorderableListFlags.HideAddButton) == 0; }
		}

		private bool HasAddMenuButton { get; set; }

		private bool HasRemoveButtons
		{
			get { return (this.flags & ReorderableListFlags.HideRemoveButtons) == 0; }
		}
		
		private static int GetReorderableListControlId()
		{
			return GUIUtility.GetControlID(ListControlHint, FocusType.Passive);
		}
		
		private static int CountDigits(int number)
		{
			return Mathf.Max(2, Mathf.CeilToInt(Mathf.Log10(number)));
		}
		
		private void PrepareState(int targetControlId, IReorderableListAdaptor adaptor)
		{
			this.controlId = targetControlId;
			this.visibleRect = ReorderableListGUIHelper.VisibleRect();

			if ((this.Flags & ReorderableListFlags.ShowIndices) != 0)
			{
				this.indexLabelWidth = CountDigits(adaptor.Count) * 8 + 8;
			}
			else
			{
				this.indexLabelWidth = 0;
			}

			this.tracking = IsTrackingControl(targetControlId);

			this.allowReordering = (this.Flags & ReorderableListFlags.DisableReordering) == 0;

			// The value of this field is reset each time the control is drawn and may
			// be invalidated when list items are drawn.
			this.allowDropInsertion = true;
		}

		private void AutoFocusItem(int targetControlId, int itemIndex)
		{
			if ((this.Flags & ReorderableListFlags.DisableAutoFocus) == 0)
			{
				autoFocusControlId = targetControlId;
				autoFocusIndex = itemIndex;
			}
		}

		private bool DoRemoveButton(Rect position, bool visible)
		{
			var iconNormal = ReorderableListResources.GetTexture(ReorderableListTexture.IconRemoveNormal);
			var iconActive = ReorderableListResources.GetTexture(ReorderableListTexture.IconRemoveActive);

			return ReorderableListGUIHelper.IconButton(position, visible, iconNormal, iconActive, this.ItemButtonStyle);
		}
		
		private static void BeginTrackingReorderDrag(int targetControlId, int itemIndex)
		{
			GUIUtility.hotControl = targetControlId;
			GUIUtility.keyboardControl = 0;
			anchorIndex = itemIndex;
			targetIndex = itemIndex;
			trackingCancelBlockContext = false;
		}

		private static void StopTrackingReorderDrag()
		{
			GUIUtility.hotControl = 0;
			anchorIndex = -1;
			targetIndex = -1;
		}

		private static bool IsTrackingControl(int targetControlId)
		{
			return !trackingCancelBlockContext && GUIUtility.hotControl == targetControlId;
		}

		private void AcceptReorderDrag(IReorderableListAdaptor adaptor)
		{
			try
			{
				// Reorder list as needed!
				targetIndex = Mathf.Clamp(targetIndex, 0, adaptor.Count + 1);
				if (targetIndex != anchorIndex && targetIndex != anchorIndex + 1)
				{
					this.MoveItem(adaptor, anchorIndex, targetIndex);
				}
			}
			finally
			{
				StopTrackingReorderDrag();
			}
		}

		private void DrawListItem(Rect position, IReorderableListAdaptor adaptor, int itemIndex)
		{
			bool isRepainting = Event.current.type == EventType.Repaint;
			bool isVisible = (position.y < this.visibleRect.yMax && position.yMax > this.visibleRect.y);
			bool isDraggable = this.allowReordering && adaptor.CanDrag(itemIndex);

			Rect itemContentPosition = position;
			itemContentPosition.x = position.x + 2;
			itemContentPosition.y += 1;
			itemContentPosition.width = position.width - 4;
			itemContentPosition.height = position.height - 4;

			// Make space for grab handle?
			if (isDraggable)
			{
				itemContentPosition.x += 20;
				itemContentPosition.width -= 20;
			}

			// Make space for element index.
			if (Math.Abs(this.indexLabelWidth) > EssentialMathUtils.Epsilon)
			{
				itemContentPosition.width -= this.indexLabelWidth;

				if (isRepainting && isVisible)
				{
					rightAlignedLabelStyle.Draw(
						new Rect(itemContentPosition.x, position.y, this.indexLabelWidth, position.height - 4), itemIndex + ":", false,
						false, false, false);
				}

				itemContentPosition.x += this.indexLabelWidth;
			}

			// Make space for remove button?
			if (this.HasRemoveButtons)
			{
				itemContentPosition.width -= 27;
			}

			try
			{
				CurrentItemStack.Push(new ItemInfo(itemIndex, position));
				EditorGUI.BeginChangeCheck();

				if (isRepainting && isVisible)
				{
					// Draw background of list item.
					var backgroundPosition = new Rect(position.x, position.y, position.width, position.height - 1);
					adaptor.DrawItemBackground(backgroundPosition, itemIndex);

					// Draw grab handle?
					if (isDraggable)
					{
						var texturePosition = new Rect(position.x + 6, position.y + position.height / 2f - 3, 9, 5);
						ReorderableListGUIHelper.DrawTexture(texturePosition, ReorderableListResources.GetTexture(ReorderableListTexture.GrabHandle));
					}

					// Draw horizontal line between list items.
					if (!this.tracking || itemIndex != anchorIndex)
					{
						if (itemIndex != 0 || this.HorizontalLineAtStart)
						{
							var horizontalLinePosition = new Rect(position.x, position.y - 1, position.width, 1);
							ReorderableListGUIHelper.Separator(horizontalLinePosition, this.HorizontalLineColor);
						}
					}
				}

				// Allow control to be automatically focused.
				if (autoFocusIndex == itemIndex)
				{
					GUI.SetNextControlName("AutoFocus_" + this.controlId + "_" + itemIndex);
				}

				// Present actual control.
				adaptor.DrawItem(itemContentPosition, itemIndex);

				if (EditorGUI.EndChangeCheck())
				{
					ReorderableListGUI.IndexOfChangedItem = itemIndex;
				}

				// Draw remove button?
				if (this.HasRemoveButtons && adaptor.CanRemove(itemIndex))
				{
					removeButtonPosition = position;
					removeButtonPosition.width = 27;
					removeButtonPosition.x = itemContentPosition.xMax + 2;
					removeButtonPosition.y -= 1;

					if (this.DoRemoveButton(removeButtonPosition, isVisible))
					{
						this.RemoveItem(adaptor, itemIndex);
					}
				}

				// Check for context click?
				if ((this.Flags & ReorderableListFlags.DisableContextMenu) == 0)
				{
					if (Event.current.GetTypeForControl(this.controlId) == EventType.ContextClick &&
					    position.Contains(Event.current.mousePosition))
					{
						this.ShowContextMenu(itemIndex, adaptor);
						Event.current.Use();
					}
				}
			}
			finally
			{
				CurrentItemStack.Pop();
			}
		}

		private void DrawFloatingListItem(IReorderableListAdaptor adaptor, float targetSlotPosition)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;

				// Fill background of target area.
				Rect targetPosition = dragItemPosition;
				targetPosition.y = targetSlotPosition - 1;
				targetPosition.height = 1;

				ReorderableListGUIHelper.Separator(targetPosition, this.HorizontalLineColor);

				--targetPosition.x;
				++targetPosition.y;
				targetPosition.width += 2;
				targetPosition.height = dragItemPosition.height - 1;

				GUI.color = TargetBackgroundColor;
				ReorderableListGUIHelper.DrawTexture(targetPosition, EditorGUIUtility.whiteTexture);

				// Fill background of item which is being dragged.
				--dragItemPosition.x;
				dragItemPosition.width += 2;
				--dragItemPosition.height;

				GUI.color = AnchorBackgroundColor;
				ReorderableListGUIHelper.DrawTexture(dragItemPosition, EditorGUIUtility.whiteTexture);

				++dragItemPosition.x;
				dragItemPosition.width -= 2;
				++dragItemPosition.height;

				// Draw horizontal splitter above and below.
				GUI.color = new Color(0f, 0f, 0f, 0.6f);
				targetPosition.y = dragItemPosition.y - 1;
				targetPosition.height = 1;
				ReorderableListGUIHelper.DrawTexture(targetPosition, EditorGUIUtility.whiteTexture);

				targetPosition.y += dragItemPosition.height;
				ReorderableListGUIHelper.DrawTexture(targetPosition, EditorGUIUtility.whiteTexture);

				GUI.color = restoreColor;
			}

			this.DrawListItem(dragItemPosition, adaptor, anchorIndex);
		}
		
		private void DrawListContainerAndItems(Rect position, IReorderableListAdaptor adaptor)
		{
			int initialDropTargetNestedCounterValue = dropTargetNestedCounter;

			// Get local copy of event information for efficiency.
			EventType eventType = Event.current.GetTypeForControl(this.controlId);
			Vector2 mousePosition = Event.current.mousePosition;

			int newTargetIndex = targetIndex;

			// Position of first item in list.
			float firstItemY = position.y + this.ContainerStyle.padding.top;
			// Maximum position of dragged item.
			float dragItemMaxY = (position.yMax - this.ContainerStyle.padding.bottom) - dragItemPosition.height + 1;

			bool isMouseDragEvent = eventType == EventType.MouseDrag;
			if (simulateMouseDragControlId == this.controlId && eventType == EventType.Repaint)
			{
				simulateMouseDragControlId = 0;
				isMouseDragEvent = true;
			}
			if (isMouseDragEvent && this.tracking)
			{
				// Reset target index and adjust when looping through list items.
				if (mousePosition.y < firstItemY)
				{
					newTargetIndex = 0;
				}
				else if (mousePosition.y >= position.yMax)
				{
					newTargetIndex = adaptor.Count;
				}

				dragItemPosition.y = Mathf.Clamp(mousePosition.y + anchorMouseOffset, firstItemY, dragItemMaxY);
			}

			switch (eventType)
			{
				case EventType.MouseDown:
					if (this.tracking)
					{
						// Cancel drag when other mouse button is pressed.
						trackingCancelBlockContext = true;
						Event.current.Use();
					}
					
					break;

				case EventType.MouseUp:
					if (this.controlId == GUIUtility.hotControl)
					{
						// Allow user code to change control over reordering during drag.
						if (!trackingCancelBlockContext && this.allowReordering)
						{
							this.AcceptReorderDrag(adaptor);
						}
						else
						{
							StopTrackingReorderDrag();
						}
						
						Event.current.Use();
					}
					
					break;

				case EventType.KeyDown:
					if (this.tracking && Event.current.keyCode == KeyCode.Escape)
					{
						StopTrackingReorderDrag();
						Event.current.Use();
					}
					
					break;

				case EventType.ExecuteCommand:
					if (contextControlId == this.controlId)
					{
						int itemIndex = contextItemIndex;
						try
						{
							this.DoCommand(contextCommandName, itemIndex, adaptor);
							Event.current.Use();
						}
						finally
						{
							contextControlId = 0;
							contextItemIndex = 0;
						}
					}
					
					break;

				case EventType.Repaint:
					// Draw caption area of list.
					this.ContainerStyle.Draw(position, GUIContent.none, false, false, false, false);
					break;
			}

			ReorderableListGUI.IndexOfChangedItem = -1;

			// Draw list items!
			Rect itemPosition = new Rect(position.x + this.ContainerStyle.padding.left, firstItemY,
				position.width - this.ContainerStyle.padding.horizontal, 0);
			float targetSlotPosition = dragItemMaxY;

			this.insertionIndex = 0;
			this.insertionPosition = itemPosition.yMax;

			float lastMidPoint;
			float lastHeight = 0f;

			int count = adaptor.Count;
			for (int i = 0; i < count; ++i)
			{
				itemPosition.y = itemPosition.yMax;
				itemPosition.height = 0;

				lastMidPoint = itemPosition.y - lastHeight / 2f;

				if (this.tracking)
				{
					// Does this represent the target index?
					if (i == targetIndex)
					{
						targetSlotPosition = itemPosition.y;
						itemPosition.y += dragItemPosition.height;
					}

					// Do not draw item if it is currently being dragged.
					// Draw later so that it is shown in front of other controls.
					if (i == anchorIndex)
					{
						continue;
					}

					// Update position for current item.
					itemPosition.height = adaptor.GetItemHeight(i) + 4;
					lastHeight = itemPosition.height;
				}
				else
				{
					// Update position for current item.
					itemPosition.height = adaptor.GetItemHeight(i) + 4;
					lastHeight = itemPosition.height;

					// Does this represent the drop insertion index?
					float midpoint = itemPosition.y + itemPosition.height / 2f;
					if (mousePosition.y > lastMidPoint && mousePosition.y <= midpoint)
					{
						this.insertionIndex = i;
						this.insertionPosition = itemPosition.y;
					}
				}

				if (this.tracking && isMouseDragEvent)
				{
					float midpoint = itemPosition.y + itemPosition.height / 2f;

					if (targetIndex < i)
					{
						if (dragItemPosition.yMax > lastMidPoint && dragItemPosition.yMax < midpoint)
						{
							newTargetIndex = i;
						}
					}
					else if (targetIndex > i)
					{
						if (dragItemPosition.y > lastMidPoint && dragItemPosition.y < midpoint)
						{
							newTargetIndex = i;
						}
					}

					/*if (s_DragItemPosition.y > itemPosition.y && s_DragItemPosition.y <= midpoint)
						newTargetIndex = i;
					else if (s_DragItemPosition.yMax > midpoint && s_DragItemPosition.yMax <= itemPosition.yMax)
						newTargetIndex = i + 1;*/
				}

				// Draw list item.
				this.DrawListItem(itemPosition, adaptor, i);

				// Did list count change (i.e. item removed)?
				if (adaptor.Count < count)
				{
					// We assume that it was this item which was removed, so --i allows us
					// to process the next item as usual.
					count = adaptor.Count;
					--i;
					continue;
				}

				// Event has already been used, skip to next item.
				if (Event.current.type != EventType.Used)
				{
					switch (eventType)
					{
						case EventType.MouseDown:
							if (GUI.enabled && itemPosition.Contains(mousePosition))
							{
								// Remove input focus from control before attempting a context click or drag.
								GUIUtility.keyboardControl = 0;

								if (this.allowReordering && adaptor.CanDrag(i) && Event.current.button == 0)
								{
									dragItemPosition = itemPosition;

									BeginTrackingReorderDrag(this.controlId, i);
									anchorMouseOffset = itemPosition.y - mousePosition.y;
									targetIndex = i;

									Event.current.Use();
								}
							}
							
							break;
					}
				}
			}

			if (this.HorizontalLineAtEnd)
			{
				var horizontalLinePosition = new Rect(itemPosition.x, position.yMax - this.ContainerStyle.padding.vertical,
					itemPosition.width, 1);
				ReorderableListGUIHelper.Separator(horizontalLinePosition, this.HorizontalLineColor);
			}

			lastMidPoint = position.yMax - lastHeight / 2f;

			// Assume that drop insertion is not allowed at this time; we can change our
			// mind a little further down ;)
			this.allowDropInsertion = false;

			// Item which is being dragged should be shown on top of other controls!
			if (IsTrackingControl(this.controlId))
			{
				if (isMouseDragEvent)
				{
					if (dragItemPosition.yMax >= lastMidPoint)
					{
						newTargetIndex = count;
					}

					targetIndex = newTargetIndex;

					// Force repaint to occur so that dragging rectangle is visible.
					// But only if this is a real MouseDrag event!!
					if (eventType == EventType.MouseDrag)
					{
						Event.current.Use();
					}
				}

				this.DrawFloatingListItem(adaptor, targetSlotPosition);
			}
			else
			{
				// Cannot react to drop insertion if a nested drop target has already reacted!
				if (dropTargetNestedCounter == initialDropTargetNestedCounterValue)
				{
					if (Event.current.mousePosition.y >= lastMidPoint)
					{
						this.insertionIndex = adaptor.Count;
						this.insertionPosition = itemPosition.yMax;
					}
					this.allowDropInsertion = true;
				}
			}

			// Fake control to catch input focus if auto focus was not possible.
			GUIUtility.GetControlID(FocusType.Keyboard);

			if (isMouseDragEvent && (this.Flags & ReorderableListFlags.DisableAutoScroll) == 0 &&
			    IsTrackingControl(this.controlId))
			{
				this.AutoScrollTowardsMouse();
			}
		}

		private static bool ContainsRect(Rect a, Rect b)
		{
			return a.Contains(new Vector2(b.xMin, b.yMin)) && a.Contains(new Vector2(b.xMax, b.yMax));
		}

		private void AutoScrollTowardsMouse()
		{
			const float triggerPaddingInPixels = 8f;
			const float maximumRangeInPixels = 4f;

			Rect visiblePosition = ReorderableListGUIHelper.VisibleRect();
			Vector2 mousePosition = Event.current.mousePosition;
			Rect mouseRect = new Rect(mousePosition.x - triggerPaddingInPixels, mousePosition.y - triggerPaddingInPixels,
				triggerPaddingInPixels * 2, triggerPaddingInPixels * 2);

			if (!ContainsRect(visiblePosition, mouseRect))
			{
				if (mousePosition.y < visiblePosition.center.y)
				{
					mousePosition = new Vector2(mouseRect.xMin, mouseRect.yMin);
				}
				else
				{
					mousePosition = new Vector2(mouseRect.xMax, mouseRect.yMax);
				}

				mousePosition.x = Mathf.Max(mousePosition.x - maximumRangeInPixels, mouseRect.xMax);
				mousePosition.y = Mathf.Min(mousePosition.y + maximumRangeInPixels, mouseRect.yMax);
				GUI.ScrollTo(new Rect(mousePosition.x, mousePosition.y, 1, 1));

				simulateMouseDragControlId = this.controlId;

				var focusedWindow = EditorWindow.focusedWindow;
				if (focusedWindow != null)
				{
					focusedWindow.Repaint();
				}
			}
		}

		private void HandleDropInsertion(Rect position, IReorderableListAdaptor adaptor)
		{
			var target = adaptor as IReorderableListDropTarget;
			if (target == null || !this.allowDropInsertion)
			{
				return;
			}

			if (target.CanDropInsert(this.insertionIndex))
			{
				++dropTargetNestedCounter;

				switch (Event.current.type)
				{
					case EventType.DragUpdated:
						DragAndDrop.visualMode = DragAndDropVisualMode.Move;
						DragAndDrop.activeControlID = this.controlId;
						target.ProcessDropInsertion(this.insertionIndex);
						Event.current.Use();
						break;

					case EventType.DragPerform:
						target.ProcessDropInsertion(this.insertionIndex);

						DragAndDrop.AcceptDrag();
						DragAndDrop.activeControlID = 0;
						Event.current.Use();
						break;

					default:
						target.ProcessDropInsertion(this.insertionIndex);
						break;
				}

				if (DragAndDrop.activeControlID == this.controlId && Event.current.type == EventType.Repaint)
				{
					this.DrawDropIndicator(new Rect(position.x, this.insertionPosition - 2, position.width, 3));
				}
			}
		}
		
		private void CheckForAutoFocusControl()
		{
			if (Event.current.type == EventType.Used)
			{
				return;
			}

			// Automatically focus control!
			if (autoFocusControlId == this.controlId)
			{
				autoFocusControlId = 0;
				ReorderableListGUIHelper.FocusTextInControl("AutoFocus_" + this.controlId + "_" + autoFocusIndex);
				autoFocusIndex = -1;
			}
		}

		private void DrawFooterControls(Rect position, IReorderableListAdaptor adaptor)
		{
			if (this.HasFooterControls)
			{
				Rect buttonPosition = new Rect(position.xMax - 30, position.yMax - 1, 30, this.FooterButtonStyle.fixedHeight);

				Rect menuButtonPosition = buttonPosition;
				var menuIconNormal = ReorderableListResources.GetTexture(ReorderableListTexture.IconAddMenuNormal);
				var menuIconActive = ReorderableListResources.GetTexture(ReorderableListTexture.IconAddMenuActive);

				if (this.HasSizeField)
				{
					// Draw size field.
					Rect sizeFieldPosition = new Rect(
						position.x,
						position.yMax + 1,
						Mathf.Max(150f, position.width / 3f),
						16f
					);

					this.DrawSizeFooterControl(sizeFieldPosition, adaptor);
				}

				if (this.HasAddButton)
				{
					// Draw add menu drop-down button.
					if (this.HasAddMenuButton)
					{
						menuButtonPosition.x = buttonPosition.xMax - 14;
						menuButtonPosition.xMax = buttonPosition.xMax;
						menuIconNormal = ReorderableListResources.GetTexture(ReorderableListTexture.IconMenuNormal);
						menuIconActive = ReorderableListResources.GetTexture(ReorderableListTexture.IconMenuActive);
						buttonPosition.width -= 5;
						buttonPosition.x = menuButtonPosition.x - buttonPosition.width + 1;
					}

					// Draw add item button.
					var iconNormal = ReorderableListResources.GetTexture(ReorderableListTexture.IconAddNormal);
					var iconActive = ReorderableListResources.GetTexture(ReorderableListTexture.IconAddActive);

					if (ReorderableListGUIHelper.IconButton(buttonPosition, true, iconNormal, iconActive, this.FooterButtonStyle))
					{
						// Append item to list.
						GUIUtility.keyboardControl = 0;
						this.AddItem(adaptor);
					}
				}

				if (this.HasAddMenuButton)
				{
					// Draw add menu drop-down button.
					if (ReorderableListGUIHelper.IconButton(menuButtonPosition, true, menuIconNormal, menuIconActive, this.FooterButtonStyle))
					{
						GUIUtility.keyboardControl = 0;
						Rect totalAddButtonPosition = buttonPosition;
						totalAddButtonPosition.xMax = position.xMax;
						this.OnAddMenuClicked(new AddMenuClickedEventArgs(adaptor, totalAddButtonPosition));

						// This will be helpful in many circumstances; including by default!
						GUIUtility.ExitGUI();
					}
				}
			}
		}

		private void DrawSizeFooterControl(Rect position, IReorderableListAdaptor adaptor)
		{
			float restoreLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 60f;

			this.DrawSizeField(position, adaptor);

			EditorGUIUtility.labelWidth = restoreLabelWidth;
		}
		
		private Rect GetListRectWithAutoLayout(IReorderableListAdaptor adaptor, float padding)
		{
			float totalHeight;

			// Calculate position of list field using layout engine.
			if (Event.current.type == EventType.Layout)
			{
				totalHeight = this.CalculateListHeight(adaptor);
				ContainerHeightCache[this.controlId] = totalHeight;
			}
			else
			{
				totalHeight = ContainerHeightCache.ContainsKey(this.controlId)
					? ContainerHeightCache[this.controlId]
					: 0;
			}

			totalHeight += padding;

			return GUILayoutUtility.GetRect(GUIContent.none, this.ContainerStyle, GUILayout.Height(totalHeight));
		}

		private Rect DrawLayoutListField(IReorderableListAdaptor adaptor, float padding)
		{
			Rect position = this.GetListRectWithAutoLayout(adaptor, padding);

			// Make room for footer buttons?
			if (this.HasFooterControls)
			{
				position.height -= this.FooterButtonStyle.fixedHeight;
			}

			// Make room for vertical spacing below footer buttons.
			position.height -= this.VerticalSpacing;

			CurrentListStack.Push(new ListInfo(this.controlId, position));
			try
			{
				// Draw list as normal.
				adaptor.BeginGUI();
				this.DrawListContainerAndItems(position, adaptor);
				this.HandleDropInsertion(position, adaptor);
				adaptor.EndGUI();
			}
			finally
			{
				CurrentListStack.Pop();
			}

			this.CheckForAutoFocusControl();

			return position;
		}

		private Rect DrawLayoutEmptyList(IReorderableListAdaptor adaptor, DrawEmpty drawEmpty)
		{
			Rect position = EditorGUILayout.BeginVertical(this.ContainerStyle);
			{
				if (drawEmpty != null)
				{
					drawEmpty();
				}
				else
				{
					Debug.LogError("Unexpected call to 'DrawLayoutEmptyList'");
				}

				CurrentListStack.Push(new ListInfo(this.controlId, position));
				try
				{
					adaptor.BeginGUI();
					this.insertionIndex = 0;
					this.insertionPosition = position.y + 2;
					this.HandleDropInsertion(position, adaptor);
					adaptor.EndGUI();
				}
				finally
				{
					CurrentListStack.Pop();
				}
			}
			EditorGUILayout.EndVertical();

			// Allow room for footer buttons?
			if (this.HasFooterControls)
			{
				GUILayoutUtility.GetRect(0, this.FooterButtonStyle.fixedHeight - 1);
			}

			return position;
		}

		private void DrawEmptyListControl(Rect position, DrawEmptyAbsolute drawEmpty)
		{
			if (Event.current.type == EventType.Repaint)
			{
				this.ContainerStyle.Draw(position, GUIContent.none, false, false, false, false);
			}

			// Take padding into consideration when drawing empty content.
			position = this.ContainerStyle.padding.Remove(position);

			if (drawEmpty != null)
			{
				drawEmpty(position);
			}
		}

		private void FixStyles()
		{
			this.ContainerStyle = this.ContainerStyle ?? ReorderableListStyles.Container;
			this.FooterButtonStyle = this.FooterButtonStyle ?? ReorderableListStyles.FooterButton;
			this.ItemButtonStyle = this.ItemButtonStyle ?? ReorderableListStyles.ItemButton;

			if (rightAlignedLabelStyle == null)
			{
				rightAlignedLabelStyle = new GUIStyle(GUI.skin.label);
				rightAlignedLabelStyle.alignment = TextAnchor.MiddleRight;
				rightAlignedLabelStyle.padding.right = 4;
			}
		}

		private void Draw(int targetControlId, IReorderableListAdaptor adaptor, DrawEmpty drawEmpty)
		{
			this.FixStyles();
			this.PrepareState(targetControlId, adaptor);

			Rect position;
			if (adaptor.Count > 0)
			{
				position = this.DrawLayoutListField(adaptor, 0f);
			}
			else if (drawEmpty == null)
			{
				position = this.DrawLayoutListField(adaptor, 5f);
			}
			else
			{
				position = this.DrawLayoutEmptyList(adaptor, drawEmpty);
			}

			this.DrawFooterControls(position, adaptor);
		}
		
		private void Draw(Rect position, int targetControlId, IReorderableListAdaptor adaptor, DrawEmptyAbsolute drawEmpty)
		{
			this.FixStyles();
			this.PrepareState(targetControlId, adaptor);

			// Allow for footer area.
			if (this.HasFooterControls)
			{
				position.height -= this.FooterButtonStyle.fixedHeight;
			}

			// Make room for vertical spacing below footer buttons.
			position.height -= this.VerticalSpacing;

			CurrentListStack.Push(new ListInfo(this.controlId, position));
			try
			{
				adaptor.BeginGUI();

				this.DrawListContainerAndItems(position, adaptor);
				this.HandleDropInsertion(position, adaptor);
				this.CheckForAutoFocusControl();

				if (adaptor.Count == 0)
				{
					ReorderableListGUI.IndexOfChangedItem = -1;
					this.DrawEmptyListControl(position, drawEmpty);
				}

				adaptor.EndGUI();
			}
			finally
			{
				CurrentListStack.Pop();
			}

			this.DrawFooterControls(position, adaptor);
		}
	
		private void ShowContextMenu(int itemIndex, IReorderableListAdaptor adaptor)
		{
			GenericMenu menu = new GenericMenu();

			contextControlId = this.controlId;
			contextItemIndex = itemIndex;

			this.AddItemsToMenu(menu, itemIndex, adaptor);

			if (menu.GetItemCount() > 0)
			{
				menu.ShowAsContext();
			}
		}

		private static void DefaultContextMenuHandler(object userData)
		{
			var commandContent = userData as GUIContent;
			if (commandContent == null || string.IsNullOrEmpty(commandContent.text))
			{
				return;
			}

			contextCommandName = commandContent.text;

			var e = EditorGUIUtility.CommandEvent("ReorderableListContextCommand");
			EditorWindow.focusedWindow.SendEvent(e);
		}
		
		private struct ListInfo
		{
			public ListInfo(int controlId, Rect position)
			{
				this.ControlId = controlId;
				this.Position = position;
			}
			
			public readonly int ControlId;
			public readonly Rect Position;
		}

		private struct ItemInfo
		{
			public ItemInfo(int itemIndex, Rect itemPosition)
			{
				this.ItemIndex = itemIndex;
				this.ItemPosition = itemPosition;
			}
			
			public readonly int ItemIndex;
			public readonly Rect ItemPosition;
		}
	}
}

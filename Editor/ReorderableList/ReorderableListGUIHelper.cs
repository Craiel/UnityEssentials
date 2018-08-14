namespace Craiel.UnityEssentials.Editor.ReorderableList
{
	using System;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;

	public static class ReorderableListGUIHelper
	{
		private static readonly Color SeparatorColor;
		private static readonly GUIStyle SeparatorStyle;
		
		private static readonly GUIStyle TempStyle = new GUIStyle();
		private static readonly GUIContent TempIconContent = new GUIContent();
		private static readonly int IconButtonHint = "_ReorderableIconButton_".GetHashCode();
		
		// -------------------------------------------------------------------
		// Constructor
		// -------------------------------------------------------------------
		static ReorderableListGUIHelper()
		{
			Type guiClipType = Type.GetType("UnityEngine.GUIClip,UnityEngine");
			if (guiClipType != null)
			{
				PropertyInfo visibleRectProperty = guiClipType.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if (visibleRectProperty != null)
				{
					VisibleRect = (Func<Rect>) Delegate.CreateDelegate(typeof(Func<Rect>), visibleRectProperty.GetGetMethod(true) ?? visibleRectProperty.GetGetMethod(false));
				}
			}

		    if (VisibleRect == null)
		    {
                Debug.LogError("Could not get VisibleRect property getter from GUIClip!");
                return;
		    }

			MethodInfo focusTextInControlMethod = typeof(EditorGUI).GetMethod("FocusTextInControl", BindingFlags.Static | BindingFlags.Public);
			if (focusTextInControlMethod == null)
			{
				focusTextInControlMethod = typeof(GUI).GetMethod("FocusControl", BindingFlags.Static | BindingFlags.Public);
			    if (focusTextInControlMethod == null)
			    {
                    Debug.LogError("Could not get FocusTextInControl MethodInfo!");
                    return;
			    }
			}

			FocusTextInControl = (Action<string>) Delegate.CreateDelegate(typeof(Action<string>), focusTextInControlMethod);

			SeparatorColor = EditorGUIUtility.isProSkin
				? new Color(0.11f, 0.11f, 0.11f)
				: new Color(0.5f, 0.5f, 0.5f);

			SeparatorStyle = new GUIStyle
			{
				normal = {background = EditorGUIUtility.whiteTexture},
				stretchWidth = true
			};
		}

		// -------------------------------------------------------------------
		// Public
		// -------------------------------------------------------------------
		public static Func<Rect> VisibleRect;

		public static Action<string> FocusTextInControl;

		public static void DrawTexture(Rect position, Texture2D texture)
		{
			if (Event.current.type != EventType.Repaint)
			{
				return;
			}

			TempStyle.normal.background = texture;

			TempStyle.Draw(position, GUIContent.none, false, false, false, false);
		}

		public static bool IconButton(Rect position, bool visible, Texture2D iconNormal, Texture2D iconActive, GUIStyle style)
		{
			int controldId = GUIUtility.GetControlID(IconButtonHint, FocusType.Passive);
			bool result = false;

			position.height += 1;

			switch (Event.current.GetTypeForControl(controldId))
			{
				case EventType.MouseDown:
					// Do not allow button to be pressed using right mouse button since
					// context menu should be shown instead!
					if (GUI.enabled && Event.current.button != 1 && position.Contains(Event.current.mousePosition))
					{
						GUIUtility.hotControl = controldId;
						GUIUtility.keyboardControl = 0;
						Event.current.Use();
					}
					
					break;

				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controldId)
					{
						Event.current.Use();
					}
					
					break;

				case EventType.MouseUp:
					if (GUIUtility.hotControl == controldId)
					{
						GUIUtility.hotControl = 0;
						result = position.Contains(Event.current.mousePosition);
						Event.current.Use();
					}
					
					break;

				case EventType.Repaint:
					if (visible)
					{
						bool isActive = GUIUtility.hotControl == controldId && position.Contains(Event.current.mousePosition);
						TempIconContent.image = isActive ? iconActive : iconNormal;
						position.height -= 1;
						style.Draw(position, TempIconContent, isActive, isActive, false, false);
					}
					
					break;
			}

			return result;
		}

		public static bool IconButton(Rect position, Texture2D iconNormal, Texture2D iconActive, GUIStyle style)
		{
			return IconButton(position, true, iconNormal, iconActive, style);
		}

		public static void Separator(Rect position, Color color)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Color restoreColor = GUI.color;
				GUI.color = color;
				SeparatorStyle.Draw(position, false, false, false, false);
				GUI.color = restoreColor;
			}
		}

		public static void Separator(Rect position)
		{
			Separator(position, SeparatorColor);
		}
	}
}

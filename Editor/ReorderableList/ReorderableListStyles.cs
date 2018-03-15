namespace Craiel.UnityEssentials.Editor.ReorderableList
{
	using Assets.Scripts.Craiel.Essentials.Editor.ReorderableList;
	using UnityEditor;
	using UnityEngine;

	public static class ReorderableListStyles
	{
		// -------------------------------------------------------------------
		// Constructor
		// -------------------------------------------------------------------
		static ReorderableListStyles()
		{
			Title = new GUIStyle
			{
				border = new RectOffset(2, 2, 2, 1),
				margin = new RectOffset(5, 5, 5, 0),
				padding = new RectOffset(5, 5, 3, 3),
				alignment = TextAnchor.MiddleLeft,
				normal =
				{
					background = ReorderableListResources.GetTexture(ReorderableListTexture.TitleBackground),
					textColor = EditorGUIUtility.isProSkin
						? new Color(0.8f, 0.8f, 0.8f)
						: new Color(0.2f, 0.2f, 0.2f)
				}
			};

			Container = new GUIStyle
			{
				border = new RectOffset(2, 2, 2, 2),
				margin = new RectOffset(5, 5, 0, 0),
				padding = new RectOffset(2, 2, 2, 2),
				normal = {background = ReorderableListResources.GetTexture(ReorderableListTexture.ContainerBackground)}
			};

			Container2 = new GUIStyle(Container)
			{
				normal = {background = ReorderableListResources.GetTexture(ReorderableListTexture.Container2Background)}
			};

			FooterButton = new GUIStyle
			{
				fixedHeight = 16,
				alignment = TextAnchor.MiddleCenter,
				normal = {background = ReorderableListResources.GetTexture(ReorderableListTexture.ButtonNormal)},
				active = {background = ReorderableListResources.GetTexture(ReorderableListTexture.ButtonActive)},
				border = new RectOffset(3, 3, 1, 3),
				padding = new RectOffset(2, 2, 0, 2),
				clipping = TextClipping.Overflow
			};

			FooterButton2 = new GUIStyle
			{
				fixedHeight = 18,
				alignment = TextAnchor.MiddleCenter,
				normal = {background = ReorderableListResources.GetTexture(ReorderableListTexture.Button2Normal)},
				active = {background = ReorderableListResources.GetTexture(ReorderableListTexture.Button2Active)},
				border = new RectOffset(3, 3, 3, 3),
				padding = new RectOffset(2, 2, 2, 2),
				clipping = TextClipping.Overflow
			};

			ItemButton = new GUIStyle
			{
				active =
				{
					background = ReorderableListResources.CreatePixelTexture("Dark Pixel (List GUI)", new Color32(18, 18, 18, 255))
				},
				imagePosition = ImagePosition.ImageOnly,
				alignment = TextAnchor.MiddleCenter,
				overflow = new RectOffset(0, 0, -1, 0),
				padding = new RectOffset(0, 0, 1, 0),
				contentOffset = new Vector2(0, -1f)
			};

			SelectedItem = new GUIStyle
			{
				normal =
				{
					background = ReorderableListResources.HighlightColorTexture,
					textColor = Color.white
				},
				fontSize = 12
			};
		}

		// -------------------------------------------------------------------
		// Public
		// -------------------------------------------------------------------
		public static GUIStyle Title { get; private set; }

		public static GUIStyle Container { get; private set; }

		public static GUIStyle Container2 { get; private set; }

		public static GUIStyle FooterButton { get; private set; }

		public static GUIStyle FooterButton2 { get; private set; }

		public static GUIStyle ItemButton { get; private set; }

		public static GUIStyle SelectedItem { get; private set; }

		public static Color HorizontalLineColor
		{
			get { return EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.14f) : new Color(0.59f, 0.59f, 0.59f, 0.55f); }
		}

		public static Color SelectionBackgroundColor
		{
			get { return EditorGUIUtility.isProSkin ? new Color32(62, 95, 150, 255) : new Color32(62, 125, 231, 255); }
		}
	}
}

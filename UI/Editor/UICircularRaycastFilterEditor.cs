namespace Craiel.UnityEssentialsUI.Editor
{
    using Runtime;
    using UnityEditor;
    using UnityEngine;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(UIRectangularRaycastFilter))]
    public class UICircularRaycastFilterEditor : Editor
    {
        private const string PreferencesKey = "CRA_UIRectRaycastFilter";
        
		private bool displayGeometry = true;
		
		// -------------------------------------------------------------------
		// Public
		// -------------------------------------------------------------------
		public void OnEnable()
		{
			this.displayGeometry = EditorPrefs.GetBool(PreferencesKey, true);
		}
		
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            this.displayGeometry = EditorGUILayout.Toggle("Display Geometry", this.displayGeometry);
			
			if (EditorGUI.EndChangeCheck())
			{
				EditorPrefs.SetBool(PreferencesKey, this.displayGeometry);
                SceneView.RepaintAll();
            }
		}
		
		public Vector3[] ScaledWorldCorners
		{
			get
			{
				UIRectangularRaycastFilter filter = this.target as UIRectangularRaycastFilter;
				Rect scaledRect = filter.ScaledRect;
                
				RectTransform rectTransform = (RectTransform)filter.transform;
				Vector3[] corners = new Vector3[4];

                 corners[0] = new Vector3(rectTransform.rect.x + scaledRect.x, rectTransform.rect.height + rectTransform.rect.y + scaledRect.y, 0f);
                 corners[1] = new Vector3(rectTransform.rect.x + scaledRect.x, rectTransform.rect.height + rectTransform.rect.y + scaledRect.y + scaledRect.height, 0f);
                 corners[2] = new Vector3(rectTransform.rect.x + scaledRect.x + scaledRect.width, rectTransform.rect.height + rectTransform.rect.y + scaledRect.y + scaledRect.height, 0f);
                 corners[3] = new Vector3(rectTransform.rect.x + scaledRect.x + scaledRect.width, rectTransform.rect.height + rectTransform.rect.y + scaledRect.y, 0f);
                 
                for (int i = 0; i < 4; i++)
                {
                    corners[i] += new Vector3(rectTransform.rect.width * rectTransform.pivot.x, 0f, 0f);
                    corners[i] += new Vector3(0f, (rectTransform.rect.height * (1f - rectTransform.pivot.y)) * -1f, 0f);
                    corners[i] = rectTransform.TransformPoint(corners[i]);
                }

                return corners;
			}
		}
		
		public void OnSceneGUI()
		{
			if (!this.displayGeometry)
			{
				return;
			}
			
			Vector3[] worldCorners = this.ScaledWorldCorners;
			
			Handles.color = Color.green;
			Handles.DrawLine(worldCorners[0], worldCorners[1]); // Left line
			Handles.DrawLine(worldCorners[1], worldCorners[2]); // Top line
			Handles.DrawLine(worldCorners[2], worldCorners[3]); // Right line
			Handles.DrawLine(worldCorners[3], worldCorners[0]); // Bottom line
		}
    }
}
namespace Craiel.UnityEssentialsUI.Runtime
{
	using UnityEngine;

	[AddComponentMenu(EssentialCoreUI.ComponentMenuFolder + "/Rectangular Raycast Filter")]
	[RequireComponent(typeof(RectTransform))]
	public class UIRectangularRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
    {
	    // -------------------------------------------------------------------
	    // Public
	    // -------------------------------------------------------------------
        [SerializeField] 
        public Vector2 Offset = Vector2.zero;
        
        [SerializeField] 
        public RectOffset Borders = new RectOffset();
		
		[Range(0f, 1f)]
		[SerializeField] 
		public float ScaleX = 1f;
		
		[Range(0f, 1f)]
		[SerializeField] 
		public float ScaleY = 1f;
		
		public Rect ScaledRect
		{
			get
			{
				RectTransform rectTransform = (RectTransform)this.transform;
				return new Rect(
					(this.Offset.x + this.Borders.left + (rectTransform.rect.x + ((rectTransform.rect.width - (rectTransform.rect.width * this.ScaleX)) / 2f))), 
					(this.Offset.y + this.Borders.bottom + (rectTransform.rect.y + ((rectTransform.rect.height - (rectTransform.rect.height * this.ScaleY)) / 2f))), 
					((rectTransform.rect.width * this.ScaleX) - this.Borders.left - this.Borders.right),
					((rectTransform.rect.height * this.ScaleY) - this.Borders.top - this.Borders.bottom)
				);
			}
		}
		
		public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
		{
			if (!this.enabled)
			{
				return true;
			}
			
			Vector2 localPositionPivotRelative;
			RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)this.transform, screenPoint, eventCamera, out localPositionPivotRelative);
			return this.ScaledRect.Contains(localPositionPivotRelative);
		}
    }
}
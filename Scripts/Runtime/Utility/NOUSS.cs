namespace NiqonNO.UI
{
	public static class NOUSS
	{
		private const string LabelSuffix = "__label";
		private const string BackgroundSuffix = "__background";
		private const string ButtonSuffix = "__button";
		private const string InputSuffix = "__input-container";
		private const string ContentSuffix = "__content-container";
		private const string ViewportSuffix = "__viewport";
		private const string HandleSuffix = "__handle";
		private const string DragAreaSuffix = "__drag-area";
		private const string TileSuffix = "__tile";
		
		private const string NextModifier = "--next";
		private const string PreviousModifier = "--previous";
		private const string VerticalModifier = "--vertical";
		private const string HorizontalModifier = "--horizontal";
		
		public const string TernaryClass = "ternary-slider";
		public static readonly string TernaryLabelClass = $"{TernaryClass}{LabelSuffix}";
		public static readonly string TernaryInputContainerClass = $"{TernaryClass}{InputSuffix}";
		public static readonly string TernaryBackgroundClass = $"{TernaryClass}{BackgroundSuffix}";
		public static readonly string TernaryDragAreaClass = $"{TernaryClass}{DragAreaSuffix}";
		public static readonly string TernaryHandleClass = $"{TernaryClass}{HandleSuffix}";
		
		public const string ItemSelectorClass = "toggle-selector";
		public static readonly string ToggleSelectorLabelClass = $"{ItemSelectorClass}{LabelSuffix}";
		public static readonly string ToggleSelectorInputContainerClass = $"{ItemSelectorClass}{InputSuffix}";
		public static readonly string ToggleSelectorInputContainerVerticalClass = $"{ToggleSelectorInputContainerClass}{VerticalModifier}";
		public static readonly string ToggleSelectorInputContainerHorizontalClass = $"{ToggleSelectorInputContainerClass}{HorizontalModifier}";
		public static readonly string ToggleSelectorButtonClass = $"{ItemSelectorClass}{ButtonSuffix}";
		public static readonly string ToggleSelectorButtonPreviousClass = $"{ToggleSelectorButtonClass}{PreviousModifier}";
		public static readonly string ToggleSelectorButtonNextClass = $"{ToggleSelectorButtonClass}{NextModifier}";
		public static readonly string ToggleSelectorViewportClass = $"{ItemSelectorClass}{ViewportSuffix}";
		public static readonly string ToggleSelectorViewportVerticalClass = $"{ToggleSelectorViewportClass}{VerticalModifier}";
		public static readonly string ToggleSelectorViewportHorizontalClass = $"{ToggleSelectorViewportClass}{HorizontalModifier}";
		public static readonly string ToggleSelectorContentContainerClass = $"{ItemSelectorClass}{ContentSuffix}";
		public static readonly string ToggleSelectorContentContainerVerticalClass = $"{ToggleSelectorContentContainerClass}{VerticalModifier}";
		public static readonly string ToggleSelectorContentContainerHorizontalClass = $"{ToggleSelectorContentContainerClass}{HorizontalModifier}";
		public static readonly string ToggleSelectorTile = $"{ItemSelectorClass}{TileSuffix}";
	}
}
namespace NiqonNO.UI
{
	public static class NOUSS
	{
		private const string LabelSuffix = "__label";
		private const string ButtonSuffix = "__button";
		private const string InputSuffix = "__input-container";
		private const string ContentSuffix = "__content-container";
		private const string ViewportSuffix = "__viewport";
		private const string HandleSuffix = "__handle";
		private const string DragAreaSuffix = "__drag-area";
		
		public const string TernaryClass = "ternary-slider";
		public static readonly string TernaryLabelClass = $"{TernaryClass}{LabelSuffix}";
		public static readonly string TernaryInputContainerClass = $"{TernaryClass}{InputSuffix}";
		public static readonly string TernaryHandleClass = $"{TernaryClass}{HandleSuffix}";
		public static readonly string TernaryDragAreaClass = $"{TernaryClass}{DragAreaSuffix}";
		
		public const string ItemSelectorClass = "toggle-selector";
		public static readonly string ToggleSelectorLabelClass = $"{ItemSelectorClass}{LabelSuffix}";
		public static readonly string ToggleSelectorInputContainerClass = $"{ItemSelectorClass}{InputSuffix}";
		public static readonly string ToggleSelectorInputContainerVerticalClass = $"{ToggleSelectorInputContainerClass}--vertical";
		public static readonly string ToggleSelectorInputContainerHorizontalClass = $"{ToggleSelectorInputContainerClass}--horizontal";
		public static readonly string ToggleSelectorButtonClass = $"{ItemSelectorClass}{ButtonSuffix}";
		public static readonly string ToggleSelectorButtonPreviousClass = $"{ToggleSelectorButtonClass}-previous";
		public static readonly string ToggleSelectorButtonNextClass = $"{ToggleSelectorButtonClass}-next";
		public static readonly string ToggleSelectorViewportClass = $"{ItemSelectorClass}{ViewportSuffix}";
		public static readonly string ToggleSelectorViewportVerticalClass = $"{ToggleSelectorViewportClass}--vertical";
		public static readonly string ToggleSelectorViewportHorizontalClass = $"{ToggleSelectorViewportClass}--horizontal";
		public static readonly string ToggleSelectorContentContainerClass = $"{ItemSelectorClass}{ContentSuffix}";
		public static readonly string ToggleSelectorContentContainerVerticalClass = $"{ToggleSelectorContentContainerClass}--vertical";
		public static readonly string ToggleSelectorContentContainerHorizontalClass = $"{ToggleSelectorContentContainerClass}--horizontal";
	}
}
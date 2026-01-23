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
		
		public const string ItemSelectorClass = "item-selector";
		public static readonly string ItemSelectorLabelClass = $"{ItemSelectorClass}{LabelSuffix}";
		public static readonly string ItemSelectorInputContainerClass = $"{ItemSelectorClass}{InputSuffix}";
		public static readonly string ItemSelectorInputContainerVerticalClass = $"{ItemSelectorInputContainerClass}--vertical";
		public static readonly string ItemSelectorInputContainerHorizontalClass = $"{ItemSelectorInputContainerClass}--horizontal";
		public static readonly string ItemSelectorButtonClass = $"{ItemSelectorClass}{ButtonSuffix}";
		public static readonly string ItemSelectorButtonPreviousClass = $"{ItemSelectorButtonClass}-previous";
		public static readonly string ItemSelectorButtonNextClass = $"{ItemSelectorButtonClass}-next";
		public static readonly string ItemSelectorViewportClass = $"{ItemSelectorClass}{ViewportSuffix}";
		public static readonly string ItemSelectorViewportVerticalClass = $"{ItemSelectorViewportClass}--vertical";
		public static readonly string ItemSelectorViewportHorizontalClass = $"{ItemSelectorViewportClass}--horizontal";
		public static readonly string ItemSelectorContentContainerClass = $"{ItemSelectorClass}{ContentSuffix}";
		public static readonly string ItemSelectorContentContainerVerticalClass = $"{ItemSelectorViewportClass}--vertical";
		public static readonly string ItemSelectorContentContainerHorizontalClass = $"{ItemSelectorViewportClass}--horizontal";
	}
}
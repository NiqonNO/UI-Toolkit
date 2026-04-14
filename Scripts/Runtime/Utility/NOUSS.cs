using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public static class NOUSS
	{
		private const string LabelSuffix = "__label";
		private const string ButtonSuffix = "__button";
		private const string InputSuffix = "__input-container";
		private const string ContentSuffix = "__content-container";
		private const string ViewportSuffix = "__viewport";
		private const string TrackerSuffix = "__tracker";
		private const string HandleSuffix = "__handle";
		private const string DragAreaSuffix = "__drag-area";
		private const string TileSuffix = "__tile";
		private const string PreviewSuffix = "__preview";
		
		private const string NextModifier = "--next";
		private const string PreviousModifier = "--previous";
		private const string VerticalModifier = "--vertical";
		private const string HorizontalModifier = "--horizontal";
		private const string RoundModifier = "--round";
		private const string SquareModifier = "--square";
		
		public const string TernaryStylePath = "uss_Ternary";
		public const string TernaryClass = "ternary-slider";
		public const string TernaryLabelClass = TernaryClass + LabelSuffix;
		public const string TernaryInputContainerClass = TernaryClass + InputSuffix;
		public const string TernaryDragAreaClass = TernaryClass + DragAreaSuffix;
		public const string TernaryHandleClass = TernaryClass + HandleSuffix;
		
		public const string QuaternaryStylePath = "uss_Quaternary";
		public const string QuaternaryClass = "quaternary-slider";
		public const string QuaternaryLabelClass = QuaternaryClass + LabelSuffix;
		public const string QuaternaryInputContainerClass = QuaternaryClass + InputSuffix;
		public const string QuaternaryDragAreaClass = QuaternaryClass + DragAreaSuffix;
		public const string QuaternaryHandleClass = QuaternaryClass + HandleSuffix;
		
		public const string ColorPickerStylePath = "uss_Color Picker";
		public const string ColorPickerClass = "color-picker";
		public const string ColorPickerLabelClass = ColorPickerClass + LabelSuffix;
		public const string ColorPickerInputContainerClass = ColorPickerClass + InputSuffix;
		public const string ColorPickerPlotClass = ColorPickerClass + "-plot";
		public const string ColorPickerPlotRoundClass = ColorPickerPlotClass + RoundModifier;
		public const string ColorPickerPlotSquareClass = ColorPickerPlotClass + SquareModifier;
		public const string ColorPickerPlotDragAreaClass = ColorPickerPlotClass + DragAreaSuffix;
		public const string ColorPickerPlotDragAreaRoundClass = ColorPickerPlotDragAreaClass + RoundModifier;
		public const string ColorPickerPlotDragAreaSquareClass = ColorPickerPlotDragAreaClass + SquareModifier;
		public const string ColorPickerPlotHandleClass = ColorPickerPlotClass + HandleSuffix;
		public const string ColorPickerPlotPreviewClass = ColorPickerPlotClass + PreviewSuffix;
		public const string ColorPickerSliderClass = ColorPickerClass + "-slider";
		public const string ColorPickerSliderInputContainerClass = ColorPickerSliderClass + InputSuffix;
		public const string ColorPickerSliderDragAreaClass = ColorPickerSliderClass + DragAreaSuffix;
		public const string ColorPickerSliderTrackerClass = ColorPickerSliderClass + TrackerSuffix;
		public const string ColorPickerSliderHandleClass = ColorPickerSliderClass + HandleSuffix;
		public const string ColorPickerSliderPreviewClass = ColorPickerSliderClass + PreviewSuffix;
		
		public const string ToggleSelectorStylePath = "uss_Item Selector";
		public const string ItemSelectorClass = "toggle-selector";
		public const string ToggleSelectorLabelClass = ItemSelectorClass + LabelSuffix;
		public const string ToggleSelectorInputContainerClass = ItemSelectorClass + InputSuffix;
		public const string ToggleSelectorInputContainerVerticalClass = ToggleSelectorInputContainerClass + VerticalModifier;
		public const string ToggleSelectorInputContainerHorizontalClass = ToggleSelectorInputContainerClass + HorizontalModifier;
		public const string ToggleSelectorButtonClass = ItemSelectorClass + ButtonSuffix;
		public const string ToggleSelectorButtonPreviousClass = ToggleSelectorButtonClass + PreviousModifier;
		public const string ToggleSelectorButtonNextClass = ToggleSelectorButtonClass + NextModifier;
		public const string ToggleSelectorViewportClass = ItemSelectorClass + ViewportSuffix;
		public const string ToggleSelectorViewportVerticalClass = ToggleSelectorViewportClass + VerticalModifier;
		public const string ToggleSelectorViewportHorizontalClass = ToggleSelectorViewportClass + HorizontalModifier;
		public const string ToggleSelectorContentContainerClass = ItemSelectorClass + ContentSuffix;
		public const string ToggleSelectorContentContainerVerticalClass = ToggleSelectorContentContainerClass + VerticalModifier;
		public const string ToggleSelectorContentContainerHorizontalClass = ToggleSelectorContentContainerClass + HorizontalModifier;
		public const string ToggleSelectorTile = ItemSelectorClass + TileSuffix;

		public static void TryToApplyStyle(VisualElement element, string path)
		{
			StyleSheet style = Resources.Load<StyleSheet>(path);
			if(style == null) return;
			element.styleSheets.Insert(0, style);
		}
	}
}
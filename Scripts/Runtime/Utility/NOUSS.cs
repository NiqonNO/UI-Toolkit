using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public static class NOUSS
	{
		public const string LabelSuffix = "__label";
		public const string ButtonSuffix = "__button";
		public const string IconSuffix = "__icon";
		public const string InputSuffix = "__input";
		public const string ViewportSuffix = "__viewport";
		public const string ContentSuffix = "__content";
		public const string HeaderSuffix = "__header";
		public const string TrackerSuffix = "__tracker";
		public const string HandleSuffix = "__handle";
		public const string DragAreaSuffix = "__drag-area";
		public const string TileSuffix = "__tile";
		public const string PreviewSuffix = "__preview";
		public const string CategorySuffix = "__category";
		public const string DropdownSuffix = "__dropdown";
		public const string BorderSuffix = "__border";
		public const string BackgroundSuffix = "__background";
		public const string HighlightSuffix = "__highlight";
		
		public const string Container = "-container";
		public const string Space = "--";
		
		public const string NextModifier = "--next";
		public const string PreviousModifier = "--previous";
		public const string VerticalModifier = "--vertical";
		public const string HorizontalModifier = "--horizontal";
		public const string RoundModifier = "--round";
		public const string SquareModifier = "--square";
		public const string OpenModifier = "--open";
		public const string EndModifier = "--close";
		
		public const string TernaryStylePath = "uss_Ternary";
		public const string TernaryClass = "ternary-slider";
		public const string TernaryLabelClass = TernaryClass + LabelSuffix;
		public const string TernaryInputContainerClass = TernaryClass + InputSuffix + Container;
		public const string TernaryDragAreaClass = TernaryClass + DragAreaSuffix;
		public const string TernaryHandleClass = TernaryClass + HandleSuffix;
		
		public const string QuaternaryStylePath = "uss_Quaternary";
		public const string QuaternaryClass = "quaternary-slider";
		public const string QuaternaryLabelClass = QuaternaryClass + LabelSuffix;
		public const string QuaternaryInputContainerClass = QuaternaryClass + InputSuffix + Container;
		public const string QuaternaryDragAreaClass = QuaternaryClass + DragAreaSuffix;
		public const string QuaternaryHandleClass = QuaternaryClass + HandleSuffix;
		
		public const string ColorPickerStylePath = "uss_Color Picker";
		public const string ColorPickerClass = "color-picker";
		public const string ColorPickerLabelClass = ColorPickerClass + LabelSuffix;
		public const string ColorPickerInputContainerClass = ColorPickerClass + InputSuffix + Container;
		public const string ColorPickerPlotClass = ColorPickerClass + "-plot";
		public const string ColorPickerPlotRoundClass = ColorPickerPlotClass + RoundModifier;
		public const string ColorPickerPlotSquareClass = ColorPickerPlotClass + SquareModifier;
		public const string ColorPickerPlotDragAreaClass = ColorPickerPlotClass + DragAreaSuffix;
		public const string ColorPickerPlotDragAreaRoundClass = ColorPickerPlotDragAreaClass + RoundModifier;
		public const string ColorPickerPlotDragAreaSquareClass = ColorPickerPlotDragAreaClass + SquareModifier;
		public const string ColorPickerPlotHandleClass = ColorPickerPlotClass + HandleSuffix;
		public const string ColorPickerPlotPreviewClass = ColorPickerPlotClass + PreviewSuffix;
		public const string ColorPickerSliderClass = ColorPickerClass + "-slider";
		public const string ColorPickerSliderInputContainerClass = ColorPickerSliderClass + InputSuffix + Container;
		public const string ColorPickerSliderDragAreaClass = ColorPickerSliderClass + DragAreaSuffix;
		public const string ColorPickerSliderTrackerClass = ColorPickerSliderClass + TrackerSuffix;
		public const string ColorPickerSliderHandleClass = ColorPickerSliderClass + HandleSuffix;
		public const string ColorPickerSliderPreviewClass = ColorPickerSliderClass + PreviewSuffix;
		
		public const string ToggleSelectorStylePath = "uss_Item Selector";
		public const string ItemSelectorClass = "toggle-selector";
		public const string ToggleSelectorLabelClass = ItemSelectorClass + LabelSuffix;
		public const string ToggleSelectorInputContainerClass = ItemSelectorClass + InputSuffix + Container;
		public const string ToggleSelectorInputContainerVerticalClass = ToggleSelectorInputContainerClass + VerticalModifier;
		public const string ToggleSelectorInputContainerHorizontalClass = ToggleSelectorInputContainerClass + HorizontalModifier;
		public const string ToggleSelectorButtonClass = ItemSelectorClass + ButtonSuffix;
		public const string ToggleSelectorButtonPreviousClass = ToggleSelectorButtonClass + PreviousModifier;
		public const string ToggleSelectorButtonNextClass = ToggleSelectorButtonClass + NextModifier;
		public const string ToggleSelectorViewportClass = ItemSelectorClass + ViewportSuffix;
		public const string ToggleSelectorViewportVerticalClass = ToggleSelectorViewportClass + VerticalModifier;
		public const string ToggleSelectorViewportHorizontalClass = ToggleSelectorViewportClass + HorizontalModifier;
		public const string ToggleSelectorContentContainerClass = ItemSelectorClass + ContentSuffix + Container;
		public const string ToggleSelectorContentContainerVerticalClass = ToggleSelectorContentContainerClass + VerticalModifier;
		public const string ToggleSelectorContentContainerHorizontalClass = ToggleSelectorContentContainerClass + HorizontalModifier;
		public const string ToggleSelectorTile = ItemSelectorClass + TileSuffix;
		
		public const string MultiCategoryScrollViewStylePath = "uss_Multi Scroll View";
		public const string MultiCategoryScrollViewClass = "multi-scroll-view";
		public const string MultiCategoryScrollViewCategoryClass = MultiCategoryScrollViewClass + CategorySuffix;
		public const string MultiCategoryScrollViewDropdownClass = MultiCategoryScrollViewClass + DropdownSuffix;
		public const string MultiCategoryScrollViewContentListViewportClass = MultiCategoryScrollViewClass + ViewportSuffix + Container;
		public const string MultiCategoryScrollViewContentListContentClass = MultiCategoryScrollViewClass + ContentSuffix + Container;

		public static void TryToApplyStyle(VisualElement element, string path)
		{
			StyleSheet style = Resources.Load<StyleSheet>(path);
			if(style == null) return;
			element.styleSheets.Insert(0, style);
		}
	}
}
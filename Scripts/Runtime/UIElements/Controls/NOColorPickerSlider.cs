using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOColorPickerSlider : Slider
	{
		private VisualElement InputContainer;
		private VisualElement DragContainer;
		private VisualElement DragTracker;
		private VisualElement DragHandle;
		
		public NOColorPickerSlider(SliderDirection direction)
			: base(0, 1, direction)
		{
			name = "color-slider-container";
			AddToClassList(NOUSS.ColorPickerSliderClass);

			InputContainer = this.Q(className: inputUssClassName);
			InputContainer.AddToClassList(NOUSS.ColorPickerSliderInputContainerClass);
			
			DragContainer = this.Q(className: dragContainerUssClassName);
			DragContainer.AddToClassList(NOUSS.ColorPickerSliderDragAreaClass);
			
			DragTracker = this.Q(className: trackerUssClassName);
			DragTracker.AddToClassList(NOUSS.ColorPickerSliderTrackerClass);
			
			DragHandle = this.Q(className: draggerUssClassName);
			DragHandle.AddToClassList(NOUSS.ColorPickerSliderHandleClass);
		}
		
		public void AddToHandle(VisualElement element)
		{
			DragHandle.Add(element);
		}
		
		public void SetMaterial(Material drawerMaterial)
		{
			DragTracker.style.unityMaterial = new StyleMaterialDefinition(drawerMaterial);
		}
	}
}
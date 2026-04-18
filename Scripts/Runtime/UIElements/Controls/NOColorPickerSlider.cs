using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOColorPickerSlider : Slider
	{
		private const string DrawerShaderName = "Shader Graphs/shg_NO_UI_ColorSlider";
		private static readonly string[] KeywordNames = { "_COLOR_PICKER_TYPE_LS_H", "_COLOR_PICKER_TYPE_HL_S", "_COLOR_PICKER_TYPE_HS_L" };
		private static readonly int HueShaderProperty = Shader.PropertyToID("_Hue");
		private static Shader DrawerShader = Shader.Find(DrawerShaderName);
		
		private VisualElement InputContainer;
		private VisualElement DragContainer;
		private VisualElement DragTracker;
		private VisualElement DragHandle;
		
		private MaterialDefinition PickerSliderMaterial;
		private LocalKeyword[] Keywords;
		
		public NOColorPickerSlider(SliderDirection direction) : this(direction, DrawerShader) { }
		public NOColorPickerSlider(SliderDirection direction, Shader pickerDrawerShader) : base(0, 1, direction)
		{
			AddToClassList(NOUSS.ColorPickerSliderClass);

			InputContainer = this.Q(className: inputUssClassName);
			InputContainer.AddToClassList(NOUSS.ColorPickerSliderInputContainerClass);
			
			DragContainer = this.Q(className: dragContainerUssClassName);
			DragContainer.AddToClassList(NOUSS.ColorPickerSliderDragAreaClass);
			
			DragTracker = this.Q(className: trackerUssClassName);
			DragTracker.AddToClassList(NOUSS.ColorPickerSliderTrackerClass);
			DragTracker.style.unityMaterial = CreateMaterial(pickerDrawerShader);
			
			DragHandle = this.Q(className: draggerUssClassName);
			DragHandle.AddToClassList(NOUSS.ColorPickerSliderHandleClass);
		}
		
		public void AddToHandle(VisualElement element)
		{
			DragHandle.Add(element);
		}

		private StyleMaterialDefinition CreateMaterial(Shader shader)
		{
			if (shader == null)
			{
				if (DrawerShader == null)
				{
					Debug.LogError(
						$"Aborting material creation. Received null shader and fallback shader '{DrawerShaderName}' could not be resolved. " +
						$"Target object: '{DragTracker?.name}' in '{name}' ({nameof(NOColorPickerSlider)}).");
					return new StyleMaterialDefinition();
				}
				Debug.LogWarning(
					$"Received null shader. Falling back to default shader '{DrawerShaderName}'. " +
					$"Target object: '{DragTracker?.name}' in '{name}' ({nameof(NOColorPickerSlider)}).");
				shader = DrawerShader;
			}

			Keywords = new LocalKeyword[KeywordNames.Length];
			for (var i = 0; i < KeywordNames.Length; i++) 
				Keywords[i] = new LocalKeyword(shader, KeywordNames[i]);

			PickerSliderMaterial = new Material(shader);
			return new StyleMaterialDefinition(PickerSliderMaterial);
		}
		
		public void SetPickerType(int keyword)
		{
			if (PickerSliderMaterial.IsEmpty()) return;
			for (var i = 0; i < Keywords.Length; i++)
				PickerSliderMaterial.material.SetKeyword(Keywords[i], keyword == i);
		}
		
		public void SetMaterialProperty(float hueMaterialValue)
		{
			if (PickerSliderMaterial.IsEmpty()) return;
			PickerSliderMaterial.material.SetFloat(HueShaderProperty, hueMaterialValue);
		}
	}
}
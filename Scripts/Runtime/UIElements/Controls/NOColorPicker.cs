using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	[UxmlElement]
	public partial class NOColorPicker : BaseField<Color>
	{
		private static readonly string[] Keywords = { "_COLOR_PICKER_TYPE_LS_H", "_COLOR_PICKER_TYPE_HL_S", "_COLOR_PICKER_TYPE_HS_L" };
		private static readonly int EdgesOffsetShaderID = Shader.PropertyToID("_Wheel_Edges_Offset");
		private static readonly int ExternalShaderProperty = Shader.PropertyToID("_External");
		private static readonly int HueShaderProperty = Shader.PropertyToID("_Hue");
		
		private readonly VisualElement InputContainer;
		private readonly NOColorPickerPlot PickerPlot;
		private readonly VisualElement PickerColorPreview;
		private readonly NOColorPickerSlider PickerSlider;
		private readonly VisualElement SliderColorPreview;

		private ColorPickerType _PickerPlotType;
		[UxmlAttribute]
		private ColorPickerType PickerPlotType
		{
			get => _PickerPlotType;
			set
			{
				if (value == _PickerPlotType) return;
				_PickerPlotType = value;
				if (PickerPlotMaterial == null) return;
				SetMaterialKeyword();
				PickerPlot.IsWheel = IsColorWheel;
				OnValueChanged();
			}
		}

		private Vector2 _EdgesOffset;
		[UxmlAttribute, MinMaxSlider(0, 1, true), ShowIf(nameof(IsColorWheel))]
		private Vector2 EdgesOffset
		{
			get => _EdgesOffset;
			set
			{
				if (value == _EdgesOffset) return;
				_EdgesOffset = value;
				if (PickerPlotMaterial == null) return;
				SetMaterialOffset();
			}
		}

		private Material PickerPlotMaterial;
		[UxmlAttribute]
		private Shader PickerPlotDrawer
		{
			get => PickerPlotMaterial != null ? PickerPlotMaterial.shader : null;
			set
			{
				if (value == null) return;
				PickerPlotMaterial = new Material(value);
				SetMaterialKeyword();
				SetMaterialOffset();
				PickerPlot.SetMaterial(PickerPlotMaterial);
			}
		}
		
		private Material PickerSliderMaterial;
		[UxmlAttribute]
		private Shader PickerSliderDrawer
		{
			get => PickerSliderMaterial != null ? PickerSliderMaterial.shader : null;
			set
			{
				if (value == null) return;
				PickerSliderMaterial = new Material(value);
				SetMaterialKeyword();
				PickerSlider.SetMaterial(PickerSliderMaterial);
			}
		}
		
		private bool IsColorWheel => PickerPlotType != ColorPickerType.LightnessSaturation_Hue;
		
		private Vector3 HSV;
		
		public NOColorPicker() : this(string.Empty) { }
		public NOColorPicker(string label) : base(label, new VisualElement())
		{
			var styleSheet = NOUSS.GetStyleSheet(NOUSS.ColorPickerStylePath);
			if(styleSheet) styleSheets.Add(styleSheet);

			AddToClassList(NOUSS.ColorPickerClass);
			labelElement.AddToClassList(NOUSS.ColorPickerLabelClass);

			InputContainer = this.Q(className: inputUssClassName);
			InputContainer.AddToClassList(NOUSS.ColorPickerInputContainerClass);
			
			PickerPlot = new NOColorPickerPlot();
			PickerPlot.RegisterCallback<ChangeEvent<Vector2>>(SetPolar);
			
			PickerColorPreview = new VisualElement() { name = "color-picker-drag-handle-color-preview" };
			PickerColorPreview.AddToClassList(NOUSS.ColorPickerPlotPreviewClass);
			
			SliderColorPreview = new VisualElement() { name = "color-slider-drag-handle-color-preview" };
			SliderColorPreview.AddToClassList(NOUSS.ColorPickerSliderPreviewClass);

			PickerSlider = new NOColorPickerSlider(SliderDirection.Vertical);
			PickerSlider.RegisterCallback<ChangeEvent<float>>(SetSingle);
			
			InputContainer.Add(PickerPlot);
			PickerPlot.AddToHandle(PickerColorPreview);
			InputContainer.Add(PickerSlider);
			PickerSlider.AddToHandle(SliderColorPreview);
		}

		void SetMaterialKeyword()
		{
			for (var i = 0; i < Keywords.Length; i++)
			{
				if((int)PickerPlotType == i)
				{
					PickerPlotMaterial?.EnableKeyword(Keywords[i]);
					PickerSliderMaterial?.EnableKeyword(Keywords[i]);
				}
				else
				{
					PickerPlotMaterial?.DisableKeyword(Keywords[i]);
					PickerSliderMaterial?.DisableKeyword(Keywords[i]);
				}
			}
		}
		void SetMaterialOffset()
		{
			Vector4 offset = new Vector4(-EdgesOffset.x, 2-EdgesOffset.y, 0, 0);
			PickerPlotMaterial.SetVector(EdgesOffsetShaderID, offset);
		}

		private void SetSingle(ChangeEvent<float> evt)
		{
			HSV.x = evt.newValue;
			PickerPlotMaterial?.SetFloat(ExternalShaderProperty, evt.newValue);
			OnValueChanged();
		}

		private void SetPolar(ChangeEvent<Vector2> evt)
		{
			HSV.y = evt.newValue.x;
			HSV.z = evt.newValue.y;
			PickerSliderMaterial?.SetFloat(HueShaderProperty, evt.newValue.y);
			OnValueChanged();
		}

		public override void SetValueWithoutNotify(Color newValue)
		{
			PickerColorPreview.style.backgroundColor = new StyleColor(newValue);
			SliderColorPreview.style.backgroundColor = new StyleColor(newValue);
			base.SetValueWithoutNotify(newValue);
		}
		private void OnValueChanged()
		{
			value = PickerPlotType switch
			{
				ColorPickerType.LightnessSaturation_Hue => Color.HSVToRGB(HSV.x, HSV.y, HSV.z),
				ColorPickerType.HueLightness_Saturation => Color.HSVToRGB(HSV.z, HSV.x, HSV.y),
				ColorPickerType.HueSaturation_Lightness => Color.HSVToRGB(HSV.z, HSV.y, HSV.x),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		[Serializable]
		enum ColorPickerType
		{
			LightnessSaturation_Hue = 0,
			HueLightness_Saturation = 1,
			HueSaturation_Lightness = 2,
		}
	}
}
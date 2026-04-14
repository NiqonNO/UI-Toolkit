using System;
using Sirenix.OdinInspector;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	[UxmlElement]
	public partial class NOColorPicker : BaseField<Vector3>
	{
		private static readonly BindingId colorProperty = (BindingId) nameof (ColorValue);
		
		private readonly VisualElement InputContainer;
		private readonly NOColorPickerPlot PickerPlot;
		private readonly VisualElement PickerColorPreview;
		private readonly NOColorPickerSlider PickerSlider;
		private readonly VisualElement SliderColorPreview;

		private Color _ColorValue;
		[UxmlAttribute, CreateProperty]
		private Color ColorValue
		{
			get => _ColorValue;
			set
			{
				if (value == _ColorValue) return;
				_ColorValue = value;
				SetHSV(_ColorValue);
			}
		}
		
		private ColorPickerType _PickerPlotType;
		[UxmlAttribute, CreateProperty]
		private ColorPickerType PickerPlotType
		{
			get => _PickerPlotType;
			set
			{
				if (value == _PickerPlotType) return;
				Vector3 HSV = this.value;
				_PickerPlotType = value;
				SetPickerType();
				SetHSV(HSV);
			}
		}

		private Vector2 _EdgesOffset;
		[UxmlAttribute, CreateProperty, MinMaxSlider(0, 1, true), ShowIf(nameof(IsColorWheel))]
		private Vector2 EdgesOffset
		{
			get => _EdgesOffset;
			set
			{
				if (value == _EdgesOffset) return;
				_EdgesOffset = value;
				SetEdgesOffset();
			}
		}
		
		private bool IsColorWheel => PickerPlotType != ColorPickerType.ValueSaturation_Hue;
		
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
			PickerPlot.RegisterCallback<ChangeEvent<Vector2>>(OnPlotChange);
			
			PickerColorPreview = new VisualElement() { name = "color-picker-drag-handle-color-preview" };
			PickerColorPreview.AddToClassList(NOUSS.ColorPickerPlotPreviewClass);
			
			SliderColorPreview = new VisualElement() { name = "color-slider-drag-handle-color-preview" };
			SliderColorPreview.AddToClassList(NOUSS.ColorPickerSliderPreviewClass);

			PickerSlider = new NOColorPickerSlider(SliderDirection.Vertical);
			PickerSlider.RegisterCallback<ChangeEvent<float>>(OnSliderChange);
			
			InputContainer.Add(PickerPlot);
			PickerPlot.AddToHandle(PickerColorPreview);
			InputContainer.Add(PickerSlider);
			PickerSlider.AddToHandle(SliderColorPreview);

			SetPickerType();
			SetEdgesOffset();
		}

		Color GetHSVColor() => Color.HSVToRGB(Mathf.Clamp01(value.x), Mathf.Clamp01(value.y), Mathf.Clamp01(value.z));
		Vector3 GetHSV() => PickerPlotType switch
		{
			ColorPickerType.ValueSaturation_Hue => new Vector3(PickerSlider.value, PickerPlot.value.x, PickerPlot.value.y),
			ColorPickerType.HueValue_Saturation => new Vector3(PickerPlot.value.y, PickerSlider.value, PickerPlot.value.x),
			ColorPickerType.HueSaturation_Value => new Vector3(PickerPlot.value.y, PickerPlot.value.x, PickerSlider.value),
			_ => Vector3.one
		};

		void SetHSV(Vector3 hsv) => SetHSV(hsv.x, hsv.y, hsv.z);
		void SetHSV(float hue, float sat, float val)
		{
			switch (PickerPlotType)
			{
				case ColorPickerType.ValueSaturation_Hue:
					UpdateSliders(hue, sat, val);
					return;
				case ColorPickerType.HueValue_Saturation:
					UpdateSliders(sat, val, hue);
					return;
				case ColorPickerType.HueSaturation_Value:
					UpdateSliders(val, sat, hue);
					return;
			}
		}
		void SetHSV(Color color)
		{
			Color.RGBToHSV(color, out var hue, out var sat, out var val);
			bool svZero = sat == 0 || val == 0;
			switch (PickerPlotType)
			{
				case ColorPickerType.ValueSaturation_Hue:
					if (svZero)   hue = PickerSlider.value;
					UpdateSliders(hue, sat, val);
					return;
				case ColorPickerType.HueValue_Saturation:
					if (svZero)   hue = PickerPlot.value.y;
					if (val == 0) sat = PickerSlider.value;
					UpdateSliders(sat, val, hue);
					return;
				case ColorPickerType.HueSaturation_Value:
					if (svZero)   hue = PickerPlot.value.y;
					if (val == 0) sat = PickerPlot.value.x;
					UpdateSliders(val, sat, hue);
					return;
			}
		}

		void UpdateSliders(float sliderVal, float plotValX, float plotValY)
		{
			PickerPlot.SetValueWithoutNotify(new Vector2(plotValX, plotValY));
			PickerPlot.SetMaterialProperty(Mathf.Clamp01(sliderVal));
			PickerSlider.SetValueWithoutNotify(Mathf.Clamp01(sliderVal));
			PickerSlider.SetMaterialProperty(plotValY);
			value = GetHSV();
		}

		void SetPickerType()
		{
			PickerPlot.SetPickerType((int)PickerPlotType, PickerPlotType != ColorPickerType.ValueSaturation_Hue);
			PickerSlider.SetPickerType((int)PickerPlotType);
		}

		void SetEdgesOffset()
		{
			Vector4 offset = new Vector4(-EdgesOffset.x, 2-EdgesOffset.y, 0, 0);
			PickerPlot.SetMaterialOffset(offset);
		}

		private void OnPlotChange(ChangeEvent<Vector2> evt)
		{
			if(IsColorWheel)
				PickerSlider.SetMaterialProperty(evt.newValue.y);
			value = GetHSV();
		}

		private void OnSliderChange(ChangeEvent<float> evt)
		{
			PickerPlot.SetMaterialProperty(evt.newValue);
			value = GetHSV();
		}
		
		public override void SetValueWithoutNotify(Vector3 newValue)
		{
			base.SetValueWithoutNotify(newValue);

			_ColorValue = GetHSVColor();
			NotifyPropertyChanged(colorProperty);
			PickerColorPreview.style.backgroundColor = new StyleColor(ColorValue);
			SliderColorPreview.style.backgroundColor = new StyleColor(ColorValue);
		}

		[Serializable]
		public enum ColorPickerType
		{
			ValueSaturation_Hue = 0,
			HueValue_Saturation = 1,
			HueSaturation_Value = 2,
		}
	}
}
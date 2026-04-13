using System;
using Sirenix.OdinInspector;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	[UxmlElement]
	public partial class NOColorPicker : BaseField<Color>
	{
		private readonly VisualElement InputContainer;
		private readonly NOColorPickerPlot PickerPlot;
		private readonly VisualElement PickerColorPreview;
		private readonly NOColorPickerSlider PickerSlider;
		private readonly VisualElement SliderColorPreview;

		private ColorPickerType _PickerPlotType;
		[UxmlAttribute, CreateProperty]
		private ColorPickerType PickerPlotType
		{
			get => _PickerPlotType;
			set
			{
				if (value == _PickerPlotType) return;
				Vector3 HSV = GetHSV();
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

		private bool InternalSet;
		
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

		Vector3 GetHSV() => PickerPlotType switch
		{
			ColorPickerType.ValueSaturation_Hue => new Vector3(PickerSlider.value, PickerPlot.value.x, PickerPlot.value.y),
			ColorPickerType.HueValue_Saturation => new Vector3(PickerPlot.value.y, PickerSlider.value, PickerPlot.value.x),
			ColorPickerType.HueSaturation_Value => new Vector3(PickerPlot.value.y, PickerPlot.value.x, PickerSlider.value),
			_ => GetHSV(value)
		};

		Vector3 GetHSV(Color color)
		{
			Color.RGBToHSV(color, out var hue, out var sat, out var val);
			switch (PickerPlotType)
			{
				case ColorPickerType.ValueSaturation_Hue:
					if (sat == 0 || val == 0) hue = PickerSlider.value;
					break;
				case ColorPickerType.HueValue_Saturation:
					if (sat == 0 || val == 0) hue = PickerPlot.value.y;
					if (val == 0)             sat = PickerSlider.value;
					break;
				case ColorPickerType.HueSaturation_Value:
					if (sat == 0 || val == 0) hue = PickerPlot.value.y;
					if (val == 0)             sat = PickerPlot.value.x;
					break;
			}
			return new Vector3(hue, sat, val);
		}

		void SetHSV(Vector3 hsv)
		{
			switch (PickerPlotType)
			{
				case ColorPickerType.ValueSaturation_Hue:
					PickerPlot.SetValueWithoutNotify(new Vector2(hsv.y, hsv.z));
					PickerPlot.SetMaterialProperty(hsv.x);
					PickerSlider.SetValueWithoutNotify(hsv.x);
					break;
				case ColorPickerType.HueValue_Saturation:
					PickerPlot.SetValueWithoutNotify(new Vector2(hsv.z, hsv.x));
					PickerPlot.SetMaterialProperty(hsv.y);
					PickerSlider.SetValueWithoutNotify(hsv.y);
					PickerSlider.SetMaterialProperty(hsv.x);
					break;
				case ColorPickerType.HueSaturation_Value:
					PickerPlot.SetValueWithoutNotify(new Vector2(hsv.y, hsv.x));
					PickerPlot.SetMaterialProperty(hsv.z);
					PickerSlider.SetValueWithoutNotify(hsv.z);
					PickerSlider.SetMaterialProperty(hsv.x);
					break;
			}
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
			OnValueChanged();
		}

		private void OnSliderChange(ChangeEvent<float> evt)
		{
			PickerPlot.SetMaterialProperty(evt.newValue);
			OnValueChanged();
		}
		
		private void OnValueChanged()
		{
			InternalSet = true;
			Vector3 hsv = GetHSV();
			value = Color.HSVToRGB(hsv.x, hsv.y, hsv.z);
			InternalSet = false;
		}
		public override void SetValueWithoutNotify(Color newValue)
		{
			base.SetValueWithoutNotify(newValue);

			if(!InternalSet) SetHSV(GetHSV(value));
			PickerColorPreview.style.backgroundColor = new StyleColor(value);
			SliderColorPreview.style.backgroundColor = new StyleColor(value);
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
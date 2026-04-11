using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	[UxmlElement]
	public partial class NOColorPicker : BaseField<Color>
	{
		private readonly VisualElement InputContainer;
		
		[UxmlAttribute] public float Saturation { get; set; }
		[UxmlAttribute] public float Value { get; set; }

		public NOColorPicker() : this(string.Empty) { }
		public NOColorPicker(string label) : base(label, new VisualElement())
		{
			var styleSheet = NOUSS.GetStyleSheet(NOUSS.ColorPickerStylePath);
			if(styleSheet) styleSheets.Add(styleSheet);

			AddToClassList(NOUSS.ColorPickerClass);
			labelElement.AddToClassList(NOUSS.ColorPickerLabelClass);

			InputContainer = this.Q(className: inputUssClassName);
			InputContainer.AddToClassList(NOUSS.ColorPickerInputContainerClass);
		}
	}
}
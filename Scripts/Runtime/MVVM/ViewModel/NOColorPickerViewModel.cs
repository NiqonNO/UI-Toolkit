using NiqonNO.UI.View;
using UnityEngine;

namespace NiqonNO.UI.MVVM
{
	public class NOColorPickerViewModel :NOPropertyViewModel<Color>
	{
		[SerializeField] 
		private string Label;
		
		[SerializeField] 
		private NOColorPicker.ColorPickerType ColorPickerType;
	}
}
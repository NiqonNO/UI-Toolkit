using NiqonNO.Core.Editor.Drawers;
using UnityEditor;
using UnityEngine;

namespace NiqonNO.UI.Editor.Drawers.PropertyDrawers
{
	public class NOColorSliderShaderGUI : NOShaderGUI
	{
		MaterialProperty GradientType;
		MaterialProperty HueValue;
		
		string[] GradientTypes = 
		{
			"Hue", 
			"Saturation", 
			"Lightness"
		};
		string[] Keywords = 
		{
			"_COLOR_PICKER_TYPE_LS_H", 
			"_COLOR_PICKER_TYPE_HL_S", 
			"_COLOR_PICKER_TYPE_HS_L"
		};
		
		protected override void FindProperties()
		{
			GradientType = FindProperty("_COLOR_PICKER_TYPE");
			HueValue = FindProperty("_Hue");
		}

		protected override void DrawProperties()
		{
			EditorGUI.BeginChangeCheck();
			int picker = (int)GradientType.floatValue;
			picker = EditorGUILayout.Popup("Gradient Type", picker, GradientTypes);
			if (EditorGUI.EndChangeCheck())
			{
				GradientType.floatValue = picker;
			}

			if ((int)GradientType.floatValue != 0)
			{
				MaterialEditor.ShaderProperty(HueValue, "Hue");
			}
		}

		protected override void SetKeywords(Material target)
		{
			for (var i = 0; i < Keywords.Length; i++)
			{
				SetKeyword(target, Keywords[i], i == (int)GradientType.floatValue);
			}
		}
	}
}
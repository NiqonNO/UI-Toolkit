using NiqonNO.Core.Editor.Drawers;
using UnityEditor;
using UnityEngine;

namespace NiqonNO.UI.Editor.Drawers.PropertyDrawers
{
	public class NOColorPickerShaderGUI : NOShaderGUI
	{
		MaterialProperty PickerType;
		MaterialProperty EdgesOffset;
		MaterialProperty ExternalValue;

		string[] PickerTypes = 
		{
			"Lightness - Saturation", 
			"Hue - Lightness", 
			"Hue - Saturation"
		};
		
		string[] Keywords = 
		{
			"_COLOR_PICKER_TYPE_LS_H", 
			"_COLOR_PICKER_TYPE_HL_S", 
			"_COLOR_PICKER_TYPE_HS_L"
		};
		
		protected override void FindProperties()
		{
			PickerType = FindProperty("_COLOR_PICKER_TYPE");
			EdgesOffset = FindProperty("_Wheel_Edges_Offset");
			ExternalValue = FindProperty("_External");
		}

		protected override void DrawProperties()
		{
			EditorGUI.BeginChangeCheck();
			int picker = (int)PickerType.floatValue;
			picker = EditorGUILayout.Popup("Picker Type", picker, PickerTypes);
			if (EditorGUI.EndChangeCheck())
			{
				PickerType.floatValue = picker;
			}

			if ((int)PickerType.floatValue != 0)
			{				
				Vector4 v = EdgesOffset.vectorValue;

				EditorGUI.BeginChangeCheck();
				float center = -v.x;
				float edge = 2-v.y;
				MinMaxSliderWithFields(new GUIContent("Edge Offset"), ref center, ref edge, 0, 1);
				if (EditorGUI.EndChangeCheck())
				{
					EdgesOffset.vectorValue = new Vector4(-center, 2-edge, v.z, v.w);
				}
			}

			string label = "Value";
			switch ((int)PickerType.floatValue)
			{
				case 0: label = "Hue"; break;
				case 1: label = "Saturation"; break;
				case 2: label = "Lightness"; break;
			}

			MaterialEditor.ShaderProperty(ExternalValue, label);
		}

		protected override void SetKeywords(Material target)
		{
			for (var i = 0; i < Keywords.Length; i++)
			{
				SetKeyword(target, Keywords[i], i == (int)PickerType.floatValue);
			}
		}
	}
}
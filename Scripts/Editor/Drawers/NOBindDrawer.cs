using System;
using NiqonNO.UI.MVVM;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace NiqonNO.UI.Editor.Drawers.PropertyDrawers
{
	public class NOBindDrawer : OdinValueDrawer<NOBind>
	{
		InspectorProperty BindByProperty;
		InspectorProperty ElementNameProperty;
		InspectorProperty ClassNameProperty;
		
		protected override void Initialize()
		{
			BindByProperty = Property.Children["BindBy"];
			ElementNameProperty = Property.Children["BindName"];
			ClassNameProperty = Property.Children["BindClass"];
		}
		
		protected override void DrawPropertyLayout(GUIContent label)
		{
			SirenixEditorGUI.BeginBox();
			var bindBy = (NOBind.BindType)BindByProperty.ValueEntry.WeakSmartValue;

			BindByProperty.Draw(label ?? GUIContent.none);
			if (bindBy == NOBind.BindType.None)
			{
				SirenixEditorGUI.EndBox();
				return;
			}

			bool hasName = bindBy.HasFlag(NOBind.BindType.Name);
			bool hasClass = bindBy.HasFlag(NOBind.BindType.Class);
			bool both = hasName && hasClass;
			
			if (both)
				SirenixEditorGUI.BeginHorizontalPropertyLayout(GUIContent.none);

			if (hasName)
				ElementNameProperty.Draw(GUIContent.none);

			if (hasClass)
				ClassNameProperty.Draw(GUIContent.none);

			if (both)
				SirenixEditorGUI.EndHorizontalPropertyLayout();
			SirenixEditorGUI.EndBox();
		}
	}
}
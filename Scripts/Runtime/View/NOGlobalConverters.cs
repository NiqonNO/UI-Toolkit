using UnityEditor;
using UnityEngine.UIElements;

namespace NiqonNO.UI.View
{
	[InitializeOnLoad]
	public static class NOGlobalConverters
	{
		static NOGlobalConverters()
		{
			ConverterGroup BoolToDisplay = new("Bool To Display");
			BoolToDisplay.AddConverter((ref bool value) => new StyleEnum<DisplayStyle>(value ? DisplayStyle.Flex : DisplayStyle.None));
			ConverterGroups.RegisterConverterGroup(BoolToDisplay);

			ConverterGroup InverseBoolToDisplay = new("Inverse Bool To Display");
			InverseBoolToDisplay.AddConverter((ref bool value) => new StyleEnum<DisplayStyle>(value ? DisplayStyle.None : DisplayStyle.Flex));
			ConverterGroups.RegisterConverterGroup(InverseBoolToDisplay);
		}
	}
}
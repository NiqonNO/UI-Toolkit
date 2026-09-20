using UnityEngine.UIElements;

namespace NiqonNO.UI.View
{
	public static class NOGlobalConverters
	{
#if UNITY_EDITOR
		[UnityEditor.InitializeOnLoadMethod]
#else
		[UnityEngine.RuntimeInitializeOnLoadMethod]
#endif
		static void InitializeConverters()
		{
			ConverterGroup InverseBool = new("Inverse Bool");
			InverseBool.AddConverter((ref bool value) =>!value);
			ConverterGroups.RegisterConverterGroup(InverseBool);
			
			ConverterGroup BoolToDisplay = new("Bool To Display");
			BoolToDisplay.AddConverter((ref bool value) => new StyleEnum<DisplayStyle>(value ? DisplayStyle.Flex : DisplayStyle.None));
			ConverterGroups.RegisterConverterGroup(BoolToDisplay);

			ConverterGroup InverseBoolToDisplay = new("Inverse Bool To Display");
			InverseBoolToDisplay.AddConverter((ref bool value) => new StyleEnum<DisplayStyle>(value ? DisplayStyle.None : DisplayStyle.Flex));
			ConverterGroups.RegisterConverterGroup(InverseBoolToDisplay);
			
			ConverterGroup BoolToPicking = new("Bool To Picking");
			BoolToPicking.AddConverter((ref bool value) => value ? PickingMode.Position : PickingMode.Ignore);
			ConverterGroups.RegisterConverterGroup(BoolToPicking);

			ConverterGroup InverseBoolToPicking = new("Inverse Bool To Picking");
			InverseBoolToPicking.AddConverter((ref bool value) => value ? PickingMode.Ignore : PickingMode.Position);
			ConverterGroups.RegisterConverterGroup(InverseBoolToPicking);
		}
	}
}
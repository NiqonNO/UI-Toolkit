using UnityEditor.UIElements;

namespace NiqonNO.UI.Converters
{
	public class NOViewModelListStateConverter : UxmlAttributeConverter<NOViewModelListState>
	{
		public override NOViewModelListState FromString(string value)
		{
			return null;
		}

		public override string ToString(NOViewModelListState value)
		{
			return string.Empty;
		}
	}
}
using UnityEditor.UIElements;

namespace NiqonNO.UI.Editor
{
	public class NOViewModelListStateConverter : UxmlAttributeConverter<INOViewModelListState>
	{
		public override INOViewModelListState FromString(string value)
		{
			return null;
		}

		public override string ToString(INOViewModelListState value)
		{
			return string.Empty;
		}
	}
}
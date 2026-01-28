using UnityEditor.UIElements;

namespace NiqonNO.UI.Editor.Converters
{
	public class NOCollectionStateConverter : UxmlAttributeConverter<NOCollectionState>
	{
		public override NOCollectionState FromString(string value)
		{
			return new NOCollectionState();
		}

		public override string ToString(NOCollectionState value)
		{
			return string.Empty;
		}
	}
}
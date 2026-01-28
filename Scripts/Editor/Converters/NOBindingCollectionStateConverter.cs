using UnityEditor.UIElements;

namespace NiqonNO.UI.Editor.Converters
{
	public class NOBindingCollectionStateConverter : UxmlAttributeConverter<NOBindingCollectionState>
	{
		public override NOBindingCollectionState FromString(string value)
		{
			return new NOBindingCollectionState();
		}

		public override string ToString(NOBindingCollectionState value)
		{
			return string.Empty;
		}
	}
}
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace NiqonNO.UI.MVVM
{
	public interface INOBindingCategory
	{
		string CategoryName { get; }
		IReadOnlyList<INOBindingContext> ItemsCollection { get; }
		VisualTreeAsset ItemTemplate { get; }
	}
}
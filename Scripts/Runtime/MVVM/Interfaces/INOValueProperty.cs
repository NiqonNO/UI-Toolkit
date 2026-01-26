using System;

namespace NiqonNO.UI.MVVM
{
	public interface INOValueProperty<T>
	{
		T Value { get; }
		event Action<T> ValueChanged;
	}
}
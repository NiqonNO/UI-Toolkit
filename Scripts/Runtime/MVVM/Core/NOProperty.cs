using System;
using System.Collections.Generic;

namespace NiqonNO.UI.MVVM
{
	public sealed class NOProperty<T> : INOVariableProperty<T>
	{
		T _Value;
		public event Action<T> ValueChanged;

		public T Value
		{
			get => _Value;
			set
			{
				if (EqualityComparer<T>.Default.Equals(_Value, value)) return;
				_Value = value;
				ValueChanged?.Invoke(value);
			}
		}

		public NOProperty(T initial = default) => _Value = initial;
	}
}
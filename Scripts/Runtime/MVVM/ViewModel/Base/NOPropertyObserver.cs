using System;
using System.Collections.Generic;

namespace NiqonNO.UI.MVVM
{
	public class NOPropertyObserver<T>
	{
		Action<T> OnValueChanged;
		T LocalValue;

		public NOPropertyObserver(T value, Action<T> onValueChanged = null)
		{
			LocalValue = value;
			OnValueChanged = onValueChanged;
		}

		public T Validate(T newValue)
		{
			if (EqualityComparer<T>.Default.Equals(LocalValue, newValue)) return LocalValue;
			
			LocalValue = newValue;
			OnValueChanged?.Invoke(LocalValue);
			return LocalValue;
		}
	}
}
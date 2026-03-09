using System;
using System.Collections.Generic;
using System.Linq;

namespace NiqonNO.UI.MVVM
{
	public class NOCollectionObserver<TSource, TReturn> where TSource : TReturn
	{
		Action<List<TSource>> OnValueChanged;

		List<TSource> SourceValue;
		IReadOnlyList<TReturn> ReturnValue;

		public NOCollectionObserver(List<TSource> value, Action<List<TSource>> onValueChanged = null)
		{
			SourceValue = value;
			ReturnValue = value.Cast<TReturn>().ToList();
			OnValueChanged = onValueChanged;
		}

		public IReadOnlyList<TReturn> Validate(List<TSource> newValue)
		{
			if (ReturnValue.SequenceEqual((IEnumerable<TReturn>)newValue)) return ReturnValue;

			SourceValue = newValue;
			ReturnValue = newValue.Cast<TReturn>().ToList();
			OnValueChanged?.Invoke(SourceValue);
			return ReturnValue;
		}

		public List<TSource> Validate(IReadOnlyList<TReturn> newValue)
		{
			if (SourceValue.SequenceEqual((IEnumerable<TSource>)newValue)) return SourceValue;

			ReturnValue = newValue;
			SourceValue = newValue.Cast<TSource>().ToList();
			OnValueChanged?.Invoke(SourceValue);
			return SourceValue;
		}
	}
}
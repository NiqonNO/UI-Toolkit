using System;

namespace NiqonNO.UI.MVVM
{
	public interface INOBindingData
	{
		public string BindTarget { get; }
	}

	public interface INOBindingData<T> : INOBindingData
	{
		public T Data { get; set; }
	}
}
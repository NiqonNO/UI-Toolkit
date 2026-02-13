using System;

namespace NiqonNO.UI.MVVM
{
	public interface INOBindingData
	{
		public NOBind BindTarget { get; }
		public NOCallback[] Callbacks { get;  }
	}

	public interface INOBindingData<T> : INOBindingData
	{
		public T Data { get; set; }
	}
}
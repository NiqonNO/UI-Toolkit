using NiqonNO.UI.Callbacks;

namespace NiqonNO.UI.MVVM
{
	public interface INOBindingData
	{
		public NOBind BindTarget { get; }
		public NOCallbackHandler[] Callbacks { get;  }
	}

	public interface INOBindingData<T> : INOBindingData
	{
		public T Data { get; set; }
	}
}
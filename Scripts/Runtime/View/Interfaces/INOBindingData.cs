using UnityEngine.UIElements;

namespace NiqonNO.UI.View
{
	public interface INOBindingData<T>
	{
		public T Data { get; }
	}
	
	public interface INOBindingData
	{
		void Bind(VisualElement context);
		void Unbind();
	}
}
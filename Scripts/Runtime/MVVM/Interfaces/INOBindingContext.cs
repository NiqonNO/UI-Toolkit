using UnityEngine.UIElements;

namespace NiqonNO.UI.MVVM
{
	public interface INOBindingContext
	{
		void Bind(VisualElement context);
		void Unbind();
	}
}
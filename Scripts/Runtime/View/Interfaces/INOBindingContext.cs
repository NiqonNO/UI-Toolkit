using UnityEngine.UIElements;

namespace NiqonNO.UI.View
{
	public interface INOBindingContext
	{
		void Bind(VisualElement context);
		void Unbind();
	}
}
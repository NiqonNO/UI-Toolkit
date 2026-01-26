using UnityEngine.UIElements;

namespace NiqonNO.UI.MVVM
{
	public interface INOBindingContext
	{
		void Bind(UIDocument context);
		void Unbind();
	}
}
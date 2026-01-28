using System;
using UnityEngine.UIElements;

namespace NiqonNO.UI.MVVM
{
	public abstract class NOViewModel : INOBindingContext, IDisposable
	{
		public abstract void Bind(VisualElement context);
		public abstract void Unbind();
		public virtual void Dispose() {}
	}
}
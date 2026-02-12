using System;
using UnityEngine.UIElements;

namespace NiqonNO.UI.MVVM
{
	public abstract class NOViewModel : INOBindingContext, IDisposable
	{
		public abstract void Bind(VisualElement context);
		//public abstract void Unbind();
		public virtual void Dispose() {}
	}
	public abstract class NOViewModel<T> : NOViewModel where T : class, INOBindingData
	{
		protected T DataProvider { get; private set; }

		protected NOViewModel(T dataProvider)
		{
			DataProvider = dataProvider;
		}  
		
		public virtual void UpdateDataProvider(T dataProvider)
		{
			DataProvider = dataProvider;
		}

		public override void Bind(VisualElement context)
		{
			context.Q(DataProvider.BindTarget).dataSource = this;
		}
	}
}
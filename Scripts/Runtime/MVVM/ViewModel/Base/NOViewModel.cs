using UnityEngine.UIElements;

namespace NiqonNO.UI.MVVM
{
	public abstract class NOViewModel : INOBindingContext
	{
		public abstract void Bind(VisualElement context);
		public abstract void Unbind();
	}
	public abstract class NOViewModel<T> : NOViewModel where T : class, INOBindingData
	{
		protected VisualElement BindTarget { get; private set; }
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
			BindTarget = DataProvider.BindTarget.QueryTarget(context);
			if (BindTarget == null) return;
			BindTarget.dataSource = this;
			RegisterCallback();
		}
		
		private void RegisterCallback()
		{
			if (BindTarget == null) return;
			if (DataProvider.Callbacks.Length == 0) return;

			foreach (var callbackBind in DataProvider.Callbacks)
			{
				callbackBind.RegisterCallback(BindTarget);
			}
		}
		
		public override void Unbind()
		{
			if (BindTarget == null) return;
			BindTarget.dataSource = null;
			BindTarget = null;
			
			if (DataProvider.Callbacks.Length == 0) return;
			foreach (var callbackBind in DataProvider.Callbacks)
			{
				callbackBind.UnregisterCallback();
			}
		}
	}
}
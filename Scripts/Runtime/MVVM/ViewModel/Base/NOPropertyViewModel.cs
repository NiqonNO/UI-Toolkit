using System;
using Unity.Properties;

namespace NiqonNO.UI.MVVM
{
	public class NOPropertyViewModel<T> : NOViewModel<T>
	{
		private Func<T> Get;
		private Action<T> Set;

		[CreateProperty]
		protected T Value
		{
			get => Get != null ? Get() : default;
			set => Set?.Invoke(value);
		}

		protected override void OnSourceReady()
		{
			if(SourceProperty.CanRead)
				Get = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), SourceInstance, SourceProperty.GetMethod);
			if(SourceProperty.CanWrite)
				Set = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), SourceInstance, SourceProperty.SetMethod);
		}
	}
}
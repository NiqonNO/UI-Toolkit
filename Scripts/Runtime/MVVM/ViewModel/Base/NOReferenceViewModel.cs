namespace NiqonNO.UI.MVVM
{
	public class NOReferenceViewModel<T> : NOViewModel<T> where T : class
	{
		protected T Value { get; private set; }

		protected override void OnSourceReady()
		{
			Value = SourceProperty.GetValue(SourceInstance) as T;
		}
	}
}
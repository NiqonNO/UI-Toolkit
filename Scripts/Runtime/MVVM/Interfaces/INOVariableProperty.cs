namespace NiqonNO.UI.MVVM
{
	public interface INOVariableProperty<T> : INOValueProperty<T>
	{
		new T Value { get; set; }
	}
}
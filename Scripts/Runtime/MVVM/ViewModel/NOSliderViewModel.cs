using Unity.Properties;

namespace NiqonNO.UI.MVVM
{
	public class NOSliderViewModel : NOViewModel<NOSliderData>
	{		
		[CreateProperty]
		private float Value
		{
			get => DataProvider.Data.Value;
			set => DataProvider.Data.Value = value;
		}
		public NOSliderViewModel(NOSliderData dataProvider) : base(dataProvider) { }
	}
}
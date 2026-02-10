using Unity.Properties;
using UnityEngine;

namespace NiqonNO.UI.MVVM
{
	public class NOTernaryViewModel : NOViewModel<NOTernaryData>
	{
		[CreateProperty]
		private Vector3 Value
		{
			get => DataProvider.Data.Value;
			set => DataProvider.Data.Value = value;
		}
		
		public NOTernaryViewModel(NOTernaryData data) : base(data) { }
	}
}
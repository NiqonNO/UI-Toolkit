using System.Collections.Generic;

namespace NiqonNO.UI.MVVM
{
	public interface INOBindingDataCollection<T> : INOBindingData
	{
		public List<T> Data { get; set; }
		int SelectedIndex { get; set; }
	}
}
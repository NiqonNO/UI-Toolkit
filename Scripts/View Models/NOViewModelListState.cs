using System;
using System.Collections.Generic;

namespace NiqonNO.UI
{
	[Serializable]
	public class NOViewModelListState
	{
		public List<INOViewModel> Items = new();
		public int SelectedIndex;

		public INOViewModel Selected => Items[SelectedIndex];
	}
}
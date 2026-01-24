using System;
using System.Collections.Generic;

namespace NiqonNO.UI
{
	public interface INOViewModelListState
	{
		public List<INOViewModel> Items { get; }
	
		public int SelectedIndex { get; set;  }

		public INOViewModel Selected => Items[SelectedIndex];
	}
}
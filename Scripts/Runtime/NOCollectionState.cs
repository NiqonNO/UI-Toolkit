using System;
using System.Collections.Generic;
using System.Linq;
using NiqonNO.UI.MVVM;

namespace NiqonNO.UI
{
	[Serializable]
	public struct NOCollectionState
	{
		public List<INOBindingContext> DataSource;
		public int SelectedItem;
		private INOBindingContext SelectedElement => DataSource[SelectedItem];
		
		public static NOCollectionState Create<T>(List<T> dataSource, int selected = 0) where T : INOBindingContext
		{
			return new NOCollectionState()
			{
				DataSource = dataSource.Cast<INOBindingContext>().ToList(),
				SelectedItem = selected,
			};
		}
	}
}
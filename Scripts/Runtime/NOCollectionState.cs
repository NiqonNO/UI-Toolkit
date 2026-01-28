using System;
using System.Collections.Generic;
using System.Linq;
using NiqonNO.UI.MVVM;

namespace NiqonNO.UI
{
	[Serializable]
	public struct NOCollectionState : 
		IEquatable<NOCollectionState>,
		IComparable<NOCollectionState>
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
		
		public int CompareTo(NOCollectionState other) => other == this ? 1 : -1;
		public static bool operator ==(NOCollectionState lftHnd, NOCollectionState rgtHnd) =>  lftHnd.DataSource == rgtHnd.DataSource;
		public static bool operator !=(NOCollectionState lftHnd, NOCollectionState rgtHnd) => lftHnd.DataSource != rgtHnd.DataSource;
		public bool Equals(NOCollectionState other) => DataSource == other.DataSource;
		public override bool Equals(object other) => other is NOCollectionState other1 && Equals(other1);
		public override int GetHashCode() => HashCode.Combine(DataSource, SelectedItem);
	}
}
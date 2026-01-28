using System;
using System.Collections.Generic;
using NiqonNO.UI.MVVM;

namespace NiqonNO.UI
{
	[Serializable]
	public struct NOBindingCollectionState : 
		IEquatable<NOBindingCollectionState>,
		IComparable<NOBindingCollectionState>
	{
		public IList<INOBindingContext> DataSource;
		public int SelectedItem;
		private INOBindingContext SelectedElement => DataSource[SelectedItem];
		
		public int CompareTo(NOBindingCollectionState other) => other == this ? 1 : -1;
		public static bool operator ==(NOBindingCollectionState lftHnd, NOBindingCollectionState rgtHnd) =>  Equals(lftHnd.DataSource, rgtHnd.DataSource);
		public static bool operator !=(NOBindingCollectionState lftHnd, NOBindingCollectionState rgtHnd) => !Equals(lftHnd.DataSource, rgtHnd.DataSource);
		public bool Equals(NOBindingCollectionState other) => Equals(DataSource, other.DataSource);
		public override bool Equals(object other) => other is NOBindingCollectionState other1 && Equals(other1);
		public override int GetHashCode() => HashCode.Combine(DataSource, SelectedItem);
	}
}
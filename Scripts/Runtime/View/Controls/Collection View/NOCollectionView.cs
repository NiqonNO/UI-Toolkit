using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI.View
{
	[UxmlElement]
	public abstract partial class NOCollectionView<T> : BindableElement
	{
		private static readonly BindingId SourceCollectionProperty = (BindingId) nameof (SourceCollection);
		private static readonly BindingId SelectedIndexProperty = (BindingId) nameof (SelectedIndex);
		
		private IReadOnlyList<T> _SourceCollection;
		[UxmlObjectReference][CreateProperty] 
		public IReadOnlyList<T> SourceCollection
		{
			get => _SourceCollection;
			set
			{
				if (ReferenceEquals(_SourceCollection, value))
					return;
				
				_SourceCollection = value;
				
				SelectedIndex = SelectedIndex;
				Refresh();
				NotifyPropertyChanged(SourceCollectionProperty);
			}
		}

		private int _SelectedIndex;
		[UxmlAttribute][CreateProperty]
		protected int SelectedIndex
		{
			get => _SelectedIndex;
			private set => SetIndex(value, true);
		}

		protected void SetIndex(int newIdx, bool refresh = false)
		{
			int circular = CircularIndex(newIdx);
			if (_SelectedIndex == circular)
				return;

			_SelectedIndex = circular;
			NotifyPropertyChanged(SelectedIndexProperty);
			if(refresh)
				Refresh();
		}

		public int CollectionLength => _SourceCollection?.Count ?? 0;

		protected abstract void Refresh();
		
		int CircularIndex(int idx) => CollectionLength == 0 ? 0 : (int)Mathf.Repeat(idx, CollectionLength);

		protected T GetItem(int index)
		{
			if (CollectionLength == 0)
				return default;

			if (index < 0 || index >= CollectionLength)
				index = CircularIndex(index);

			return _SourceCollection[index];
		}
	}
}
using System.Collections;
using System.Collections.Specialized;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	[UxmlElement]
	public abstract partial class NOCollectionView : BindableElement
	{
		internal static readonly BindingId ItemsSourceProperty = new (nameof(ItemsSource));
		internal static readonly BindingId SelectedIndexProperty = new (nameof(SelectedIndex));

		INotifyCollectionChanged CollectionNotifier;
		
		IList _ItemsSource;
		[CreateProperty]
		public IList ItemsSource
		{
			get => _ItemsSource;
			set
			{
				if (ReferenceEquals(_ItemsSource, value))
					return;

				UnsubscribeCollection();
				_ItemsSource = value;
				SubscribeCollection();

				ClampSelection();
				NotifyPropertyChanged(in ItemsSourceProperty);
				OnItemsSourceChanged();
			}
		}
		
		int _SelectedIndex = -1;
		[CreateProperty]
		public int SelectedIndex
		{
			get => _SelectedIndex;
			set
			{
				if (_SelectedIndex == value)
					return;

				_SelectedIndex = value;
				
				ClampSelection();
				NotifyPropertyChanged(in SelectedIndexProperty);
				OnSelectionChanged();
			}
		}
		
		public int CollectionLength => _ItemsSource?.Count ?? 0;
		

		void SubscribeCollection()
		{
			CollectionNotifier = _ItemsSource as INotifyCollectionChanged;
			if (CollectionNotifier != null)
				CollectionNotifier.CollectionChanged += OnCollectionChanged;
		}
		void UnsubscribeCollection()
		{
			if (CollectionNotifier != null)
				CollectionNotifier.CollectionChanged -= OnCollectionChanged;
			CollectionNotifier = null;
		}

		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			ClampSelection();
			OnItemsSourceChanged();
		}

		void ClampSelection()
		{
			if (CollectionLength== 0)
			{
				_SelectedIndex = -1;
				return;
			}

			_SelectedIndex = (int)Mathf.Repeat(_SelectedIndex, CollectionLength);
		}
		
		protected virtual void OnItemsSourceChanged() {}
		protected virtual void OnSelectionChanged() {}

		protected object GetItem(int index)
		{
			if (_ItemsSource == null)
				return null;

			if (index < 0 || index >= CollectionLength)
				index = (int)Mathf.Repeat(index, CollectionLength);

			return _ItemsSource[index];
		}
	}
}
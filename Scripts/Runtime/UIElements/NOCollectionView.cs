using System.Collections.Generic;
using System.Collections.Specialized;
using NiqonNO.UI.MVVM;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	[UxmlElement]
	public abstract partial class NOCollectionView : BindableElement
	{
		internal static readonly BindingId CollectionStateProperty = new (nameof(CollectionState));
		
		INotifyCollectionChanged CollectionNotifier;

		[UxmlAttribute][CreateProperty]
		private NOCollectionState CollectionState;

		protected List<INOBindingContext> DataSource
		{
			get => CollectionState.DataSource;
			set
			{
				if (/*CollectionState == null || */ReferenceEquals(CollectionState.DataSource, value))
					return;

				UnsubscribeCollection();
				CollectionState.DataSource = value;
				SubscribeCollection();

				ClampSelection();
				NotifyPropertyChanged(in CollectionStateProperty);
				OnItemsSourceChanged();
			}
		}
		
		protected int SelectedIndex
		{
			get => CollectionState.SelectedItem;
			set
			{
				if (/*CollectionState == null || */CollectionState.SelectedItem == value)
					return;

				CollectionState.SelectedItem = value;
				
				ClampSelection();
				NotifyPropertyChanged(in CollectionStateProperty);
				OnSelectionChanged();
			}
		}
		
		public int CollectionLength => CollectionState.DataSource?.Count ?? 0;
		

		void SubscribeCollection()
		{
			CollectionNotifier = CollectionState.DataSource as INotifyCollectionChanged;
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
				CollectionState.SelectedItem = -1;
				return;
			}

			CollectionState.SelectedItem = (int)Mathf.Repeat(CollectionState.SelectedItem, CollectionLength);
		}
		
		protected virtual void OnItemsSourceChanged() {}
		protected virtual void OnSelectionChanged() {}

		protected INOBindingContext GetItem(int index)
		{
			if (CollectionState.DataSource == null)
				return null;

			if (index < 0 || index >= CollectionLength)
				index = (int)Mathf.Repeat(index, CollectionLength);

			return CollectionState.DataSource[index];
		}
	}
}
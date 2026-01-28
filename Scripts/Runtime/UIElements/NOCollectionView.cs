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

		[UxmlAttribute]
		private NOBindingCollectionState _CollectionState;

		[CreateProperty]
		protected NOBindingCollectionState CollectionState
		{
			get => _CollectionState;
			private set
			{
				if (_CollectionState == value)
					return;
				
				_CollectionState = value;
				NotifyPropertyChanged(in CollectionStateProperty);
				OnItemsSourceChanged();
			}
		}
		
		protected IList<INOBindingContext> DataSource
		{
			get => _CollectionState.DataSource;
			set
			{
				if (ReferenceEquals(_CollectionState.DataSource, value))
					return;

				UnsubscribeCollection();
				_CollectionState.DataSource = value;
				SubscribeCollection();

				ClampSelection();
				NotifyPropertyChanged(in CollectionStateProperty);
				OnItemsSourceChanged();
			}
		}
		
		protected int SelectedIndex
		{
			get => _CollectionState.SelectedItem;
			set
			{
				if (_CollectionState.SelectedItem == value)
					return;

				_CollectionState.SelectedItem = value;
				
				ClampSelection();
				NotifyPropertyChanged(in CollectionStateProperty);
				OnSelectionChanged();
			}
		}
		
		public int CollectionLength => DataSource?.Count ?? 0;
		

		void SubscribeCollection()
		{
			CollectionNotifier = _CollectionState.DataSource as INotifyCollectionChanged;
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
				SelectedIndex = -1;
				return;
			}

			SelectedIndex = (int)Mathf.Repeat(SelectedIndex, CollectionLength);
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
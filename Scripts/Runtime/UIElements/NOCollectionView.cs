using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public abstract class NOCollectionView<T> : BindableElement
	{
		internal static readonly BindingId SourceCollectionProperty = (BindingId) nameof (SourceCollection);
		internal static readonly BindingId SelectedIndexProperty = (BindingId) nameof (SelectedIndex);
		
		protected List<T> _SourceCollection;
		[CreateProperty]
		protected List<T> SourceCollection
		{
			get => _SourceCollection;
			set
			{
				if (ReferenceEquals(_SourceCollection, value))
					return;
				
				_SourceCollection = value;
				
				SelectedIndex = CircularIndex(_SelectedIndex);
				OnItemsSourceChanged();
				NotifyPropertyChanged(SourceCollectionProperty);
			}
		}

		protected int _SelectedIndex;
		[CreateProperty]
		protected int SelectedIndex
		{
			get => _SelectedIndex;
			set
			{
				if (_SelectedIndex == value)
					return;

				_SelectedIndex = CircularIndex(value);
				OnSelectionChanged();
				NotifyPropertyChanged(SelectedIndexProperty);
			}
		}
		
		public int CollectionLength => _SourceCollection?.Count ?? 0;

		int CircularIndex(int idx)
		{
			if (CollectionLength == 0)
				return -1;

			return (int)Mathf.Repeat(idx, CollectionLength);
		}
		
		protected virtual void OnItemsSourceChanged() {}
		protected virtual void OnSelectionChanged() {}

		protected T GetItem(int index)
		{
			if (_SourceCollection == null)
				return default;

			if (index < 0 || index >= CollectionLength)
				index = (int)Mathf.Repeat(index, CollectionLength);

			return _SourceCollection[index];
		}
		
		[Serializable]
		public new abstract class UxmlSerializedData : BindableElement.UxmlSerializedData
		{
#pragma warning disable 649
			[SerializeField] [HideInInspector] [UxmlIgnore]
			private UxmlAttributeFlags SourceCollection_UxmlAttributeFlags;
			[SerializeReference] private List<T> SourceCollection;
			
			[SerializeField] [HideInInspector] [UxmlIgnore]
			private UxmlAttributeFlags SelectedIndex_UxmlAttributeFlags;
			[SerializeField] private int SelectedIndex;
#pragma warning restore 649
			
			[System.Diagnostics.Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BindableElement.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof (UxmlSerializedData), 
					new UxmlAttributeNames[]
					{
						new("SourceCollection", "source-collection", null, Array.Empty<string>()),
						new("SelectedIndex", "selected-index", null, Array.Empty<string>())
					});
			}
			
			//public override object CreateInstance() => (object) new NOCollectionView<T>();

			public override void Deserialize(object obj)
			{
				NOCollectionView<T> view = (NOCollectionView<T>)obj;
				if (ShouldWriteAttributeValue(SourceCollection_UxmlAttributeFlags))
					view.SourceCollection = SourceCollection;
				if (ShouldWriteAttributeValue(SelectedIndex_UxmlAttributeFlags))
					view.SelectedIndex = SelectedIndex;
				base.Deserialize(obj);
			}
		}
	}
}
using System;
using System.Collections.Generic;
using NiqonNO.UI.MVVM;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NiqonNO.UI
{
	public abstract class NOCollectionView : BindableElement
	{
		internal static readonly BindingId SourceCollectionProperty = (BindingId) nameof (SourceCollection);
		internal static readonly BindingId SelectedIndexProperty = (BindingId) nameof (SelectedIndex);
		
		protected List<Object> _SourceCollection;
		[CreateProperty]
		protected List<Object> SourceCollection
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

		protected INOBindingContext GetItem(int index)
		{
			if (_SourceCollection == null)
				return null;

			if (index < 0 || index >= CollectionLength)
				index = (int)Mathf.Repeat(index, CollectionLength);

			return _SourceCollection[index] as INOBindingContext;
		}
		
		[Serializable]
		public new abstract class UxmlSerializedData : BindableElement.UxmlSerializedData
		{
#pragma warning disable 649
			[SerializeField] [HideInInspector] [UxmlIgnore]
			private UxmlAttributeFlags SourceCollection_UxmlAttributeFlags;
			[SerializeReference] private List<Object> SourceCollection;
			
			[SerializeField] [HideInInspector] [UxmlIgnore]
			private UxmlAttributeFlags SelectedIndex_UxmlAttributeFlags;
			[SerializeReference] private int SelectedIndex;
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

			//public override object CreateInstance() => (object) new NOCollectionView();

			public override void Deserialize(object obj)
			{
				
				NOCollectionView view = (NOCollectionView)obj;
				if (ShouldWriteAttributeValue(SourceCollection_UxmlAttributeFlags))
					view.SourceCollection = SourceCollection;
				if (ShouldWriteAttributeValue(SelectedIndex_UxmlAttributeFlags))
					view.SelectedIndex = SelectedIndex;
				base.Deserialize(obj);
			}
		}
	}
}
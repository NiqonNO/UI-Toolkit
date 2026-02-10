using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Properties;

namespace NiqonNO.UI.MVVM
{
	public abstract class NOCollectionViewModel<TProvider, TData>  : NOViewModel<TProvider> where TProvider : class, INOBindingDataCollection<TData>
	{
		public event Action ValueChangedEvent;

		private List<INOBindingContext> _Data;

		[CreateProperty]
		protected List<INOBindingContext> Data
		{
			get
			{
				if (DataProvider.Data.Count != _Data.Count)
					SynchroniseToData();
				return _Data;
			}
			set
			{
				if (_Data == value)
					return;

				_Data = value;
				SynchroniseToSource();
			}
		}

		public NOCollectionViewModel(TProvider dataProvider) : base(dataProvider)
		{
			SynchroniseToData();
		}

		private void SynchroniseToData()
		{
			_Data = DataProvider.Data.Cast<INOBindingContext>().ToList();
			ValueChangedEvent?.Invoke();
		}

		private void SynchroniseToSource()
		{
			DataProvider.Data = _Data.Cast<TData>().ToList();
			ValueChangedEvent?.Invoke();
		}
	}
}
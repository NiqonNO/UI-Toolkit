using System;
using System.Collections.Generic;
using System.Linq;
using NiqonNO.UI.MVVM;

namespace NiqonNO.UI
{
	public class NOBindingContextCollection<T> where T : INOBindingContext
	{
		public event Action ValueChangedEvent;
		
		private List<T> DataSource;
		private List<INOBindingContext> _Data;

		public List<INOBindingContext> Data
		{
			get
			{
				if (DataSource.Count != _Data.Count)
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

		public NOBindingContextCollection(List<T> dataSource)
		{
			DataSource = dataSource;
			SynchroniseToData();
		}

		private void SynchroniseToData()
		{
			_Data = DataSource.Cast<INOBindingContext>().ToList();
			ValueChangedEvent?.Invoke();
		}

		private void SynchroniseToSource()
		{
			DataSource = _Data.Cast<T>().ToList();
			ValueChangedEvent?.Invoke();
		}
	}
}
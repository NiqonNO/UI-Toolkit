using System;
using System.Collections.Generic;
using UnityEngine;

namespace NiqonNO.UI.MVVM
{
	[Serializable]
	public class NOBindingDataCollection<T> : NOBindingData, INOBindingDataCollection<T>
	{
		[field: SerializeField]
		public List<T> Data { get; set; }
		
		[field: SerializeField]
		public int SelectedIndex { get; set; }
	}
}
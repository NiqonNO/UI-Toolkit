using System;
using UnityEngine;

namespace NiqonNO.UI.MVVM
{
	[Serializable]
	public class NOBindingData : INOBindingData
	{
		[field: SerializeField] 
		public NOBind BindTarget { get; private set; }
	}
	
	[Serializable]
	public class NOBindingData<T> : NOBindingData, INOBindingData<T>
	{
		[field: SerializeField] 
		public T Data { get; set; }
	}
}
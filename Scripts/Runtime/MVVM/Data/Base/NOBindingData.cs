using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace NiqonNO.UI.MVVM
{
	[Serializable]
	public class NOBindingData : INOBindingData
	{
		[field: SerializeField] 
		public NOBind BindTarget { get; private set; }
		
		[field: SerializeField, PropertyOrder(float.MaxValue)] 
		public NOCallback[] Callbacks { get; private set; }
	}
	
	[Serializable]
	public class NOBindingData<T> : NOBindingData, INOBindingData<T>
	{
		[field: SerializeField] 
		public T Data { get; set; }
	}
}
using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace NiqonNO.UI.Callbacks
{
	[Serializable]
	public abstract class NOCallback
	{
		public abstract void Register(NOCallbackManipulator target);
		public abstract void Unregister(NOCallbackManipulator target);
	}
	
	[Serializable]
	public abstract class NOCallback<T> : NOCallback where T : EventBase<T>, new()
	{
		[field: SerializeField, PropertyOrder(float.MaxValue)] 
		private UnityEvent Callback { get; set; }

		public override void Register(NOCallbackManipulator target) => target.RegisterCallback(this);
		public override void Unregister(NOCallbackManipulator target) => target.UnregisterCallback(this);
		
		public virtual void Invoke(T evt) => Callback?.Invoke();
	}
}
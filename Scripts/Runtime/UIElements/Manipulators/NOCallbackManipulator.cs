using System.Collections.Generic;
using System.Linq;
using NiqonNO.UI.Callbacks;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOCallbackManipulator : Manipulator
	{
		private readonly HashSet<NOCallback> RegisteredCallbacks = new ();
		protected override void RegisterCallbacksOnTarget() { }

		protected override void UnregisterCallbacksFromTarget()
		{
			while (RegisteredCallbacks.Count > 0)
			{
				RegisteredCallbacks.First().Unregister(this);
			}
		}
		
		public void RegisterCallback<T>(NOCallback<T> callback) where T : EventBase<T>, new()
		{
			target.RegisterCallback<T>(callback.Invoke);
			RegisteredCallbacks.Add(callback);
		}		
		public void UnregisterCallback<T>(NOCallback<T> callback) where T : EventBase<T>, new()
		{
			target.UnregisterCallback<T>(callback.Invoke);
			RegisteredCallbacks.Remove(callback);
		}
	}
}
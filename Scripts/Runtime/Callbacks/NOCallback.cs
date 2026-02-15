using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace NiqonNO.UI.Callbacks
{
	[Serializable]
	public abstract class NOCallback
	{
		[field: SerializeField] 
		protected UnityEvent Callback { get; private set; }

		private Dictionary<VisualElement, IManipulator> ActiveCallbacks = new ();

		public void EnsureCapacity(int count)
		{
			ActiveCallbacks.EnsureCapacity(count);
		}
		
		public void Register(VisualElement target)
		{
			if (ActiveCallbacks.ContainsKey(target)) return;

			var manipulator = CreateManipulator();
			target.AddManipulator(manipulator);
			ActiveCallbacks.Add(target, manipulator);
		}

		public void Unregister(VisualElement target)
		{
			if (!ActiveCallbacks.TryGetValue(target, out var manipulator)) return;
			
			target.RemoveManipulator(manipulator);
			ActiveCallbacks.Remove(target);
		}
		
		protected abstract IManipulator CreateManipulator();
	}
}
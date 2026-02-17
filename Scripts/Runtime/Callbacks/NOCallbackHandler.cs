using System;
using System.Collections.Generic;
using NiqonNO.Core;
using NiqonNO.UI.MVVM;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI.Callbacks
{
	[Serializable]
	public class NOCallbackHandler
	{
		[field: SerializeField] 
		private NOBind BindTarget { get; set; }

		[field: SerializeReference, NOComponentList("Add  Callback")]
		private List<NOCallback> Callbacks = new ();

		private Dictionary<VisualElement, IManipulator> CallbackTargets = new ();

		public void RegisterCallback(VisualElement context)
		{
			if (Callbacks.Count == 0) return;
			if (CallbackTargets is not { Count: 0 }) UnregisterCallback();

			var targets = BindTarget.QueryTargets(context);
			if (targets is null or { Count: 0 }) return;

			foreach (var target in targets)
			{
				NOCallbackManipulator manipulator = new NOCallbackManipulator();
				target.AddManipulator(manipulator);
				foreach (var callback in Callbacks)
				{
					callback.Register(manipulator);
				}
				CallbackTargets.Add(target, manipulator);
			}
		}

		public void UnregisterCallback()
		{
			if (CallbackTargets == null) return;

			foreach (var target in CallbackTargets)
			{
				target.Key.RemoveManipulator(target.Value);
			}

			CallbackTargets.Clear();
		}
	}
}
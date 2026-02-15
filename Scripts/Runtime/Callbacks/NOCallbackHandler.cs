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
		private List<NOCallback> Callbacks;

		private List<VisualElement> CallbackTargets;

		public void RegisterCallback(VisualElement context)
		{
			if (Callbacks.Count == 0) return;
			if (CallbackTargets is not { Count: 0 }) UnregisterCallback();

			CallbackTargets = BindTarget.QueryTargets(context);
			if (CallbackTargets is null or { Count: 0 }) return;

			foreach (var callback in Callbacks)
			{
				callback.EnsureCapacity(CallbackTargets.Count);
				foreach (var target in CallbackTargets)
				{
					callback.Register(target);
				}
			}
		}

		public void UnregisterCallback()
		{
			if (CallbackTargets == null) return;

			foreach (var callback in Callbacks)
			{
				foreach (var target in CallbackTargets)
				{
					callback.Unregister(target);
				}
			}

			CallbackTargets.Clear();
		}
	}
}
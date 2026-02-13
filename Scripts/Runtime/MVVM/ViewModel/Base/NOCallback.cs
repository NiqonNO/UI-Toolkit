using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace NiqonNO.UI.MVVM
{
	[Serializable]
	public class NOCallback
	{
		[field: SerializeField] 
		private NOBind BindTarget { get; set; }

		[field: SerializeField] 
		private UnityEvent OnHover;
		
		[field: SerializeField] 
		private UnityEvent OnClick;

		private IEnumerable<VisualElement> CallbackTargets;
		
		public void RegisterCallback(VisualElement context)
		{
			CallbackTargets = BindTarget.QueryTargets(context);
			if (CallbackTargets == null) return;
			
			foreach(var callbackTarget in CallbackTargets)
			{
				callbackTarget.RegisterCallback<PointerEnterEvent>(HoverInvoke);
				callbackTarget.RegisterCallback<ClickEvent>(ClickInvoke);
			}
		}

		public void UnregisterCallback()
		{
			if (CallbackTargets == null) return;
			
			foreach(var callbackTarget in CallbackTargets)
			{
				callbackTarget.UnregisterCallback<PointerEnterEvent>(HoverInvoke);
				callbackTarget.UnregisterCallback<ClickEvent>(ClickInvoke);
			}
		}

		private void HoverInvoke(PointerEnterEvent evt) => OnHover?.Invoke();
		private void ClickInvoke(ClickEvent evt) => OnClick?.Invoke();
	}
}
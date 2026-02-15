using System;
using UnityEngine.UIElements;

namespace NiqonNO.UI.Callbacks
{
	public class NOHoverCallbackManipulator : Manipulator
	{
		private readonly Action Callback;
		
		public NOHoverCallbackManipulator(Action callback)
		{
			Callback = callback;
		}

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
		}
		
		private void OnPointerEnter(PointerEnterEvent evt)
		{
			Callback?.Invoke();
		}
	}
}

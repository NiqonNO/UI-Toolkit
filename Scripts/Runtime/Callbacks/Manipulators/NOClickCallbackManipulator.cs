using System;
using UnityEngine.UIElements;

namespace NiqonNO.UI.Callbacks
{
	public class NOClickCallbackManipulator : PointerManipulator
	{
		private readonly Action Callback;
		
		protected bool IsActive;
		private int PointerId;
		
		public NOClickCallbackManipulator(Action callback)
		{
			IsActive = false;
			PointerId = -1;
			Callback = callback;
			//activators.Add(new ManipulatorActivationFilter { button = MouseButton });
		}

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<ClickEvent>(OnPointerClick);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<ClickEvent>(OnPointerClick);
		}
		
		private void OnPointerClick(ClickEvent evt)
		{
			Callback?.Invoke();
		}
	}
}
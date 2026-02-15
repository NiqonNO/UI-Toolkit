using System;
using UnityEngine.UIElements;

namespace NiqonNO.UI.Callbacks
{
	public class NOHoverCallbackManipulator : PointerManipulator
	{
		private readonly Action Callback;
		
		protected bool IsActive;
		private int PointerId;
		
		public NOHoverCallbackManipulator(Action callback)
		{
			IsActive = false;
			PointerId = -1;
			Callback = callback;
			//activators.Add(new ManipulatorActivationFilter { button = MouseButton });
		}

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
			
			target.RegisterCallback<PointerDownEvent>(OnPointerDown);
			target.RegisterCallback<PointerUpEvent>(OnPointerUp);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
			
			target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
			target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
		}
		
		private void OnPointerEnter(PointerEnterEvent evt)
		{
			/*if (!IsActive || !target.HasPointerCapture(PointerId))
				return;*/

			Callback.Invoke();
		}

		private void OnPointerDown(PointerDownEvent evt)
		{
			/*if (IsActive)
			{
				evt.StopImmediatePropagation();
				return;
			}

			if (!CanStartManipulation(evt)) return;

			IsActive = true;
			PointerId = evt.pointerId;
			target.CapturePointer(PointerId);
			evt.StopPropagation();*/
		}

		private void OnPointerUp(PointerUpEvent evt)
		{
			/*if (!IsActive || !target.HasPointerCapture(PointerId) || !CanStopManipulation(evt))
				return;

			IsActive = false;
			target.ReleaseMouse();
			evt.StopPropagation();*/
		}
	}
}

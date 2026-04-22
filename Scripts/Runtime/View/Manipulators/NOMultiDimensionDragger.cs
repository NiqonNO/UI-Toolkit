using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI.View
{
	public class NOMultiDimensionDragger : PointerManipulator
	{
		private readonly Action<Vector2> OnDrag;
		private readonly VisualElement Handle;

		private bool IsActive;
		private Vector2 Offset;
		private int PointerId;

		public NOMultiDimensionDragger(VisualElement dragHandle, Action<Vector2> onDrag)
		{
			IsActive = false;
			PointerId = -1;
			activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
			
			OnDrag = onDrag;
			Handle = dragHandle;
		}

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<PointerDownEvent>(OnPointerDown);
			target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
			target.RegisterCallback<PointerUpEvent>(OnPointerUp);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
			target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
			target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
		}

		private void OnPointerDown(PointerDownEvent evt)
		{
			if (IsActive)
			{
				evt.StopImmediatePropagation();
				return;
			}

			if (!CanStartManipulation(evt)) return;

			IsActive = true;
			PointerId = evt.pointerId;
			target.CapturePointer(PointerId);
			evt.StopPropagation();

			Vector2 clickPoint = evt.localPosition;
			if (Handle.localBound.Contains(clickPoint))
			{
				Offset = Handle.layout.position - clickPoint;
				return;
			}

			Offset = Vector2.zero;
			UpdateDrag(clickPoint);
		}

		private void OnPointerMove(PointerMoveEvent evt)
		{
			if (!IsActive || !target.HasPointerCapture(PointerId))
				return;

			evt.StopPropagation();
			
			UpdateDrag(evt.localPosition);
		}

		private void OnPointerUp(PointerUpEvent evt)
		{
			if (!IsActive || !target.HasPointerCapture(PointerId) || !CanStopManipulation(evt))
				return;

			IsActive = false;
			target.ReleaseMouse();
			evt.StopPropagation();
		}

		private void UpdateDrag(Vector2 localPosition)
		{
			OnDrag?.Invoke(NormalizePosition(localPosition + Offset));
		}

		private Vector2 NormalizePosition(Vector2 localPosition)
		{
			Rect rect = target.contentRect;
			return (localPosition - rect.position) / rect.size;
		}
	}
}
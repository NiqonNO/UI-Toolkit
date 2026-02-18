using System;
using NiqonNO.Core.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOBarycentricDragger : PointerManipulator
	{
		private static readonly Vector2 LeftCorner = new(0.0f, 1.0f);
		private static readonly Vector2 TopCorner = new(0.5f, 0.0f);
		private static readonly Vector2 RightCorner = new(1.0f, 1.0f);

		private readonly Action<Vector3> OnDrag;
		private readonly VisualElement Handle;

		private bool IsActive;
		private Vector2 Offset;
		private int PointerId;

		public NOBarycentricDragger(VisualElement dragHandle, Action<Vector3> dragHandler)
		{
			IsActive = false;
			PointerId = -1;
			activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
			
			OnDrag = dragHandler;
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
			var normalizedPosition = NormalizePosition(localPosition + Offset);
			var barycentric = NOBarycentricMath.GetBarycentricFromPosition(normalizedPosition, LeftCorner, TopCorner, RightCorner);
			OnDrag?.Invoke(barycentric);
		}

		private Vector2 NormalizePosition(Vector2 localPosition)
		{
			Rect rect = target.contentRect;
			return (localPosition - rect.position) / rect.size;
		}
	}
}
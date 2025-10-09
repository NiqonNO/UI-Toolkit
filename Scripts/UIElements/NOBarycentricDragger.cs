using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOBarycentricDragger : PointerManipulator
	{
		private static readonly Vector2 LeftCorner = new(0.0f, 1.0f);
		private static readonly Vector2 TopCorner = new(0.5f, 0.0f);
		private static readonly Vector2 RightCorner = new(1.0f, 1.0f);

		private readonly VisualElement Handle;

		protected bool IsActive;
		private Vector2 Offset;
		private readonly Action<Vector3> OnDrag;
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

			UpdateDrag(evt.localPosition);

			evt.StopPropagation();
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
			var barycentric = GetBarycentricFromPosition(normalizedPosition);
			OnDrag?.Invoke(barycentric);
		}

		private Vector2 NormalizePosition(Vector2 localPosition)
		{
			return (localPosition - target.contentRect.position) / target.contentRect.size;
		}

		private Vector3 GetBarycentricFromPosition(Vector2 position)
		{
			var v0 = LeftCorner - TopCorner;
			var v1 = RightCorner - TopCorner;
			var v2 = position - TopCorner;

			var d00 = Vector2.Dot(v0, v0);
			var d01 = Vector2.Dot(v0, v1);
			var d11 = Vector2.Dot(v1, v1);
			var d20 = Vector2.Dot(v2, v0);
			var d21 = Vector2.Dot(v2, v1);

			var denom = d00 * d11 - d01 * d01;
			var x = (d11 * d20 - d01 * d21) / denom;
			var z = (d00 * d21 - d01 * d20) / denom;
			var y = 1.0f - x - z;

			if (x < 0)
			{
				var t = Vector2.Dot(position - TopCorner, RightCorner - TopCorner) /
				        Vector2.Dot(RightCorner - TopCorner, RightCorner - TopCorner);
				t = Mathf.Clamp01(t);
				return new Vector3(0.0f, 1.0f - t, t);
			}

			if (y < 0)
			{
				var t = Vector2.Dot(position - RightCorner, LeftCorner - RightCorner) /
				        Vector2.Dot(LeftCorner - RightCorner, LeftCorner - RightCorner);
				t = Mathf.Clamp01(t);
				return new Vector3(t, 0.0f, 1.0f - t);
			}

			if (z < 0)
			{
				var t = Vector2.Dot(position - LeftCorner, TopCorner - LeftCorner) /
				        Vector2.Dot(TopCorner - LeftCorner, TopCorner - LeftCorner);
				t = Mathf.Clamp01(t);
				return new Vector3(1.0f - t, t, 0.0f);
			}

			return new Vector3(x, y, z);
		}
	}
}
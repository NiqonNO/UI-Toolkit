using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI.View
{
	public class NOResizer : PointerManipulator
	{
		private readonly VisualElement DragTarget;

		private bool IsActive;
		private Vector2 Start;
		private int PointerId;
		
		public NOResizer(VisualElement dragTarget)
		{
			IsActive = false;
			PointerId = -1;
			activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
			
			DragTarget = dragTarget;
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

			Start = evt.localPosition;
		}

		private void OnPointerMove(PointerMoveEvent evt)
		{
			if (!IsActive || !target.HasPointerCapture(PointerId))
				return;

			evt.StopPropagation();
			UpdateDrag((Vector2)evt.localPosition - Start);
		}

		private void OnPointerUp(PointerUpEvent evt)
		{
			if (!IsActive || !target.HasPointerCapture(PointerId) || !CanStopManipulation(evt))
				return;

			IsActive = false;
			target.ReleaseMouse();
			evt.StopPropagation();
		}
		
		private void UpdateDrag(Vector2 delta)
		{
			Rect container = DragTarget.hierarchy.parent.contentRect;
			Rect position = DragTarget.layout;
			position.width = Mathf.Clamp(position.width + delta.x, 0, container.width - position.x);
			position.height = Mathf.Clamp(position.height + delta.y, 0, container.height - position.y);
			
			DragTarget.style.top = position.y;
			DragTarget.style.left = position.x;
			
			DragTarget.style.width = position.width;
			DragTarget.style.height = position.height;
		}
	}
}
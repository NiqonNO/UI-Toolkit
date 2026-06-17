using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI.View
{
	public class NOResizer : PointerManipulator
	{
		private readonly VisualElement DragTarget;
		private readonly Vector2 MinSize;
		private readonly bool PreserveAspect;

		public event Action<Rect> OnRectChanged;
		
		private bool IsActive;
		private Vector2 Start;
		private int PointerId;

		public NOResizer(VisualElement dragTarget, bool preserveAspect = false)
		{
			IsActive = false;
			PointerId = -1;
			activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });

			PreserveAspect = preserveAspect;
			DragTarget = dragTarget;
		}
		public NOResizer(VisualElement dragTarget, Vector2 minSize, bool preserveAspect = false) : this(dragTarget, preserveAspect)
		{
			MinSize = minSize;
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


			if (PreserveAspect)
			{
				float aspect = position.width / position.height;

				Vector2 aspectDirection = new Vector2(1f, 1f / aspect).normalized;

				float maxMagnitude = Mathf.Min(
					(container.width - position.x - position.width) / aspectDirection.x,
					(container.height - position.y - position.height) / aspectDirection.y);

				float minMagnitude = Mathf.Max(
					(MinSize.x - position.width) / aspectDirection.x,
					(MinSize.y - position.height) / aspectDirection.y);

				float magnitude = Mathf.Clamp(Vector2.Dot(delta, aspectDirection), minMagnitude, maxMagnitude);

				delta = aspectDirection * magnitude;

				position.width += delta.x;
				position.height += delta.y;
			}
			else
			{
				position.width = Mathf.Clamp(position.width + delta.x, MinSize.x, container.width - position.x);
				position.height = Mathf.Clamp(position.height + delta.y, MinSize.y, container.height - position.y);
			}

			DragTarget.style.top = position.y;
			DragTarget.style.left = position.x;

			DragTarget.style.width = position.width;
			DragTarget.style.height = position.height;

			OnRectChanged?.Invoke(position);
		}
	}
}
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOContentScroller : PointerManipulator
	{
		private readonly Action OnReachNext;
		private readonly Action OnReachPrevious;
		
		private IVisualElementScheduledItem Scheduler;

		private ToggleSelectorDirection ScrollDirection = ToggleSelectorDirection.Horizontal;
		private int Axis => 1 - (int)ScrollDirection;
		private float TileSize = 160f;
		
		private bool Hold;
		private bool Dragging;
		private int PointerId;
		private float DragDelta;

		public NOContentScroller(Action onReachNext, Action onReachPrevious)
		{
			Hold = false;
			Dragging = false;
			PointerId = -1;
			DragDelta = 0;
			activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });

			OnReachNext = onReachNext;
			OnReachPrevious = onReachPrevious;
		}

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<PointerDownEvent>(OnPointerDown);
			target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
			target.RegisterCallback<PointerUpEvent>(OnPointerUp);
			
			target.RegisterCallback<WheelEvent>(OnScroll);
		}
		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
			target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
			target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
			
			target.UnregisterCallback<WheelEvent>(OnScroll);
		}

		private void OnPointerDown(PointerDownEvent evt)
		{
			if (Hold)
			{
				evt.StopImmediatePropagation();
				return;
			}

			if (!CanStartManipulation(evt)) return;

			Hold = true;
			PointerId = evt.pointerId;
			target.CapturePointer(PointerId);
			evt.StopPropagation();
		}

		private void OnPointerMove(PointerMoveEvent evt)
		{
			if (!Hold || !target.HasPointerCapture(PointerId))
				return;
			
			evt.StopPropagation();
			
			if (!Dragging)
				OnBeginDrag(evt);
			else
				UpdateDrag(evt);
		}

		private void OnPointerUp(PointerUpEvent evt)
		{
			if (!Hold || !target.HasPointerCapture(PointerId) || !CanStopManipulation(evt)) return;

			Hold = false;
			target.ReleaseMouse();
			evt.StopPropagation();
			
			if (Dragging)
				OnEndDrag();
		}
		
		private void OnBeginDrag(PointerMoveEvent evt)
		{
			Dragging = true;
			DragDelta = 0;
		}

		private void UpdateDrag(PointerMoveEvent evt)
		{
			DragDelta += evt.deltaPosition[Axis];
			
			float offset = Mathf.Abs(DragDelta) + TileSize / 2f;
			int indexSteps = Mathf.FloorToInt(offset / TileSize) * (int)Mathf.Sign(DragDelta);
			DragDelta -= TileSize * indexSteps;
			Scroll(-indexSteps);
		}

		private void OnEndDrag()
		{
			Dragging = false;
			
			DragDelta = 0;
			UpdatePosition();
		}
		
		private void OnScroll(WheelEvent evt)
		{
			if (Hold) return;
			
			DragDelta += evt.delta.y;
		}
		
		public void ScrollToNext() => Scroll(1);
		public void ScrollToPrevious() => Scroll(-1);
		
		private void Scroll(int scrollStep)
		{
			if (scrollStep > 0)
			{
				OnReachNext?.Invoke();
				Scroll(--scrollStep);
				return;
			}
			if (scrollStep < 0)
			{
				OnReachPrevious?.Invoke();
				Scroll(++scrollStep);
				return;
			}

			UpdatePosition();
		}
		
		private void UpdatePosition()
		{
			Vector3 position = target.transform.position;
			position[Axis] = -((target.layout.size[Axis] - target.parent.layout.size[Axis]) / 2.0f) + DragDelta;
			target.transform.position = position;
		}
	}
}
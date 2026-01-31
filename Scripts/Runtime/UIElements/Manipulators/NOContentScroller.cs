using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOContentScroller : PointerManipulator
	{
		private readonly Func<Vector2> GetCenteringOffset;
		private readonly Action OnReachNext;
		private readonly Action OnReachPrevious;
		
		private IVisualElementScheduledItem Scheduler;

		private ToggleSelectorDirection ScrollDirection = ToggleSelectorDirection.Horizontal;
		private int Axis => 1 - (int)ScrollDirection;
		private float TileSize = 160f;
		
		private bool Hold;
		private bool Dragging;
		private int PointerId;
		private float Velocity;
		private float DragDelta;

		public NOContentScroller(Func<Vector2> getCenteringOffset, Action onReachNext, Action onReachPrevious)
		{
			Hold = false;
			Dragging = false;
			PointerId = -1;
			Velocity = 0f;
			activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
			
			GetCenteringOffset = getCenteringOffset;
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
			Velocity = 0f;
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

			int steps = Mathf.FloorToInt(Mathf.Abs(DragDelta) / TileSize) * (int)Mathf.Sign(DragDelta);
			DragDelta -= TileSize * steps;
			Scroll(-steps);
		}

		private void OnEndDrag()
		{
			Dragging = false;
			
			DragDelta = 0;
			CenterContent();
		}
		
		private void OnScroll(WheelEvent eventData)
		{
	        
		}
		
		public void ScrollToNext() => Scroll(1);
		public void ScrollToPrevious() => Scroll(-1);
		
		private void ScrollToIndex(int newIndex)
		{
			/*if (value == null)
				return;
            
			newIndex = (int)Mathf.Repeat(newIndex, value.Items.Count);

			int ascending = (newIndex - CurrentIndex + value.Items.Count) % value.Items.Count;
			int descending = (CurrentIndex - newIndex + value.Items.Count) % value.Items.Count;

			if (ascending < descending)
				ScrollStep += ascending;
			else
				ScrollStep -= descending;*/
		}
		
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

			CenterContent();
		}
		
		void CenterContent()
		{
			Vector2 targetOffset = GetCenteringOffset.Invoke();
            
			Vector3 position = target.transform.position;
			position.x = -targetOffset.x;
			position.y = -targetOffset.y;
			position[Axis] += DragDelta;
			target.transform.position = position;
		}
	}
}
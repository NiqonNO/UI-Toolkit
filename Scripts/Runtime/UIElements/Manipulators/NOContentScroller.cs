using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOContentScroller : PointerManipulator
	{
		private readonly Action OnReachNext;
		private readonly Action OnReachPrevious;

		private ToggleSelectorDirection ScrollDirection = ToggleSelectorDirection.Horizontal;
		private int Axis => 1 - (int)ScrollDirection;
		private float TileSize = 160f;
		
		private IVisualElementScheduledItem AutoScroll;
		private IVisualElementScheduledItem ManualScroll;
		
		private bool Hold;
		private bool Dragging;
		private bool Scrolling;
		private int PointerId;
		private float DragDelta;
		private float Velocity;
		
		private float AutoScrollStart;
		private float AutoScrollTime;
		private NOEase AutoScrollEase = NOEase.OutElastic;
		
		private float ManualScrollStart;
		private float ManualScrollTime;
		private NOEase ManualScrollEase = NOEase.InCirc;

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

			StopAutoScroll();
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
			Scroll();
		}

		private void OnEndDrag()
		{
			Dragging = false;
			RunAutoScroll();
		}

		private void OnScroll(WheelEvent evt)
		{
			if (Hold) return;

			ManualScrollStart = Velocity + evt.delta.y * 5;
			ManualScrollTime = 0;
			if(!Scrolling)
				RunManualScroll();
		}

		private void Scroll()
		{
			float offset = Mathf.Abs(DragDelta) + TileSize / 2f;
			int indexSteps = Mathf.FloorToInt(offset / TileSize) * (int)Mathf.Sign(-DragDelta);
			Step(indexSteps);
		}
				
		public void ScrollToNext()
		{
			Step(1);
			RunAutoScroll();
		}

		public void ScrollToPrevious()
		{
			Step(-1);
			RunAutoScroll();
		}

		private void Step(int scrollStep)
		{
			switch (scrollStep)
			{
				case > 0:
					target[0].BringToFront();
					DragDelta += TileSize;
					OnReachNext?.Invoke();
					Step(--scrollStep);
					return;
				case < 0:
					target[target.childCount - 1].SendToBack();
					DragDelta -= TileSize;
					OnReachPrevious?.Invoke();
					Step(++scrollStep);
					return;
				default:
					UpdatePosition();
					return;
			}
		}

		private void RunManualScroll()
		{
			if (ManualScroll == null)
				ManualScroll = target.schedule.Execute(ManualScrollUpdate).Every((int)(Time.fixedDeltaTime * 1000));
			else
				ManualScroll.Resume();
			
			AutoScroll?.Pause();
			Scrolling = true;
		}
		private void ManualScrollUpdate(TimerState time)
		{
			ManualScrollTime = Mathf.Clamp01(ManualScrollTime + time.deltaTime/200f);
			Velocity = Mathf.Lerp(ManualScrollStart, 0,  ManualScrollEase.Ease(ManualScrollTime));
			DragDelta += Velocity;
			if(Mathf.Approximately(Velocity, 0))
			{
				StopManualScroll();
				return;
			}

			Scroll();
		}
		private void StopManualScroll()
		{
			Scrolling = false;
			Velocity = 0;
			ManualScroll?.Pause();
			RunAutoScroll();
		}
		
		private void RunAutoScroll()
		{
			AutoScrollStart = DragDelta;
			AutoScrollTime = 0;
			if (AutoScroll == null)
				AutoScroll = target.schedule.Execute(AutoScrollUpdate).Every((int)(Time.fixedDeltaTime * 1000));
			else
				AutoScroll.Resume();
		}
		private void AutoScrollUpdate(TimerState time)
		{
			AutoScrollTime = Mathf.Clamp01(AutoScrollTime + time.deltaTime/1000f);
			if(Mathf.Approximately(AutoScrollTime, 1))
			{
				StopAutoScroll();
				return;
			}
			
			DragDelta = Mathf.LerpUnclamped(AutoScrollStart, 0,  AutoScrollEase.Ease(AutoScrollTime));
			UpdatePosition();
		}
		private void StopAutoScroll()
		{
			DragDelta = 0;
			AutoScroll?.Pause();
			UpdatePosition();
		}
		
		public void UpdatePosition()
		{
			Vector3 position = target.transform.position;
			position[Axis] = -((target.layout.size[Axis] - target.parent.layout.size[Axis]) / 2.0f) + DragDelta;
			target.transform.position = position;
		}
	}
}
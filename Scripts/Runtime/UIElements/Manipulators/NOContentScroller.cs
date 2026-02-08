using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOContentScroller : PointerManipulator
	{
		private readonly Action OnReachNext;
		private readonly Action OnReachPrevious;
		
		private int Direction;
		private int Axis;
		private float TileSize;

		private ScrollTween AutoScroll;
		private ScrollTween ManualScroll;
		
		private bool Hold;
		private bool Dragging;
		private int PointerId;
		private float DragDelta;

		public NOContentScroller(Action onReachNext, Action onReachPrevious, 
			float tileSize, ScrollDirection scrollDirection,
			NOEase centeringEase = NOEase.Linear, float centeringDuration = 0.3f, 
			NOEase decelerationEase = NOEase.InCirc, float decelerationDuration = 0.3f)
		{
			Hold = false;
			Dragging = false;
			PointerId = -1;
			DragDelta = 0;
			activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });

			OnReachNext = onReachNext;
			OnReachPrevious = onReachPrevious;
			
			SetTileSize(tileSize);
			SetDirection(scrollDirection);
			
			AutoScroll = new ScrollTween().SetEase(centeringEase).SetDuration(centeringDuration).OnTick(OnAutoScroll);
			ManualScroll = new ScrollTween().SetEase(decelerationEase).SetDuration(decelerationDuration).OnTick(OnManualScroll).OnComplete(RunAutoScroll);
		}
		
		public void SetTileSize(float size)
		{
			TileSize = size;
		}

		public void SetDirection(ScrollDirection direction) { Direction = (int)direction; Axis = 1 - Direction; }
		public void SetCenteringEase(NOEase ease) => AutoScroll.SetEase(ease);
		public void SetCenteringDuration(float duration) => AutoScroll.SetDuration(duration);
		public void SetScrollDecelerationEase(NOEase ease) => ManualScroll.SetEase(ease);
		public void SetScrollDecelerationDuration(float duration) => ManualScroll.SetDuration(duration);
		
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
			
			AutoScroll.Stop();
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
			RunManualScroll(evt.delta.y * 5);
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
		
		private void RunManualScroll(float velocity)
		{
			AutoScroll.Stop();
			ManualScroll.Run(target, velocity);
		}
		private void OnManualScroll(float delta)
		{
			DragDelta += delta;
			Scroll();
		}
		private void RunAutoScroll()
		{
			AutoScroll.Run(target, DragDelta);
		}
		private void OnAutoScroll(float delta)
		{
			DragDelta = delta;
			UpdatePosition();
		}
		
		public void UpdatePosition()
		{
			float contentSize = target.contentRect.size[Axis];
			float viewportSize = target.parent.contentRect.size[Axis];
			
			Vector3 position = target.transform.position;
			position[Direction] = 0;
			position[Axis] = -(contentSize - viewportSize) / 2.0f + DragDelta;
			target.transform.position = position;
		}

		private class ScrollTween
		{
			private static int UpdateDeltaTime = (int)(Time.fixedDeltaTime * 1000);
			
			private Action<float> TickEvent;
			private Action CompleteEvent;  
			
			private IVisualElementScheduledItem Scheduler;
			private float Duration;
			private NOEase Ease;
				
			private float Start = 0;
			private float Progress = 0;
			
			public ScrollTween SetEase(NOEase value)
			{ Ease = value; return this; }
			public ScrollTween SetDuration(float value)
			{ Duration = value * 1000; return this; }
			public ScrollTween OnTick(Action<float> value)
			{ TickEvent = value; return this; }
			public ScrollTween OnComplete(Action value)
			{ CompleteEvent = value; return this; }


			public void Run(VisualElement target, float startValue)
			{
				Start = startValue;
				Progress = 0f;
				
				if (Scheduler == null)
					Scheduler = target.schedule.Execute(Update).Every(UpdateDeltaTime);
				else if(target == Scheduler.element)
					Scheduler.Resume();
				else
					Scheduler = target.schedule.Execute(Update).Every(UpdateDeltaTime);
			}

			void Update(TimerState t)
			{
				Progress = Mathf.Clamp01(Progress + t.deltaTime / Duration);
				float value = Mathf.LerpUnclamped(Start, 0f, Ease.Ease(Progress));
				TickEvent?.Invoke(value);

				if (Mathf.Approximately(Progress, 1f)) Stop();
			}

			public void Stop()
			{
				Scheduler?.Pause();
				CompleteEvent?.Invoke();
			}
		}
	}
}
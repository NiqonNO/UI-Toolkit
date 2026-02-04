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

		private ScrollTween AutoScroll;
		private ScrollTween ManualScroll;
		
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

			AutoScroll = new ScrollTween(target, NOEase.OutElastic, 1.0f, OnAutoScroll);
			ManualScroll = new ScrollTween(target, NOEase.InCirc, 0.3f, OnManualScroll, RunAutoScroll);
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
			ManualScroll.Run(velocity);
		}
		private void OnManualScroll(float delta)
		{
			DragDelta += delta;
			Scroll();
		}
		private void RunAutoScroll()
		{
			AutoScroll.Run(DragDelta);
		}
		private void OnAutoScroll(float delta)
		{
			DragDelta = delta;
			UpdatePosition();
		}
		
		public void UpdatePosition()
		{
			Vector3 position = target.transform.position;
			position[Axis] = -((target.layout.size[Axis] - target.parent.layout.size[Axis]) / 2.0f) + DragDelta;
			target.transform.position = position;
		}

		private class ScrollTween
		{
			private static int UpdateDeltaTime = (int)(Time.fixedDeltaTime * 1000);
			
			private readonly Action<float> OnTick;
			private readonly Action OnComplete;  
			
			private readonly VisualElement Target;
			private readonly float Duration;
			private readonly NOEase Ease;
			
			private IVisualElementScheduledItem Scheduler;
				
			private float Start = 0;
			private float Progress = 0;

			public ScrollTween(VisualElement target, NOEase ease, float duration, Action<float> onTick, Action onComplete = null)
			{
				Target = target;
				Ease = ease;
				Duration = duration * 1000;
				OnTick = onTick;
				OnComplete = onComplete;
			}
			
			public void Run(float startValue)
			{
				Start = startValue;
				Progress = 0f;
				
				if (Scheduler == null)
					Scheduler = Target.schedule.Execute(Update).Every(UpdateDeltaTime);
				else
					Scheduler.Resume();
			}

			void Update(TimerState t)
			{
				Progress = Mathf.Clamp01(Progress + t.deltaTime / Duration);
				float value = Mathf.LerpUnclamped(Start, 0f, Ease.Ease(Progress));
				OnTick?.Invoke(value);

				if (Mathf.Approximately(Progress, 1f)) Stop();
			}

			public void Stop()
			{
				Scheduler?.Pause();
				OnComplete?.Invoke();
			}
		}
	}
}
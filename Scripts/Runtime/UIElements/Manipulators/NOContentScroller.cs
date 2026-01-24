using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOContentScroller : Manipulator
	{
		private readonly Func<Vector2> GetCenteringOffset;
		private readonly Action OnReachNext;
		private readonly Action OnReachPrevious;
		
		private IVisualElementScheduledItem Scheduler;
		
		private int _ScrollStep;
		public int ScrollStep
		{
			get => _ScrollStep;
			set
			{
				_ScrollStep = value;
				if (Scheduler != null)
				{
					Scheduler.Resume();
					return;
				}

				CreateScheduler();
			}
		}

		public NOContentScroller(Func<Vector2> getCenteringOffset, Action onReachNext, Action onReachPrevious)
		{
			GetCenteringOffset = getCenteringOffset;
			OnReachNext = onReachNext;
			OnReachPrevious = onReachPrevious;
		}
		
		protected override void RegisterCallbacksOnTarget()
		{
		}

		protected override void UnregisterCallbacksFromTarget()
		{
		}
		
		private void CreateScheduler()
		{
			Scheduler = target.schedule.Execute(Scroll).Every(0).Until(() => _ScrollStep == 0);
		}
		
		public void ScrollToNext() => ScrollStep++;
		public void ScrollToPrevious() => ScrollStep--;
		
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
		
		private void Scroll(TimerState obj)
		{
			if (_ScrollStep > 0)
			{
				OnReachNext?.Invoke();
				_ScrollStep--;
			}
			else
			{
				OnReachPrevious?.Invoke();
				_ScrollStep++;
			}

			CenterContent();
		}
		
		void CenterContent()
		{
			Vector2 targetOffset = GetCenteringOffset.Invoke();
            
			Vector3 position = target.transform.position;
			position.x = -targetOffset.x;
			position.y = -targetOffset.y;
			target.transform.position = position;
		}
	}
}
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOContentScroller : Manipulator
	{
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
		
		private void Scroll(TimerState obj)
		{
			if (_ScrollStep > 0)
			{
				int transferIndex = CurrentIndex + Mathf.CeilToInt(target.childCount / 2.0f);
				SetData(target[0], value.Items[(int)Mathf.Repeat(transferIndex, value.Items.Count)]);
				target[0].BringToFront();

				_ScrollStep--;
				CurrentIndex = (int)Mathf.Repeat(++CurrentIndex, value.Items.Count);
			}
			else
			{
				int transferIndex = CurrentIndex - Mathf.FloorToInt(target.childCount / 2.0f) - 1;
				SetData(target[target.childCount - 1], value.Items[(int)Mathf.Repeat(transferIndex, value.Items.Count)]);
				target[target.childCount - 1].SendToBack();

				_ScrollStep++;
				CurrentIndex = (int)Mathf.Repeat(--CurrentIndex, value.Items.Count);
			}

			CenterContent();
		}
		
		void CenterContent()
		{
			Vector2 targetOffset =
				new Vector2((target.layout.width - target.parent.layout.width) / 2.0f, 0);
			if (target.childCount % 2.0f == 0)
				targetOffset.x += 105f;
            
			Vector3 position = target.transform.position;
			position.x = -targetOffset.x;
			position.y = -targetOffset.y;
			target.transform.position = position;
		}
	}
}
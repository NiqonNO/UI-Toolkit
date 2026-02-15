using System;
using UnityEngine.UIElements;

namespace NiqonNO.UI.Callbacks
{
	public class NOClickCallbackManipulator : Manipulator
	{
		private readonly Action Callback;
		
		public NOClickCallbackManipulator(Action callback)
		{
			Callback = callback;
		}

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<ClickEvent>(OnPointerClick);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<ClickEvent>(OnPointerClick);
		}
		
		private void OnPointerClick(ClickEvent evt)
		{
			Callback?.Invoke();
		}
	}
}
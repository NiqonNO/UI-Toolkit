using System;
using UnityEngine.UIElements;

namespace NiqonNO.UI.Callbacks
{
	[Serializable]
	public class NOClickCallback : NOCallback
	{
		protected override IManipulator CreateManipulator()
		{
			return new NOClickCallbackManipulator(Callback.Invoke);
		}
	}
}
using System;
using UnityEngine.UIElements;

namespace NiqonNO.UI.Callbacks
{
	[Serializable]
	public class NOHoverCallback : NOCallback
	{
		protected override IManipulator CreateManipulator()
		{
			return new NOHoverCallbackManipulator(Callback.Invoke);
		}
	}
}
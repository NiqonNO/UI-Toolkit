using UnityEngine;

namespace NiqonNO.UI.MVVM
{
	public class NOSliderViewModel : NOPropertyViewModel<float>
	{
		[SerializeField] 
		private string Label;
		
		[SerializeField] 
		private Sprite MinIcon;
		
		[SerializeField] 
		private Sprite MaxIcon;
	}
}
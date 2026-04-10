using UnityEngine;

namespace NiqonNO.UI.MVVM
{
	public class NOQuaternaryViewModel : NOPropertyViewModel<Vector4>
	{
		[SerializeField] 
		private string Label;
		
		[SerializeField] 
		private Sprite XIcon;
		
		[SerializeField] 
		private Sprite YIcon;
		
		[SerializeField] 
		private Sprite ZIcon;
		
		[SerializeField] 
		private Sprite WIcon;
	}
}
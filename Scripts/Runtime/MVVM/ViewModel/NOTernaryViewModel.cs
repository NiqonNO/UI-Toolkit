using UnityEngine;

namespace NiqonNO.UI.MVVM
{
	public class NOTernaryViewModel : NOPropertyViewModel<Vector3>
	{
		[SerializeField] 
		private string Label;
		
		[SerializeField] 
		private Sprite XIcon;
		
		[SerializeField] 
		private Sprite YIcon;
		
		[SerializeField] 
		private Sprite ZIcon;
	}
}
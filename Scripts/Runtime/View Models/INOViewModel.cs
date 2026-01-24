using UnityEngine;

namespace NiqonNO.UI
{
	public interface INOViewModel
	{
		public string DisplayName { get; }
		public string DisplayDescription { get; }
		public Color DisplayColor { get; }
		public Sprite DisplaySprite { get; }
	}
}
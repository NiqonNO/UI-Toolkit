using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	[UxmlElement]
	public partial class NOAspectRatioFitterElement : VisualElement
	{
		private int _RatioHeight;
		private int _RatioWidth;

		public NOAspectRatioFitterElement() : this(1, 1)
		{
		}

		public NOAspectRatioFitterElement(int width, int height)
		{
			_RatioWidth = width;
			_RatioHeight = height;

			RegisterCallback<GeometryChangedEvent>(_ => UpdateElements());
			RegisterCallback<AttachToPanelEvent>(_ => UpdateElements());
		}

		[UxmlAttribute]
		public int RatioWidth
		{
			get => _RatioWidth;
			set
			{
				_RatioWidth = Mathf.Max(1, value);
				UpdateElements();
			}
		}

		[UxmlAttribute]
		public int RatioHeight
		{
			get => _RatioHeight;
			set
			{
				_RatioHeight = Mathf.Max(1, value);
				UpdateElements();
			}
		}

		private void UpdateElements()
		{
			var targetAspect = (float)RatioWidth / RatioHeight;
			var width = parent.resolvedStyle.width;
			var height = parent.resolvedStyle.height;
			var currentAspect = width / height;
			var diff = currentAspect - targetAspect;

			if (RatioWidth <= 0.0f || RatioHeight <= 0.0f)
			{
				ClearMargin();
				return;
			}

			if (diff > 0.01f)
			{
				var w = (width - height * targetAspect) * 0.5f;
				style.marginLeft = w;
				style.marginRight = w;
				style.marginTop = 0;
				style.marginBottom = 0;
			}
			else if (diff < -0.01f)
			{
				var h = (height - width * (1 / targetAspect)) * 0.5f;
				style.marginLeft = 0;
				style.marginRight = 0;
				style.marginTop = h;
				style.marginBottom = h;
			}
			else
			{
				ClearMargin();
			}
		}

		private void ClearMargin()
		{
			style.marginLeft = 0;
			style.marginRight = 0;
			style.marginTop = 0;
			style.marginBottom = 0;
		}
	}
}
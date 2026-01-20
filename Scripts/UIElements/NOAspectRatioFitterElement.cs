using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	[UxmlElement]
	public partial class NOAspectRatioFitterElement : VisualElement
	{
		static readonly CustomStyleProperty<int> StyleRatioHeight = new CustomStyleProperty<int>("--ratio-height");
		static readonly CustomStyleProperty<int> StyleRatioWidth = new CustomStyleProperty<int>("--ratio-width");
		
		private int _RatioHeight;
		private int _RatioWidth;

		public NOAspectRatioFitterElement() : this(1, 1)
		{
		}

		public NOAspectRatioFitterElement(int width, int height)
		{
			_RatioWidth = width;
			_RatioHeight = height;

			RegisterCallback<CustomStyleResolvedEvent>(OnStylesResolved);
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
			var width = resolvedStyle.width;
			var height = resolvedStyle.height;
			var currentAspect = width / height;
			var diff = currentAspect - targetAspect;

			if (RatioWidth <= 0.0f || RatioHeight <= 0.0f)
			{
				ClearPadding();
				return;
			}

			if (diff > 0.01f)
			{
				var w = (width - height * targetAspect) * 0.5f;
				style.paddingLeft = w;
				style.paddingRight = w;
				style.paddingTop = 0;
				style.paddingBottom = 0;
			}
			else if (diff < -0.01f)
			{
				var h = (height - width * (1 / targetAspect)) * 0.5f;
				style.paddingLeft = 0;
				style.paddingRight = 0;
				style.paddingTop = h;
				style.paddingBottom = h;
			}
			else
			{
				ClearPadding();
			}
		}

		private void ClearPadding()
		{
			style.paddingLeft = 0;
			style.paddingRight = 0;
			style.paddingTop = 0;
			style.paddingBottom = 0;
		}
		
		private void OnStylesResolved(CustomStyleResolvedEvent evt)
		{
			if(evt.customStyle.TryGetValue(StyleRatioWidth, out var width))
				_RatioWidth = Mathf.Max(1, width);
			if(evt.customStyle.TryGetValue(StyleRatioHeight, out var height))
				_RatioHeight = Mathf.Max(1, height);
			UpdateElements();
		}
	}
}
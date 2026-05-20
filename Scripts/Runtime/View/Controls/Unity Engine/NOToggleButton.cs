using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI.View
{
	[UxmlElement]
	public partial class NOToggleButton : BaseField<bool>
	{
		private static readonly BindingId IconImageProperty = (BindingId)nameof(IconImage);

		private VisualElement InputContainer => visualInput;
		private Label Label => labelElement;
		private TextElement TextElement;
		private Image ImageElement;

		public event Action Clicked
		{
			add => _Clickable.clicked += value;
			remove => _Clickable.clicked -= value;
		}

		private Clickable _Clickable;

		public Clickable Clickable
		{
			get => _Clickable;
			set
			{
				if (_Clickable != null && _Clickable.target == this)
					this.RemoveManipulator(_Clickable);
				_Clickable = value;
				if (_Clickable == null)
					return;
				this.AddManipulator(_Clickable);
			}
		}

		private Background _IconImage;

		public Background IconImage
		{
			get => _IconImage;
			set
			{
				if (value.IsEmpty() && _IconImage == null || value == _IconImage)
					return;
				if (value.IsEmpty())
				{
					_IconImage = value;
					ResetButtonHierarchy();
				}
				else
				{
					if (ImageElement == null) UpdateButtonHierarchy();
					if (value.texture) ImageElement.image = value.texture;
					else if (value.sprite) ImageElement.sprite = value.sprite;
					else if (value.renderTexture) ImageElement.image = value.renderTexture;
					else ImageElement.vectorImage = value.vectorImage;
					_IconImage = value;
					EnableInClassList(Button.iconOnlyUssClassName, string.IsNullOrEmpty(Text));
				}

				NotifyPropertyChanged(in IconImageProperty);
			}
		}

		[UxmlAttribute, CreateProperty] public bool ToggleOnLabelClick { get; set; } = true;

		private string _Text;

		[UxmlAttribute, CreateProperty]
		public string Text
		{
			get => _Text ?? string.Empty;
			set
			{
				_Text = value;
				EnableInClassList(Button.iconOnlyUssClassName, !_IconImage.IsEmpty() && string.IsNullOrEmpty(Text));
				if (TextElement == null) return;
				if (TextElement.text == _Text)
					return;
				TextElement.text = _Text;
			}
		}

		[UxmlAttribute, CreateProperty]
		[ImageFieldValueDecorator("Icon Image")]
		private UnityEngine.Object IconImageReference
		{
			get => IconImage.GetSelectedImage();
			set => IconImage = Background.FromObject(value);
		}

		public NOToggleButton() : this(new Background(), String.Empty)
		{
		}

		public NOToggleButton(Background iconImage, string label) : base(label, null)
		{
			AddToClassList(NOUSS.ToggleButtonClass);
			
			InputContainer.pickingMode = PickingMode.Position;
			InputContainer.AddToClassList(NOUSS.ToggleButtonInputContainerClass);

			Label.focusable = false;
			Label.AddToClassList(NOUSS.ToggleButtonLabelClass);
			
			IconImage = iconImage;
			this.AddManipulator(Clickable = new Clickable(OnClickEvent));
			RegisterCallback<NavigationSubmitEvent>(OnNavigationSubmit);
		}

		private void OnNavigationSubmit(NavigationSubmitEvent evt)
		{
			ToggleValue();
			evt.StopPropagation();
		}

		protected virtual void ToggleValue() => value = !value;

		public override void SetValueWithoutNotify(bool newValue)
		{
			InputContainer.SetCheckedPseudoState(newValue);
			SetCheckedPseudoState(newValue);
			base.SetValueWithoutNotify(newValue);
		}

		private void OnClickEvent(EventBase evt)
		{
			if (evt.eventTypeId == EventBase<MouseUpEvent>.TypeId())
			{
				var mouseEvent = (IMouseEvent)evt;
				if (ShouldIgnoreClick(mouseEvent.mousePosition) ||
				    mouseEvent.button != 0) return;

			}
			else
			{
				if (evt.eventTypeId != EventBase<PointerUpEvent>.TypeId() &&
				    evt.eventTypeId != EventBase<ClickEvent>.TypeId()) return;

				var pointerEvent = (IPointerEvent)evt;
				if (ShouldIgnoreClick(pointerEvent.position) ||
				    pointerEvent.button != 0) return;
			}

			ToggleValue();
		}

		private bool ShouldIgnoreClick(Vector3 position)
		{
			return !ToggleOnLabelClick && Label.worldBound.Contains(position);
		}


		/*protected override void UpdateMixedValueContent()
		{
			if (showMixedValue)
			{
				InputContainer.SetCheckedPseudoState(false);
				SetCheckedPseudoState(false);
				InputContainer.Add(mixedValueLabel);
				this.m_OriginalText = this.text;
				this.text = "";
			}
			else
			{
				mixedValueLabel.RemoveFromHierarchy();
				if (this.m_OriginalText != null)
					this.text = this.m_OriginalText;
			}
		}*/

		internal override void RegisterEditingCallbacks()
		{
			RegisterCallback<PointerUpEvent>(StartEditing);
			RegisterCallback<FocusOutEvent>(EndEditing);
		}

		internal override void UnregisterEditingCallbacks()
		{
			UnregisterCallback<PointerUpEvent>(StartEditing);
			UnregisterCallback<FocusOutEvent>(EndEditing);
		}

		private void UpdateButtonHierarchy()
		{
			if (ImageElement == null)
			{
				Image image = new Image();
				Label.AddToClassList(NOUSS.ToggleButtonIconClass);
				image.AddToClassList(Button.imageUSSClassName);
				ImageElement = image;
				InputContainer.Add(ImageElement);
				AddToClassList(Button.iconUssClassName);
			}

			if (TextElement == null)
			{
				TextElement = new TextElement() { text = Text };
				Label.AddToClassList(NOUSS.ToggleButtonTextClass);
				_Text = Text;
				InputContainer.Add(TextElement);
			}
		}

		private void ResetButtonHierarchy()
		{
			if (ImageElement != null)
			{
				ImageElement.RemoveFromHierarchy();
				ImageElement = null;
				RemoveFromClassList(Button.iconUssClassName);
				RemoveFromClassList(Button.iconOnlyUssClassName);
			}

			if (TextElement != null)
			{
				string text = TextElement.text;
				TextElement.RemoveFromHierarchy();
				TextElement = null;
				Text = text;
			}
		}
	}
}
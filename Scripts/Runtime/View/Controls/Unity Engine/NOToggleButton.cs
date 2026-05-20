using System;
using Unity.Properties;
using UnityEngine.UIElements;

[UxmlElement]
public partial class NOToggleButton : BaseBoolField
{
	private static readonly BindingId IconImageProperty = (BindingId) nameof (IconImage);
	
	private Image ImageElement;
	
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
				EnableInClassList(Button.iconOnlyUssClassName, string.IsNullOrEmpty(text));
			}
			NotifyPropertyChanged(in IconImageProperty);
		}
	}

	[UxmlAttribute, CreateProperty]
	[ImageFieldValueDecorator("Icon Image")]
	private UnityEngine.Object IconImageReference
	{
		get => IconImage.GetSelectedImage();
		set => IconImage = Background.FromObject((object)value);
	}

	public NOToggleButton() : this(new Background(), String.Empty)
	{
	}
	public NOToggleButton(Background iconImage, string label) : base(label)
	{
		IconImage = iconImage;
	}

	private void UpdateButtonHierarchy()
	{
		if (ImageElement != null) return;
		Image image = new Image();
		image.AddToClassList(Button.imageUSSClassName);
		ImageElement = image;
		Add(ImageElement);
		AddToClassList(Button.iconUssClassName);
	}

	private void ResetButtonHierarchy()
	{
		if (ImageElement == null) return;
		ImageElement.RemoveFromHierarchy();
		ImageElement = null;
		RemoveFromClassList(Button.iconUssClassName);
		RemoveFromClassList(Button.iconOnlyUssClassName);
	}
}
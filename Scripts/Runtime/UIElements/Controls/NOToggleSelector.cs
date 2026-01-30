using NiqonNO.UI.MVVM;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
    [UxmlElement]
    public partial class NOToggleSelector : NOCollectionView<INOBindingContext>
    {

        private readonly Label LabelElement;
        private readonly VisualElement InputContainer;
        private readonly Button NextButton;
        private readonly Button PreviousButton;
        private readonly VisualElement ContentContainer;
        private readonly VisualElement ContentViewport;
        
        private ToggleSelectorDirection _Mode;
        [UxmlAttribute]
        public ToggleSelectorDirection Mode
        {
            get => _Mode;
            set => SetScrollViewMode(value);
        }
        
        private VisualTreeAsset _ItemTemplate;
        [UxmlAttribute]
        public VisualTreeAsset ItemTemplate
        {
            get => _ItemTemplate;
            set
            {
                if (_ItemTemplate == value)
                    return;
                _ItemTemplate = value;
                Rebuild();
            }
        }

        private float ItemWidth => 160f;

        public NOToggleSelector() : this(string.Empty)
        {
        }

        public NOToggleSelector(string label) : base()
        {
            AddToClassList(NOUSS.ItemSelectorClass);
            if (label != null)
            {
                LabelElement = new Label(label);
                LabelElement.AddToClassList(NOUSS.ToggleSelectorLabelClass);
                Add(LabelElement);
            }
            InputContainer = new VisualElement();
            InputContainer.AddToClassList(NOUSS.ToggleSelectorInputContainerClass);
            Add(InputContainer);

            ContentViewport = new VisualElement { name = "toggle-selector-content-viewport", pickingMode = PickingMode.Ignore, };
            ContentViewport.AddToClassList(NOUSS.ToggleSelectorViewportClass);
            ContentViewport.RegisterCallback<GeometryChangedEvent>(UpdatePool);
            
            var scroller = new NOContentScroller(GetCenteringOffset, JumpNext, JumpPrevious);
            ContentContainer = new VisualElement { name = "toggle-selector-content-container", usageHints = UsageHints.GroupTransform };
            ContentContainer.AddToClassList(NOUSS.ToggleSelectorContentContainerClass);
            ContentContainer.AddManipulator(scroller);
            
            PreviousButton = new Button(scroller.ScrollToPrevious) { name = "toggle-selector-previous",  };
            PreviousButton.AddToClassList(NOUSS.ToggleSelectorButtonClass);
            PreviousButton.AddToClassList(NOUSS.ToggleSelectorButtonPreviousClass);
            
            NextButton = new Button(scroller.ScrollToNext) { name = "toggle-selector-next", };
            NextButton.AddToClassList(NOUSS.ToggleSelectorButtonClass);
            NextButton.AddToClassList(NOUSS.ToggleSelectorButtonNextClass);

            
            InputContainer.Add(PreviousButton);
            InputContainer.Add(ContentViewport);
            ContentViewport.Add(ContentContainer);
            InputContainer.Add(NextButton);
        }

        private void Rebuild()
        {
            ClearPool();
            UpdatePool();
        }

        private void UpdatePool(GeometryChangedEvent evt) => UpdatePool();

        private void UpdatePool()
        {
            if (ItemTemplate == null)
                return;
            
            ResizeItemPool();
            JumpToIndex(SelectedIndex);
        }

        private void ClearPool()
        {
            while (ContentContainer.childCount > 0)
            {
                ContentContainer.RemoveAt(0);
            }
        }
        private void ResizeItemPool()
        {
            float containerWidth = ContentViewport.contentRect.width;

            int count = Mathf.FloorToInt(containerWidth / ItemWidth) + 2;
            if (ContentContainer.childCount >= count) return;
            
            for (int i = ContentContainer.childCount; i < count; i++)
            {
                var item = CreateItem();
                ContentContainer.Add(item);
            }
        }

        VisualElement CreateItem() => ItemTemplate.Instantiate();

        protected override void OnItemsSourceChanged()
        {
            JumpToIndex(SelectedIndex); 
        }
        
        private void JumpToIndex(int index)
        {
            if (index < 0) return;

            SelectedIndex = index;
            var childCount = ContentContainer.childCount;
            int itemIndex = SelectedIndex - Mathf.FloorToInt(childCount / 2.0f);

            for (int i = 0; i < childCount; i++, itemIndex++)
            {
                BindTileData(ContentContainer[i], GetItem(itemIndex));
            } 

            CenterContent();
        }

        private void JumpNext()
        {
            int transferIndex = SelectedIndex + Mathf.CeilToInt(ContentContainer.childCount / 2.0f);
            BindTileData(ContentContainer[0], GetItem(transferIndex));
            ContentContainer[0].BringToFront();
            SelectedIndex++;
        }

        private void JumpPrevious()
        {
            int transferIndex = SelectedIndex - Mathf.FloorToInt(ContentContainer.childCount / 2.0f) - 1;
            BindTileData(ContentContainer[ContentContainer.childCount - 1], GetItem(transferIndex));
            ContentContainer[ContentContainer.childCount - 1].SendToBack();
            SelectedIndex--;
        }

        void CenterContent()
        {
            Vector2 targetOffset = GetCenteringOffset();
            
            Vector3 position = ContentContainer.transform.position;
            position.x = -targetOffset.x;
            position.y = -targetOffset.y;
            ContentContainer.transform.position = position;
        }

        Vector2 GetCenteringOffset()
        {
            Vector2 targetOffset = new Vector2((ContentContainer.layout.width - ContentViewport.layout.width) / 2.0f, 0);
            if (ContentContainer.childCount % 2.0f == 0)
                targetOffset.x += ItemWidth/2f;
            return targetOffset;
        }

        private void BindTileData(VisualElement visualElement, INOBindingContext dataCollection)
        {
            dataCollection?.Bind(visualElement);
        }
 
        private void SetScrollViewMode(ToggleSelectorDirection mode)
        {
            _Mode = mode;
            InputContainer.RemoveFromClassList(NOUSS.ToggleSelectorInputContainerVerticalClass);
            InputContainer.RemoveFromClassList(NOUSS.ToggleSelectorInputContainerHorizontalClass);
            ContentViewport.RemoveFromClassList(NOUSS.ToggleSelectorViewportVerticalClass);
            ContentViewport.RemoveFromClassList(NOUSS.ToggleSelectorViewportHorizontalClass);
            ContentContainer.RemoveFromClassList(NOUSS.ToggleSelectorContentContainerVerticalClass);
            ContentContainer.RemoveFromClassList(NOUSS.ToggleSelectorContentContainerHorizontalClass);
            switch (mode)
            {
                case ToggleSelectorDirection.Vertical:
                    InputContainer.AddToClassList(NOUSS.ToggleSelectorInputContainerVerticalClass);
                    ContentViewport.AddToClassList(NOUSS.ToggleSelectorViewportVerticalClass);
                    ContentContainer.AddToClassList(NOUSS.ToggleSelectorContentContainerVerticalClass);
                    break;
                case ToggleSelectorDirection.Horizontal:
                    InputContainer.AddToClassList(NOUSS.ToggleSelectorInputContainerHorizontalClass);
                    ContentViewport.AddToClassList(NOUSS.ToggleSelectorViewportHorizontalClass);
                    ContentContainer.AddToClassList(NOUSS.ToggleSelectorContentContainerHorizontalClass);
                    break;
            }
        }
    }
}
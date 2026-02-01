using System;
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
        
        private event Action RefreshGeometryEvent;
        
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

            var scroller = new NOContentScroller(JumpNext, JumpPrevious);
            ContentContainer = new VisualElement { name = "toggle-selector-content-container", usageHints = UsageHints.GroupTransform };
            ContentContainer.AddToClassList(NOUSS.ToggleSelectorContentContainerClass);
            ContentContainer.AddManipulator(scroller);
            RefreshGeometryEvent += scroller.UpdatePosition;
            
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

            ItemsSourceChangedEvent += RefreshTiles;
            SelectionChangedEvent += RefreshTiles;
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
            RefreshTiles();
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

            int count = Mathf.CeilToInt(containerWidth / ItemWidth);
            count += 1 + count%2;
            if (ContentContainer.childCount >= count) return;
            
            for (int i = ContentContainer.childCount; i < count; i++)
            {
                var item = CreateItem();
                ContentContainer.Add(item);
            }
        }

        VisualElement CreateItem() => ItemTemplate.Instantiate();

        private void RefreshTiles()
        {
            if (SelectedIndex < 0) return;

            var pooledTiles = ContentContainer.childCount;
            int itemIndex = SelectedIndex - Mathf.FloorToInt(pooledTiles / 2.0f);

            for (int i = 0; i < pooledTiles; i++, itemIndex++)
            {
                BindTileData(ContentContainer[i], GetItem(itemIndex));
            }
            
            RefreshGeometryEvent?.Invoke();
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
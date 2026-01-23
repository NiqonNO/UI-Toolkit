using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
    [UxmlElement]
    public partial class NOToggleSelector : BaseField<NOViewModelListState>
    {
        private readonly VisualElement InputContainer;
        private readonly Button NextButton;
        private readonly Button PreviousButton;
        private readonly VisualElement ContentContainer;
        private readonly VisualElement ContentViewport;
        
        private ScrollViewMode _Mode;
        [UxmlAttribute]
        public ScrollViewMode Mode
        {
            get => _Mode;
            set => SetScrollViewMode(value);
        }

        private int CurrentIndex
        {
            get => value?.SelectedIndex??0;
            set
            {
                if (this.value == null) return;
                this.value.SelectedIndex = value;
            }
        }

        public NOToggleSelector() : this(string.Empty)
        {
        }

        public NOToggleSelector(string label) : base(label, new VisualElement())
        {
            AddToClassList(NOUSS.ItemSelectorClass);
            labelElement.AddToClassList(NOUSS.ToggleSelectorLabelClass);
            InputContainer = this.Q(className: inputUssClassName);
            InputContainer.AddToClassList(NOUSS.ToggleSelectorInputContainerClass);

            PreviousButton = new Button(ScrollToPrevious) { name = "toggle-selector-previous",  };
            PreviousButton.AddToClassList(NOUSS.ToggleSelectorButtonClass);
            PreviousButton.AddToClassList(NOUSS.ToggleSelectorButtonPreviousClass);
            
            NextButton = new Button(ScrollToNext) { name = "toggle-selector-next", };
            NextButton.AddToClassList(NOUSS.ToggleSelectorButtonClass);
            NextButton.AddToClassList(NOUSS.ToggleSelectorButtonNextClass);

            ContentViewport = new VisualElement { name = "toggle-selector-content-viewport", pickingMode = PickingMode.Ignore, };
            ContentViewport.AddToClassList(NOUSS.ToggleSelectorViewportClass);
            ContentViewport.RegisterCallback<GeometryChangedEvent>(UpdatePositions);
            
            var scroller = new NOContentScroller();
            ContentContainer = new VisualElement { name = "toggle-selector-content-container", usageHints = UsageHints.GroupTransform, /*disableClipping = true,*/ };
            ContentContainer.AddToClassList(NOUSS.ToggleSelectorContentContainerClass);
            ContentContainer.AddManipulator(scroller);

            
            InputContainer.Add(PreviousButton);
            InputContainer.Add(ContentViewport);
            ContentViewport.Add(ContentContainer);
            InputContainer.Add(NextButton);
        }

        private void UpdatePositions(GeometryChangedEvent evt) => UpdatePositions();

        private void UpdatePositions()
        {
            ResizeItemPool();
            JumpToIndex(CurrentIndex);
        }

        private void ResizeItemPool()
        {
            float itemWidth = 200;
            float itemSpacing = 10;
            float containerWidth = ContentViewport.contentRect.width;

            int count = Mathf.FloorToInt(containerWidth / (itemWidth + itemSpacing)) + 2;
            if (ContentContainer.childCount >= count) return;
            
            for (int i = ContentContainer.childCount; i < count; i++)
            {
                var item = CreateItem();
                ContentContainer.Add(item);
            }
        }

        VisualElement CreateItem()
        {
            var item = new VisualElement
            {
                style =
                {
                    width = 200,
                    height = 150,
                    marginRight = 5,
                    marginLeft = 5,
                    backgroundColor = Color.white,
                }
            };

            var label = new Label
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                    color = Color.black,
                    fontSize = 20,
                },
            };

            item.Add(label);
            return item;
        }

        private void ScrollToNext() => ScrollToIndex(CurrentIndex + 2);
        private void ScrollToPrevious() => ScrollToIndex(CurrentIndex - 2);

        private void JumpToIndex(int index)
        {
            if (value == null)
                return; 
            
            var childCount = ContentContainer.childCount;
            if (childCount == 0) return;

            CurrentIndex = (int)Mathf.Repeat(index, value.Items.Count);
            int itemIndex = index - Mathf.FloorToInt(childCount / 2.0f);

            for (int i = 0; i < childCount; i++, itemIndex++)
            {
                SetData(ContentContainer[i], value.Items[(int)Mathf.Repeat(itemIndex, value.Items.Count)]);
            } 

            //CenterContent();
        }

        private void ScrollToIndex(int newIndex)
        {
            if (value == null)
                return;
            
            newIndex = (int)Mathf.Repeat(newIndex, value.Items.Count);

            int ascending = (newIndex - CurrentIndex + value.Items.Count) % value.Items.Count;
            int descending = (CurrentIndex - newIndex + value.Items.Count) % value.Items.Count;

            if (ascending < descending)
                ScrollStep += ascending;
            else
                ScrollStep -= descending;
        }

        private void SetData(VisualElement visualElement, INOViewModel dataCollection)
        {
            visualElement.Q<Label>().text = dataCollection.DisplayName;
            visualElement.style.backgroundColor = dataCollection.DisplayColor;
        }
 
        private void SetScrollViewMode(ScrollViewMode mode)
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
                case ScrollViewMode.Vertical:
                    InputContainer.AddToClassList(NOUSS.ToggleSelectorInputContainerVerticalClass);
                    ContentViewport.AddToClassList(NOUSS.ToggleSelectorViewportVerticalClass);
                    ContentContainer.AddToClassList(NOUSS.ToggleSelectorContentContainerVerticalClass);
                    break;
                case ScrollViewMode.Horizontal:
                    InputContainer.AddToClassList(NOUSS.ToggleSelectorInputContainerHorizontalClass);
                    ContentViewport.AddToClassList(NOUSS.ToggleSelectorViewportHorizontalClass);
                    ContentContainer.AddToClassList(NOUSS.ToggleSelectorContentContainerHorizontalClass);
                    break;
            }
        }
    }
}
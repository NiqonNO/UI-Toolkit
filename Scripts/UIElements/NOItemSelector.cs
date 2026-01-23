using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
    [Serializable]
    public class NODataProvider
    {
        [field: SerializeField] public string DisplayName { get; set; }
        [field: SerializeField] public Color DisplayColor { get; set; }
    }

    [UxmlElement]
    public partial class NOItemSelector : BaseField<ToggleButtonGroupState>
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

        private List<NODataProvider> DataCollection = new()
        {
            new NODataProvider { DisplayName = "0 | 0", DisplayColor = Color.red },
            new NODataProvider { DisplayName = "1 | 1", DisplayColor = Color.green },
            new NODataProvider { DisplayName = "2 | 2", DisplayColor = Color.blue },
            new NODataProvider { DisplayName = "3 | 3", DisplayColor = Color.cyan },
            new NODataProvider { DisplayName = "4 | 4", DisplayColor = Color.magenta },
            new NODataProvider { DisplayName = "5 | 5", DisplayColor = Color.yellow }
        };

        private int _ScrollStep;

        public int ScrollStep
        {
            get => _ScrollStep;
            set
            {
                _ScrollStep = value;
                if (Scheduler != null)
                {
                    Scheduler.Resume();
                    return;
                }

                CreateScheduler();
            }
        }

        private int _CurrentIndex;
        private IVisualElementScheduledItem Scheduler;

        public NOItemSelector() : this(string.Empty)
        {
        }

        public NOItemSelector(string label) : base(label, new VisualElement())
        {
            AddToClassList(NOUSS.ItemSelectorClass);
            labelElement.AddToClassList(NOUSS.ItemSelectorLabelClass);
            InputContainer = this.Q(className: BaseField<ToggleButtonGroupState>.inputUssClassName);
            InputContainer.AddToClassList(NOUSS.ItemSelectorInputContainerClass);

            PreviousButton = new Button(ScrollToPrevious)
            {
                name = "item-selector-previous",
                text = "<"
            };
            PreviousButton.AddToClassList(NOUSS.ItemSelectorButtonClass);
            PreviousButton.AddToClassList(NOUSS.ItemSelectorButtonPreviousClass);
            InputContainer.Add(PreviousButton);

            ContentViewport = new VisualElement
            {
                name = "item-selector-content-viewport",
                pickingMode = PickingMode.Ignore,
            };
            ContentViewport.AddToClassList(NOUSS.ItemSelectorViewportClass);
            ContentViewport.RegisterCallback<GeometryChangedEvent>(UpdatePositions);
            InputContainer.Add(ContentViewport);

            NextButton = new Button(ScrollToNext)
            {
                name = "item-selector-next",
                text = ">"
            };
            NextButton.AddToClassList(NOUSS.ItemSelectorButtonClass);
            NextButton.AddToClassList(NOUSS.ItemSelectorButtonNextClass);
            InputContainer.Add(NextButton);

            ContentContainer = new VisualElement
            {
                name = "item-selector-content-container",
                usageHints = UsageHints.GroupTransform,
                //disableClipping = true,
            };
            ContentContainer.AddToClassList(NOUSS.ItemSelectorContentContainerClass);
            ContentViewport.Add(ContentContainer);

        }

        private void UpdatePositions(GeometryChangedEvent evt) => UpdatePositions();

        private void UpdatePositions()
        {
            ResizeItemPool();
            JumpToIndex(_CurrentIndex);
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
                var item = CreateItem(i.ToString());
                ContentContainer.Add(item);
            }
        }

        VisualElement CreateItem(string name)
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
                text = name,
            };

            item.Add(label);
            return item;
        }

        private void CreateScheduler()
        {
            Scheduler = schedule.Execute(Scroll).Every(0).Until(() => _ScrollStep == 0);
        }

        private void ScrollToNext() => ScrollToIndex(_CurrentIndex + 2);
        private void ScrollToPrevious() => ScrollToIndex(_CurrentIndex - 2);

        private void JumpToIndex(int index)
        {
            var childCount = ContentContainer.childCount;
            if (childCount == 0) return;

            _CurrentIndex = (int)Mathf.Repeat(index, DataCollection.Count);
            int itemIndex = index - Mathf.FloorToInt(childCount / 2.0f);

            for (int i = 0; i < childCount; i++, itemIndex++)
            {
                SetData(ContentContainer[i], DataCollection[(int)Mathf.Repeat(itemIndex, DataCollection.Count)]);
            }

            CenterContent();
        }

        private void ScrollToIndex(int newIndex)
        {
            newIndex = (int)Mathf.Repeat(newIndex, DataCollection.Count);

            int ascending = (newIndex - _CurrentIndex + DataCollection.Count) % DataCollection.Count;
            int descending = (_CurrentIndex - newIndex + DataCollection.Count) % DataCollection.Count;

            if (ascending < descending)
                ScrollStep += ascending;
            else
                ScrollStep -= descending;
        }

        private void Scroll(TimerState obj)
        {
            if (_ScrollStep > 0)
            {
                int transferIndex = _CurrentIndex + Mathf.CeilToInt(ContentContainer.childCount / 2.0f);
                SetData(ContentContainer[0], DataCollection[(int)Mathf.Repeat(transferIndex, DataCollection.Count)]);
                ContentContainer[0].BringToFront();

                _ScrollStep--;
                _CurrentIndex = (int)Mathf.Repeat(++_CurrentIndex, DataCollection.Count);
            }
            else
            {
                int transferIndex = _CurrentIndex - Mathf.FloorToInt(ContentContainer.childCount / 2.0f) - 1;
                SetData(ContentContainer[ContentContainer.childCount - 1],
                    DataCollection[(int)Mathf.Repeat(transferIndex, DataCollection.Count)]);
                ContentContainer[ContentContainer.childCount - 1].SendToBack();

                _ScrollStep++;
                _CurrentIndex = (int)Mathf.Repeat(--_CurrentIndex, DataCollection.Count);
            }

            CenterContent();
        }

        private void SetData(VisualElement visualElement, NODataProvider dataCollection)
        {
            visualElement.Q<Label>().text = dataCollection.DisplayName;
            visualElement.style.backgroundColor = dataCollection.DisplayColor;
        }

        void CenterContent()
        {
            Vector2 targetOffset =
                new Vector2((ContentContainer.layout.width - ContentViewport.layout.width) / 2.0f, 0);
            if (ContentContainer.childCount % 2.0f == 0)
                targetOffset.x += 105f;
            
            Vector3 position = ContentContainer.transform.position;
            position.x = -targetOffset.x;
            position.y = -targetOffset.y;
            ContentContainer.transform.position = position;
        }
 
        private void SetScrollViewMode(ScrollViewMode mode)
        {
            _Mode = mode;
            InputContainer.RemoveFromClassList(NOUSS.ItemSelectorInputContainerVerticalClass);
            InputContainer.RemoveFromClassList(NOUSS.ItemSelectorInputContainerHorizontalClass);
            ContentViewport.RemoveFromClassList(NOUSS.ItemSelectorViewportVerticalClass);
            ContentViewport.RemoveFromClassList(NOUSS.ItemSelectorViewportHorizontalClass);
            ContentContainer.RemoveFromClassList(NOUSS.ItemSelectorContentContainerVerticalClass);
            ContentContainer.RemoveFromClassList(NOUSS.ItemSelectorContentContainerHorizontalClass);
            switch (mode)
            {
                case ScrollViewMode.Vertical:
                    InputContainer.AddToClassList(NOUSS.ItemSelectorInputContainerVerticalClass);
                    ContentViewport.AddToClassList(NOUSS.ItemSelectorViewportVerticalClass);
                    ContentContainer.AddToClassList(NOUSS.ItemSelectorContentContainerVerticalClass);
                    break;
                case ScrollViewMode.Horizontal:
                    InputContainer.AddToClassList(NOUSS.ItemSelectorInputContainerHorizontalClass);
                    ContentViewport.AddToClassList(NOUSS.ItemSelectorViewportHorizontalClass);
                    ContentContainer.AddToClassList(NOUSS.ItemSelectorContentContainerHorizontalClass);
                    break;
            }
        }
    }
}
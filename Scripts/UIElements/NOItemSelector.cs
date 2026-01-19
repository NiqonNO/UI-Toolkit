using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
    [Serializable]
    public class NODataProvider
    {
        [field: SerializeField]
        public string DisplayName { get; set; }
        [field: SerializeField]
        public Color DisplayColor { get; set; }
    }
    
    [UxmlElement]
    public partial class NOItemSelector : ScrollView
    {
        private readonly VisualElement ScrollContainer;
        private readonly Button NextButton;
        private readonly Button PreviousButton;
        private VisualElement ContentContainer => base.contentContainer;
        private VisualElement ViewportContainer => base.contentViewport;

        private List<NODataProvider> DataCollection = new()
        {
            new NODataProvider { DisplayName = "0 | 0",  DisplayColor = Color.red }, 
            new NODataProvider { DisplayName = "1 | 1",  DisplayColor = Color.green }, 
            new NODataProvider { DisplayName = "2 | 2",  DisplayColor = Color.blue }, 
            new NODataProvider { DisplayName = "3 | 3",  DisplayColor = Color.cyan }, 
            new NODataProvider { DisplayName = "4 | 4",  DisplayColor = Color.magenta },
            new NODataProvider { DisplayName = "5 | 5",  DisplayColor = Color.yellow }
        };
        private List<VisualElement> ItemPool = new();
        
        private int _currentIndex;

        public NOItemSelector()
        {
            ScrollContainer = this.Q(className: ScrollView.contentAndVerticalScrollUssClassName);
            
            horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            verticalScrollerVisibility = ScrollerVisibility.Hidden;
            
            mode = ScrollViewMode.Horizontal;
            elasticity = 0;
            
            PreviousButton = new Button(ScrollToPrevious)
            {
                text = "<"
            };
            PreviousButton.AddToClassList("item-selector__button");
            PreviousButton.AddToClassList("item-selector__button-previous");
            ScrollContainer.Insert(0, PreviousButton);
            
            NextButton = new Button(ScrollToNext)
            {
                text = ">"
            };
            NextButton.AddToClassList("item-selector__button");
            NextButton.AddToClassList("item-selector__button-next");
            
            ScrollContainer.Add(NextButton);
            ViewportContainer.RegisterCallback<GeometryChangedEvent>(UpdatePositions);
        }

        private void UpdatePositions(GeometryChangedEvent evt) => UpdatePositions();
        private void UpdatePositions()
        {
            ResizeItemPool();
            JumpToIndex(_currentIndex);
        }

        private void ResizeItemPool()
        {
            float itemWidth = 200;
            float itemSpacing = 10;
            float containerWidth = ViewportContainer.contentRect.width;
            
            int count = Mathf.FloorToInt(containerWidth / (itemWidth + itemSpacing)) + 2;
            if (ItemPool.Count >= count) return;
            
            for (int i = ItemPool.Count; i < count; i++)
            {
                var item = CreateItem(i.ToString());
                ContentContainer.Add(item);
                ItemPool.Add(item);
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

        private void ScrollToNext() => ScrollToIndex(_currentIndex + 1);
        private void ScrollToPrevious() => ScrollToIndex(_currentIndex - 1);
        private void JumpToIndex(int index)
        {
            var childCount = contentContainer.childCount;
            if (childCount == 0) return; 

            _currentIndex = (int)Mathf.Repeat(index, DataCollection.Count);
            int itemIndex = index - Mathf.FloorToInt(childCount/2.0f);
            
            for (int i = 0; i < childCount; i++, itemIndex++)
            {
                SetData(contentContainer[i], DataCollection[(int)Mathf.Repeat(itemIndex, DataCollection.Count)]);
            }
            CenterContent();
            
            
            /*this.schedule.Execute(() =>
            {
                scrollOffset = Vector2.Lerp(scrollOffset, targetOffset, Time.deltaTime * _snapSpeed);
            }).Every(0);*/
        }

        private void ScrollToIndex(int newIndex)
        {
            newIndex = (int)Mathf.Repeat(newIndex, DataCollection.Count);

            int ascending = (newIndex - _currentIndex + DataCollection.Count) % DataCollection.Count;
            int descending = (_currentIndex - newIndex + DataCollection.Count) % DataCollection.Count;
 
            if (ascending < descending)
            {
                int transferIndex = _currentIndex + Mathf.CeilToInt(ItemPool.Count / 2.0f);
                for (int i = 0, j = transferIndex; i < ascending; i++, j--) 
                {
                    SetData(contentContainer[0], DataCollection[(int)Mathf.Repeat(j, DataCollection.Count)]);
                    contentContainer[0].BringToFront();
                }
            }
            else
            {
                int transferIndex = _currentIndex - Mathf.CeilToInt(ItemPool.Count / 2.0f);
                for (int i = 0, j = transferIndex; i < descending; i++, j++)
                { 
                    SetData(contentContainer[ItemPool.Count - 1], DataCollection[(int)Mathf.Repeat(j, DataCollection.Count)]);
                    contentContainer[ItemPool.Count - 1].SendToBack();
                }
            }

            _currentIndex = newIndex;
        }

        private void SetData(VisualElement visualElement, NODataProvider dataCollection)
        {
            visualElement.Q<Label>().text = dataCollection.DisplayName;
            visualElement.style.backgroundColor = dataCollection.DisplayColor;
        }

        void CenterContent()
        {
            Vector2 targetOffset = new Vector2((contentContainer.layout.width-ViewportContainer.layout.width) / 2.0f, 0);
            scrollOffset = targetOffset;
        }
        /*

        [UxmlAttribute] 
        private VisualTreeAsset ItemTemplate;
        
        public NOItemSelector() : base()
        {
            AddToClassList("item-selector");
            style.flexDirection = FlexDirection.Row;
            
            NextButton = new Button();
            NextButton.AddToClassList("item-selector__button");
            NextButton.AddToClassList("item-selector__button-next");
            Add(NextButton);

            ContentContainer = new VisualElement()
            {
                style = { flexGrow = 1 },
            };
            ContentContainer.AddToClassList("item-selector__container");
            Add(ContentContainer);
            
            PreviousButton = new Button();
            PreviousButton.AddToClassList("item-selector__button");
            PreviousButton.AddToClassList("item-selector__button-previous");
            Add(PreviousButton);
            
            
        }*/
    }
}
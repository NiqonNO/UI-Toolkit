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

        private readonly NOContentScroller ScrollerManipulator;

        [UxmlAttribute]
        private ScrollDirection Direction { get => _Direction; set { _Direction = value; ScrollerManipulator?.SetDirection(_Direction); UpdateScrollDirection(); } }
        private ScrollDirection _Direction;
        
        [UxmlAttribute, Range(0.01f, 10.0f)]
        private float TileAspectRatio { get => _TileAspectRatio; set { _TileAspectRatio = value; ResizeTiles(); } }
        private float _TileAspectRatio;
        
        [UxmlAttribute]
        private NOEase AutoScrollEase { get => _AutoScrollEase; set { _AutoScrollEase = value; ScrollerManipulator?.SetCenteringEase(_AutoScrollEase); }  }
        private NOEase _AutoScrollEase;
        
        [UxmlAttribute] 
        private float AutoScrollDuration { get => _AutoScrollDuration; set { _AutoScrollDuration = value; ScrollerManipulator?.SetCenteringDuration(_AutoScrollDuration); }  }
        private float _AutoScrollDuration;
        
        [UxmlAttribute]
        private VisualTreeAsset ItemTemplate { get => _ItemTemplate; set { _ItemTemplate = value; Rebuild(); } }
        private VisualTreeAsset _ItemTemplate;

        private float ViewportMainSize => Direction == ScrollDirection.Vertical
            ? ContentViewport.resolvedStyle.height : ContentViewport.resolvedStyle.width;
        private float ViewportCrossSize => Direction == ScrollDirection.Vertical
            ? ContentViewport.resolvedStyle.width : ContentViewport.resolvedStyle.height;
        private float TilePixelSize => ViewportCrossSize * TileAspectRatio;

        public NOToggleSelector() : this(string.Empty) {  }

        public NOToggleSelector(string label, 
            float tileAspectRatio = 1.0f, ScrollDirection scrollDirection = ScrollDirection.Vertical,
            NOEase centeringEase = NOEase.Linear, float centeringDuration = 0.3f)
        {
            _TileAspectRatio = tileAspectRatio;
            _Direction = scrollDirection;
            _AutoScrollEase = centeringEase;
            _AutoScrollDuration = centeringDuration;
            
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

            ScrollerManipulator = new NOContentScroller(JumpNext, JumpPrevious, TilePixelSize, scrollDirection, centeringEase, centeringDuration);
            ContentContainer = new VisualElement { name = "toggle-selector-content-container", usageHints = UsageHints.GroupTransform };
            ContentContainer.AddToClassList(NOUSS.ToggleSelectorContentContainerClass);
            ContentContainer.AddManipulator(ScrollerManipulator);
            
            PreviousButton = new Button(ScrollerManipulator.ScrollToPrevious) { name = "toggle-selector-previous",  };
            PreviousButton.AddToClassList(NOUSS.ToggleSelectorButtonClass);
            PreviousButton.AddToClassList(NOUSS.ToggleSelectorButtonPreviousClass);
            
            NextButton = new Button(ScrollerManipulator.ScrollToNext) { name = "toggle-selector-next", };
            NextButton.AddToClassList(NOUSS.ToggleSelectorButtonClass);
            NextButton.AddToClassList(NOUSS.ToggleSelectorButtonNextClass);
            
            InputContainer.Add(PreviousButton);
            InputContainer.Add(ContentViewport);
            ContentViewport.Add(ContentContainer);
            InputContainer.Add(NextButton);
            
            UpdateScrollDirection();
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
            int count = Mathf.Max(0, Mathf.CeilToInt(ViewportMainSize / TilePixelSize));
            count += 1 + count%2;

            if (ContentContainer.childCount < count) SpawnTile();
            else if (ContentContainer.childCount > count) RemoveTiles();
            ResizeTiles();
            return;
           
            void SpawnTile()
            {
                for (int i = ContentContainer.childCount; i < count; i++)
                {
                    var item = ItemTemplate.Instantiate();
                    item.AddToClassList(NOUSS.ToggleSelectorTile);
                    ContentContainer.Add(item);
                }
            }
            void RemoveTiles()
            {
                for (int i = ContentContainer.childCount; i > count; i--)
                {
                    ContentContainer.RemoveAt(0);
                }
            }
        }
        private void ResizeTiles()
        {
            if (ContentContainer.childCount == 0) return;
            
            for (int i = 0; i < ContentContainer.childCount; i++)
            {
                switch (Direction)
                {
                    case ScrollDirection.Vertical:
                        ContentContainer[i].style.width = Length.Percent(100);
                        ContentContainer[i].style.height = TilePixelSize;
                        break;
                    case ScrollDirection.Horizontal:
                        ContentContainer[i].style.width = TilePixelSize;
                        ContentContainer[i].style.height = Length.Percent(100);
                        break;
                }
            }
            ScrollerManipulator?.SetTileSize(TilePixelSize);
            CenterOnSelection();
        }

        protected override void Refresh() => RefreshTiles();
        private void RefreshTiles()
        {
            if (ContentContainer.childCount == 0) return;
            
            var pooledTiles = ContentContainer.childCount;
            int itemIndex = SelectedIndex - Mathf.FloorToInt(pooledTiles / 2.0f);

            for (int i = 0; i < pooledTiles; i++, itemIndex++)
            {
                BindTileData(ContentContainer[i], GetItem(itemIndex));
            }

            CenterOnSelection();
        }

        private void CenterOnSelection() => ScrollerManipulator?.UpdatePosition();
        
        private void JumpNext()
        {
            int transferIndex = SelectedIndex + Mathf.CeilToInt(ContentContainer.childCount / 2.0f);
            BindTileData(ContentContainer[ContentContainer.childCount - 1], GetItem(transferIndex));
            SetIndex(SelectedIndex + 1);
        }

        private void JumpPrevious()
        {
            int transferIndex = SelectedIndex - Mathf.FloorToInt(ContentContainer.childCount / 2.0f) - 1;
            BindTileData(ContentContainer[0], GetItem(transferIndex));
            SetIndex(SelectedIndex - 1);
        }

        private void BindTileData(VisualElement visualElement, INOBindingContext dataCollection)
        {
            dataCollection?.Bind(visualElement);
        }
 
        private void UpdateScrollDirection()
        {
            InputContainer.RemoveFromClassList(NOUSS.ToggleSelectorInputContainerVerticalClass);
            InputContainer.RemoveFromClassList(NOUSS.ToggleSelectorInputContainerHorizontalClass);
            ContentViewport.RemoveFromClassList(NOUSS.ToggleSelectorViewportVerticalClass);
            ContentViewport.RemoveFromClassList(NOUSS.ToggleSelectorViewportHorizontalClass);
            ContentContainer.RemoveFromClassList(NOUSS.ToggleSelectorContentContainerVerticalClass);
            ContentContainer.RemoveFromClassList(NOUSS.ToggleSelectorContentContainerHorizontalClass);
            switch (Direction)
            {
                case ScrollDirection.Vertical:
                    InputContainer.AddToClassList(NOUSS.ToggleSelectorInputContainerVerticalClass);
                    ContentViewport.AddToClassList(NOUSS.ToggleSelectorViewportVerticalClass);
                    ContentContainer.AddToClassList(NOUSS.ToggleSelectorContentContainerVerticalClass);
                    break;
                case ScrollDirection.Horizontal:
                    InputContainer.AddToClassList(NOUSS.ToggleSelectorInputContainerHorizontalClass);
                    ContentViewport.AddToClassList(NOUSS.ToggleSelectorViewportHorizontalClass);
                    ContentContainer.AddToClassList(NOUSS.ToggleSelectorContentContainerHorizontalClass);
                    break;
            }

            ResizeTiles();
        }
    }
}
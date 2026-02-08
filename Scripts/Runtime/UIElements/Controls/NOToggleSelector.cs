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

        private bool ApplyScheduled;
        private bool PositionDirty;
        private bool ScrollerDirty;
        private bool LayoutDirty;
        private bool PoolDirty;

        private ScrollDirection _Direction;
        [UxmlAttribute]
        private ScrollDirection Direction
        {
            get => _Direction;
            set
            {
                if (_Direction == value) return;
                _Direction = value;
                MarkDirty(scroller: true, layout: true);
            }
        }

        private float _TileAspectRatio;
        [UxmlAttribute, Range(0.01f, 10.0f)]
        private float TileAspectRatio
        {
            get => _TileAspectRatio;
            set
            {
                if (Mathf.Approximately(_TileAspectRatio, value)) return;
                _TileAspectRatio = value;
                MarkDirty(scroller: true, layout: true);
            }
        }

        private NOEase _AutoScrollEase;
        [UxmlAttribute]
        private NOEase AutoScrollEase
        {
            get => _AutoScrollEase;
            set
            {
                if (_AutoScrollEase == value) return;
                _AutoScrollEase = value;
                MarkDirty(scroller: true);
            }
        }
        
        private float _AutoScrollDuration;
        [UxmlAttribute]
        private float AutoScrollDuration
        {
            get => _AutoScrollDuration;
            set
            {
                if (Mathf.Approximately(_AutoScrollDuration, value)) return;
                _AutoScrollDuration = value;
                MarkDirty(scroller: true);
            }
        }

        private VisualTreeAsset _ItemTemplate;
        [UxmlAttribute]
        private VisualTreeAsset ItemTemplate
        {
            get => _ItemTemplate;
            set
            {
                if (_ItemTemplate == value) return;
                _ItemTemplate = value;
                ClearPool();
                MarkDirty(pool: true);
            }
        }

        private float ViewportMainSize => Direction == ScrollDirection.Vertical
            ? ContentViewport.contentRect.height : ContentViewport.contentRect.width;
        private float ViewportCrossSize => Direction == ScrollDirection.Vertical
            ? ContentViewport.contentRect.width : ContentViewport.contentRect.height;
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

            styleSheets.Add(NOUSS.GetStyleSheet(NOUSS.ToggleSelectorStylePath));
            AddToClassList(NOUSS.ItemSelectorClass);

            if (!string.IsNullOrEmpty(label))
            {
                LabelElement = new Label(label);
                LabelElement.AddToClassList(NOUSS.ToggleSelectorLabelClass);
                Add(LabelElement);
            }

            InputContainer = new VisualElement();
            InputContainer.AddToClassList(NOUSS.ToggleSelectorInputContainerClass);

            ContentViewport = new VisualElement { name = "toggle-selector-content-viewport", pickingMode = PickingMode.Ignore, };
            ContentViewport.AddToClassList(NOUSS.ToggleSelectorViewportClass);
            ContentViewport.RegisterCallback<GeometryChangedEvent>(OnViewportGeometryChanged);

            ScrollerManipulator = new NOContentScroller(JumpNext, JumpPrevious, TilePixelSize, scrollDirection,
                centeringEase, centeringDuration);
            ContentContainer = new VisualElement { name = "toggle-selector-content-container", usageHints = UsageHints.GroupTransform };
            ContentContainer.AddToClassList(NOUSS.ToggleSelectorContentContainerClass);
            ContentContainer.RegisterCallback<GeometryChangedEvent>(OnContainerGeometryChanged);
            ContentContainer.AddManipulator(ScrollerManipulator);

            PreviousButton = new Button(ScrollerManipulator.ScrollToPrevious) { name = "toggle-selector-previous", };
            PreviousButton.AddToClassList(NOUSS.ToggleSelectorButtonClass);
            PreviousButton.AddToClassList(NOUSS.ToggleSelectorButtonPreviousClass);

            NextButton = new Button(ScrollerManipulator.ScrollToNext) { name = "toggle-selector-next", };
            NextButton.AddToClassList(NOUSS.ToggleSelectorButtonClass);
            NextButton.AddToClassList(NOUSS.ToggleSelectorButtonNextClass);

            Add(InputContainer);
            InputContainer.Add(PreviousButton);
            InputContainer.Add(ContentViewport);
            ContentViewport.Add(ContentContainer);
            InputContainer.Add(NextButton);

            //MarkDirty(true, true);
        }

        private void OnContainerGeometryChanged(GeometryChangedEvent evt)
        {
            if (evt.oldRect.size == evt.newRect.size) return;
            MarkDirty(position: true, layout: true);
        }
        private void OnViewportGeometryChanged(GeometryChangedEvent evt)
        {
            if (evt.oldRect.size == evt.newRect.size) return;
            MarkDirty(position: true, pool: true);
        }
        
        private void MarkDirty(bool position = false, bool scroller = false, bool layout = false, bool pool = false)
        {
            PositionDirty |= position;
            ScrollerDirty |= scroller;
            LayoutDirty |= layout;
            PoolDirty |= pool;

            if (ApplyScheduled)
                return;

            ApplyScheduled = true;
            schedule.Execute(ApplyState);
        }
        
        private void ApplyState()
        {
            ApplyScheduled = false;
            
            if (PoolDirty)
            {
                RebuildPool();
                PoolDirty = false;
            }

            if (LayoutDirty)
            {
                ApplyLayout();
                LayoutDirty = false;
            }
            
            if (ScrollerDirty)
            {
                UpdateScroller();
                ScrollerDirty = false;
            }
            
            if(PositionDirty)
            {
                ScrollerManipulator.UpdatePosition();
                PositionDirty = false;
            }
        }

        private void ClearPool()
        {
            while (ContentContainer.childCount > 0)
            {
                ContentContainer.RemoveAt(0);
            }
        }
        
        private void RebuildPool()
        {
            EnsurePoolSize();
            BindVisibleItems();
        }
        
        private void EnsurePoolSize()
        {
            int required = ComputeRequiredTileCount();
            int current = ContentContainer.childCount;

            while (current < required)
            {
                var item = ItemTemplate.Instantiate();
                item.AddToClassList(NOUSS.ToggleSelectorTile);
                ContentContainer.Add(item);
                current++;
            }

            while (current > required)
            {
                ContentContainer.RemoveAt(0);
                current--;
            }

            return;

            int ComputeRequiredTileCount()
            {
                if (ItemTemplate == null) return 0;
                int count = Mathf.Max(0, Mathf.CeilToInt(ViewportMainSize / TilePixelSize));
                return count + 1 + count % 2;
            }
        }
        
        private void ApplyLayout()
        {
            ApplyDirectionClasses();
            ApplyTileSizes();
        }
        
        private void ApplyDirectionClasses()
        {
            bool vertical = Direction == ScrollDirection.Vertical;
            
            InputContainer.EnableInClassList(NOUSS.ToggleSelectorInputContainerVerticalClass, vertical);
            InputContainer.EnableInClassList(NOUSS.ToggleSelectorInputContainerHorizontalClass, !vertical);

            ContentViewport.EnableInClassList(NOUSS.ToggleSelectorViewportVerticalClass, vertical);
            ContentViewport.EnableInClassList(NOUSS.ToggleSelectorViewportHorizontalClass, !vertical);

            ContentContainer.EnableInClassList(NOUSS.ToggleSelectorContentContainerVerticalClass, vertical);
            ContentContainer.EnableInClassList(NOUSS.ToggleSelectorContentContainerHorizontalClass, !vertical);
        }
        
        private void ApplyTileSizes()
        {
            for (int i = 0; i < ContentContainer.childCount; i++)
            {
                var tile = ContentContainer[i];

                if (Direction == ScrollDirection.Vertical)
                {
                    tile.style.width = Length.Percent(100);
                    tile.style.height = TilePixelSize;
                }
                else
                {
                    tile.style.width = TilePixelSize;
                    tile.style.height = Length.Percent(100);
                }
            }
        }
        
        private void UpdateScroller()
        {
            ScrollerManipulator.SetDirection(Direction);
            ScrollerManipulator.SetTileSize(TilePixelSize);
            ScrollerManipulator.SetCenteringEase(AutoScrollEase);
            ScrollerManipulator.SetCenteringDuration(AutoScrollDuration);
        }

        protected override void Refresh() => BindVisibleItems();
        private void BindVisibleItems()
        {
            if (ContentContainer.childCount == 0)
                return;

            int pooled = ContentContainer.childCount;
            int startIndex = SelectedIndex - pooled / 2;

            for (int i = 0; i < pooled; i++)
                BindTileData(ContentContainer[i], GetItem(startIndex + i));
        }
        
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
    }
}
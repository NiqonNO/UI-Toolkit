using System.Collections.Generic;
using NiqonNO.UI.MVVM;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOMultiCategoryScrollView : ScrollView
	{
		private readonly VisualElement Category;
		private readonly DropdownField Dropdown;
		private readonly VisualElement ScrollerViewport;
		private readonly VisualElement ScrollerContainer;
		
		private readonly List<VisualElement> Tabs = new();
		private int CurrentIndex = -1;
        
		public NOMultiCategoryScrollView() : base()
		{
			NOUSS.TryToApplyStyle(this, NOUSS.MultiCategoryScrollViewStylePath);
			AddToClassList(NOUSS.MultiCategoryScrollViewClass);
			
			Category = contentViewport;
			Category.AddToClassList(NOUSS.MultiCategoryScrollViewCategoryClass);

			ScrollerViewport = new VisualElement() { name = "preset-scroller__viewport" };
			ScrollerViewport.AddToClassList(NOUSS.MultiCategoryScrollViewContentListViewportClass);
			
			ScrollerContainer = new VisualElement() { name = "preset-scroller__content" };
			ScrollerContainer.AddToClassList(NOUSS.MultiCategoryScrollViewContentListContentClass);
			
			Dropdown = new DropdownField() { name = "preset-scroller__category-dropdown" };
			Dropdown.AddToClassList(NOUSS.MultiCategoryScrollViewDropdownClass);
			Dropdown.RegisterCallback<ChangeEvent<string>>(OnDropdownValueChanged);

			Category.Add(Dropdown);
			Category.Add(ScrollerViewport);
			ScrollerViewport.Add(ScrollerContainer);
			ScrollerContainer.Add(base.contentContainer);
		}

		public void AddToViewport(VisualElement element)
		{
			ScrollerViewport.Add(element);
		}

		public void SetCategories(IReadOnlyList<INOBindingCategory> categories)
		{
			string[] dropdownChoices = new string[categories.Count];
			for (var i = 0; i < categories.Count; i++)
			{
				var category = categories[i];
				dropdownChoices[i] = category.CategoryName;
				var tab = new VisualElement() { name = $"preset-scroller__tab-{category.CategoryName}" };
				ScrollerContainer.AddToClassList(NOUSS.MultiCategoryScrollViewContentListTabClass);
				Tabs.Add(tab);
				base.contentContainer.Add(tab);

				foreach (var item in category.ItemsCollection)
				{
					var tile = category.ItemTemplate.Instantiate();
					ScrollerContainer.AddToClassList(NOUSS.MultiCategoryScrollViewContentListTileClass);
					item.Bind(tile);
					tab.Add(tile);
				}
			}

			SelectTab(0);
		}

		private void SelectTab(int idx)
		{
			if (CurrentIndex >= 0)
				Tabs[CurrentIndex].SetCheckedPseudoState(false);

			CurrentIndex = idx;
			Tabs[CurrentIndex].SetCheckedPseudoState(false);
		}
		
		private void OnDropdownValueChanged(ChangeEvent<string> evt)
		{
			SelectTab(Dropdown.index);
		}
	}
}
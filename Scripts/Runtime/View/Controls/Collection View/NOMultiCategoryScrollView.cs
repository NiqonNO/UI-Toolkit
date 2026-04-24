using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace NiqonNO.UI.View
{
	public class NOMultiCategoryScrollView : ScrollView
	{
		private readonly DropdownField Dropdown;
		
		private readonly List<VisualElement> Tabs = new();
		private int CurrentIndex = -1;

		public event Action<INOBindingData> OnItemSelected;
        
		public NOMultiCategoryScrollView() : base()
		{
			NOUSS.TryToApplyStyle(this, NOUSS.MultiCategoryScrollViewStylePath);
			AddToClassList(NOUSS.MultiCategoryScrollViewClass);

			Dropdown = new DropdownField() { name = "preset-scroller__category-dropdown" };
			Dropdown.AddToClassList(NOUSS.MultiCategoryScrollViewDropdownClass);
			Dropdown.RegisterCallback<ChangeEvent<string>>(OnDropdownValueChanged);

			hierarchy.Insert(0, Dropdown);
		}

		public void SetCategories(IReadOnlyList<INOBindingCategory> categories)
		{
			var dropdownChoices = new List<string>(categories.Count);
			foreach (var category in categories)
			{
				var tab = new VisualElement() { name = $"preset-scroller__tab-{category.CategoryName}" };
				tab.AddToClassList(NOUSS.MultiCategoryScrollViewContentListTabClass);
				Tabs.Add(tab);
				base.contentContainer.Add(tab);
				dropdownChoices.Add(category.CategoryName);

				foreach (var item in category.ItemsCollection)
				{
					var tile = category.ItemTemplate.Instantiate();
					tile.AddToClassList(NOUSS.MultiCategoryScrollViewContentListTileClass);
					tile.AddManipulator(new Clickable(_ => SelectItem(item)));
					item.Bind(tile);
					tab.Add(tile);
				}
			}

			Dropdown.choices = dropdownChoices;
			Dropdown.index = 0;
		}

		private void SelectItem(INOBindingData item)
		{
			OnItemSelected?.Invoke(item);
		}
		
		private void SelectTab(int idx)
		{
			if (CurrentIndex >= 0)
				Tabs[CurrentIndex].SetCheckedPseudoState(false);

			CurrentIndex = idx;
			Tabs[CurrentIndex].SetCheckedPseudoState(true);
		}
		
		private void OnDropdownValueChanged(ChangeEvent<string> evt)
		{
			SelectTab(Dropdown.index);
		}
	}
}
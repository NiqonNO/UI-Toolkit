using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOMultiCategoryScrollView : ScrollView
	{
		private readonly VisualElement Category;
		private readonly DropdownField Dropdown;
		private readonly VisualElement ScrollerViewport;
		private readonly VisualElement ScrollerContainer;
		
		public NOMultiCategoryScrollView(List<string> categories) : base()
		{
			NOUSS.TryToApplyStyle(this, NOUSS.MultiCategoryScrollViewStylePath);
			AddToClassList(NOUSS.MultiCategoryScrollViewClass);
			
			Category = contentViewport;
			Category.AddToClassList(NOUSS.MultiCategoryScrollViewCategoryClass);

			ScrollerViewport = new VisualElement() { name = "preset-scroller-viewport" };
			ScrollerViewport.AddToClassList(NOUSS.MultiCategoryScrollViewContentListViewportClass);
			
			ScrollerContainer = new VisualElement() { name = "preset-scroller-content" };
			ScrollerContainer.AddToClassList(NOUSS.MultiCategoryScrollViewContentListContentClass);
			
			Dropdown = new DropdownField(categories, categories[0]) { name = "category-dropdown" };
			Dropdown.AddToClassList(NOUSS.MultiCategoryScrollViewDropdownClass);
			Dropdown.RegisterCallback<ChangeEvent<int>>(OnDropdownValueChanged);
			Dropdown.RegisterCallback<ChangeEvent<string>>(OnDropdownValueStringChanged);
			Dropdown.RegisterValueChangedCallback(OnDropdownValueChangedTest2);

			Category.Add(Dropdown);
			Category.Add(ScrollerViewport);
			ScrollerViewport.Add(ScrollerContainer);
			ScrollerContainer.Add(base.contentContainer);
		}

		public void AddToViewport(VisualElement element)
		{
			ScrollerViewport.Add(element);
		}
		
		private void OnDropdownValueChanged(ChangeEvent<int> evt)
		{
			Debug.Log($"[NOMultiCategoryScrollView] {Dropdown.choices[evt.newValue]}");
		}
		
		private void OnDropdownValueStringChanged(ChangeEvent<string> evt)
		{
			Debug.Log($"[NOMultiCategoryScrollView] 1 - {evt.newValue} {Dropdown.index} - {Dropdown.choices[Dropdown.index]}");
		}
		private void OnDropdownValueChangedTest2(ChangeEvent<string> evt)
		{
			Debug.Log($"[NOMultiCategoryScrollView] 2 - {evt.newValue} {Dropdown.index} - {Dropdown.choices[Dropdown.index]}");
		}
	}
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI.MVVM
{
	[Serializable]
	public class NOBind
	{
		[Flags]
		public enum BindType { None = 0, Name = 1 << 0, Class = 1 << 1, }
		
		[SerializeField] 
		private BindType BindBy;
		[SerializeField] 
		private string BindName;
		[SerializeField] 
		private string BindClass;
		
		private bool BinByName => (BindBy & BindType.Name) != 0 && !string.IsNullOrEmpty(BindName);
		private bool BinByClass => (BindBy & BindType.Class) != 0 && !string.IsNullOrEmpty(BindClass);
		
		public VisualElement QueryTarget(VisualElement root)
		{
			if (BindBy == BindType.None) return null;
			
			return root.Q(BinByName? BindName: null, BinByClass? BindClass: null);
		}
		
		public IEnumerable<VisualElement> QueryTargets(VisualElement root)
		{
			if (BindBy == BindType.None) return null;
			
			var query = root.Query();

			if (BinByName)
				query = query.Name(BindName);

			if (BinByClass)
				query = query.Class(BindClass);

			return query.ToList();
		}
	}
}
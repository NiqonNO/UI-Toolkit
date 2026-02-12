using System;
using UnityEngine;

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
		private string _BindName;
		[SerializeField] 
		private string _BindClass;
		
		public bool Bind => BindBy != BindType.None;
		public string BindName => (BindBy & BindType.Name) == 0 || string.IsNullOrEmpty(_BindName) ? null : _BindName;
		public string BindClass =>(BindBy & BindType.Class) == 0 || string.IsNullOrEmpty(_BindClass) ?  null : _BindClass;
	}
}
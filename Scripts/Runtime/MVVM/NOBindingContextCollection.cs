using System.Collections;
using System.Collections.Generic;
using NiqonNO.UI.MVVM;
using UnityEngine;

namespace NiqonNO.UI
{
	public class NOBindingContextCollection<T> : IList<INOBindingContext> where T : INOBindingContext
	{
		private List<T> Context;

		public NOBindingContextCollection(List<T> context)
		{
			Context = context;
		}

		public INOBindingContext this[int index]
		{
			get => Context[index];
			set => Context[index] = (T)value;
		}

		public int Count => Context.Count;
		public bool IsReadOnly => false;

		public void Add(INOBindingContext item)
		{
			if (item is T itemT) Add(itemT);
			else Debug.LogError($"Cannot add item: expected type {typeof(T)}, but received type {item?.GetType()}.");
		}
		public void Add(T item) => Context.Add(item);
		
		public void Insert(int index, INOBindingContext item)
		{
			if (item is T itemT) Insert(index, itemT);
			else Debug.LogError($"Cannot insert item: expected type {typeof(T)}, but received type {item?.GetType()}.");
		}
		public void Insert(int index, T item) => Context.Insert(index, item);
		
		public bool Contains(INOBindingContext item) => item is T itemT && Contains(itemT);
		public bool Contains(T item) => Context.Contains(item);
		
		public int IndexOf(INOBindingContext item) => item is T itemT ? IndexOf(itemT) : -1;
		public int IndexOf(T item) => Context.IndexOf(item);
		
		public bool Remove(INOBindingContext item) => item is T itemT && Remove(itemT);
		public bool Remove(T item) => Context.Remove(item);
		public void RemoveAt(int index) => Context.RemoveAt(index);
		
		public void Clear() => Context.Clear();

		public void Rebind(List<T> newContext) => Context = newContext;
		
		public void CopyTo(INOBindingContext[] array, int arrayIndex)
		{
			for (int i = 0; i < Context.Count; i++)
				array[arrayIndex + i] = Context[i];
		}

		public IEnumerator<INOBindingContext> GetEnumerator()
		{
			foreach (var item in Context)
				yield return item;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
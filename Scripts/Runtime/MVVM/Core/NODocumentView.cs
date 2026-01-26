using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI.MVVM
{
	public abstract class NODocumentView<TContext> : MonoBehaviour
		where TContext : INOBindingContext
	{
		[SerializeField] 
		private UIDocument Document;
		
		protected TContext BindingContext { get; private set; }

		protected abstract TContext CreateContext();

		protected virtual void OnEnable()
		{
			BindingContext = CreateContext();
			BindingContext.Bind(Document);
		}

		protected virtual void OnDisable()
		{
			BindingContext.Unbind();
			(BindingContext as IDisposable)?.Dispose();
		}
	}
}
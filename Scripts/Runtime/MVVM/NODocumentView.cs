using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI.MVVM
{
	public abstract class NODocumentView<TContext> : MonoBehaviour
		where TContext : INOBindingContext
	{
		[SerializeField] 
		protected UIDocument Document;
		
		protected TContext BindingContext { get; private set; }

		public virtual void Init(TContext bindingContext)
		{
			BindingContext = bindingContext;
			BindingContext.Bind(Document);
		}
	}
}
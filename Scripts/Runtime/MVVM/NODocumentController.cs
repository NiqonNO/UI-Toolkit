using System.Collections.Generic;
using NiqonNO.Core;
using NiqonNO.UI.MVVM.Callbacks;
using Sirenix.OdinInspector;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI.MVVM
{
	public abstract class NODocumentController : NOInitializableMonoBehaviour
	{
		[SerializeField, DontCreateProperty] 
		protected UIDocument Document;

		[SerializeField, DontCreateProperty, PropertyOrder(float.MaxValue)]
		private List<NOViewModel> ViewModels;
		[SerializeField, DontCreateProperty, PropertyOrder(float.MaxValue)]
		private List<NOCallbackHandler> Callbacks;
		/*[SerializeField, PropertyOrder(float.MinValue)]
		private List<NOManipulatorHandler> Manipulators;*/

		protected override void Awake()
		{
			base.Awake();
			OnGameReadyEvent += SetupViewModel;
		}

		protected virtual void SetupViewModel()
		{
			Document.rootVisualElement.dataSource = this;
		}

		private void OnEnable()
		{
			if (!Initialized)
			{
				OnGameReadyEvent += Bind;
				return;
			}
			Bind();
		}

		private void OnDisable()
		{
			Unbind();
		}

		private void Bind()
		{
			var root = Document.rootVisualElement;

			foreach (var viewModel in ViewModels)
				viewModel.RegisterViewModel(root);
			foreach (var callback in Callbacks)
				callback.RegisterCallback(root);
		}

		private void Unbind()
		{		
			foreach (var viewModel in ViewModels)
				viewModel.UnregisterViewModel();
			foreach (var callback in Callbacks)
				callback.UnregisterCallback();
		}
	}
}
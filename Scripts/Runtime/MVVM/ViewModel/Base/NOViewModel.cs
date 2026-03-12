using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NiqonNO.Core;
using Sirenix.OdinInspector;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI.MVVM
{
	public abstract class NOViewModel : NOMonoBehaviour
	{
		[field: SerializeField, DontCreateProperty]
		private NOBind BindTarget { get; set; }

		protected VisualElement BindRoot { get; private set; }

		public virtual void RegisterViewModel(VisualElement root)
		{
			BindRoot ??= BindTarget.QueryTarget(root);
			if (BindRoot is null)
				return;

			BindRoot.dataSource = this;
		}

		public virtual void UnregisterViewModel()
		{
			if (BindRoot == null)
				return;

			if (ReferenceEquals(BindRoot.dataSource, this))
				BindRoot.dataSource = null;
		}
	}

	public abstract class NOViewModel<T> : NOViewModel
	{
		private const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

		[SerializeField,
		 DontCreateProperty,
		 ValueDropdown(nameof(GetPropertyOptions)),
		 ValidateInput(nameof(ValidateProperty))]
		private PropertyReference Source;

		protected PropertyInfo SourceProperty { get; private set; }
		protected object SourceInstance => Source.Component;

		private bool PropertyReady;

		public sealed override void RegisterViewModel(VisualElement root)
		{
			if (!PropertyReady)
			{
				if (Source.Component == null)
					return;

				InitializeSource();
			}

			base.RegisterViewModel(root);
		}

		private void InitializeSource()
		{
			SourceProperty = Source.Component.GetType().GetProperty(Source.PropertyName, BindingAttr);
			if (SourceProperty == null)
				throw new MissingMemberException(Source.Component.GetType().FullName, Source.PropertyName);
			OnSourceReady();
			PropertyReady = true;
		}

		protected abstract void OnSourceReady();



		protected IEnumerable GetPropertyOptions()
		{
			return GetComponentsInParent<Component>()
				.Where(c => c is NODocumentController)
				.SelectMany(CollectionSelector);

			IEnumerable<ValueDropdownItem<PropertyReference>> CollectionSelector(Component c)
			{
				return c.GetType()
					.GetProperties(BindingAttr)
					.Where(Predicate)
					.Select(Selector);

				bool Predicate(PropertyInfo p) =>
					p.PropertyType == typeof(T) && p.GetCustomAttribute<NOMVVMBindAttribute>() != null;

				ValueDropdownItem<PropertyReference> Selector(PropertyInfo p) =>
					new($"{c.GetType().Name}/{p.Name}", new PropertyReference { Component = c, PropertyName = p.Name });
			}
		}

		protected bool ValidateProperty(ref string errorMessage)
		{
			if (Source.Component == null)
				return true;

			if (Source.Component is not NODocumentController)
			{
				errorMessage = $"Component {Source.Component} is not of type {nameof(NODocumentController)}";
				return false;
			}

			var property = Source.Component.GetType().GetProperty(Source.PropertyName, BindingAttr);
			if (property == null)
			{
				errorMessage = $"Could not find {Source.PropertyName} property in {Source.Component}";
				return false;
			}

			if (property.PropertyType != typeof(T))
			{
				errorMessage =
					$"Property {Source.PropertyName} in {Source.Component} is of type {property.PropertyType}, expected {typeof(T)}";
				return false;
			}

			if (property.GetCustomAttribute<NOMVVMBindAttribute>() == null)
			{
				errorMessage =
					$"Property {Source.PropertyName} in {Source.Component} lacks required {nameof(NOMVVMBindAttribute)}";
				return false;
			}

			return true;
		}

		[Serializable]
		private struct PropertyReference
		{
			[ReadOnly] public Component Component;
			[ReadOnly] public string PropertyName;

			public override string ToString() =>
				Component != null ? $"{Component.GetType().Name}/{PropertyName}" : "None";
		}
	}
}
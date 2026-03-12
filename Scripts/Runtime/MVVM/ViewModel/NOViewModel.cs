using System;
using System.Collections;
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
		public abstract void RegisterViewModel(VisualElement root);
		public abstract void UnregisterViewModel();
	}
	
	public abstract class NOViewModel<T> : NOViewModel
	{
		private const BindingFlags BindingAttr = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic; 
		
		[field: SerializeField, DontCreateProperty] 
		private NOBind BindTarget { get; set; }
		
		[SerializeField, 
		 DontCreateProperty,
		 ValueDropdown(nameof(GetPropertyOptions)),
		 ValidateInput(nameof(ValidateProperty))]
		private PropertyReference Source;
		
		[CreateProperty]
		protected T Value
		{
			get => Get != null ? Get() : default;
			set => Set?.Invoke(value);
		}
		
		private Func<T> Get;
		private Action<T> Set;
		private VisualElement BindRoot;
		private PropertyInfo SourceProperty;
		private bool DelegatesCreated;

		public override void RegisterViewModel(VisualElement context)
		{
			if (Source.Component == null)
				return;

			BindRoot ??= BindTarget.QueryTarget(context);
			if (BindRoot is null) return;
			if (!DelegatesCreated)
				CreateDelegates();
			BindRoot.dataSource = this;
		}

		private void CreateDelegates()
		{
			SourceProperty = Source.Component.GetType().GetProperty(Source.PropertyName, BindingAttr);
			if (SourceProperty == null) throw new MissingMemberException(Source.Component.GetType().FullName, Source.PropertyName);
			if(SourceProperty.CanRead)
				Get = (Func<T>)Delegate.CreateDelegate(typeof(Func<T>), Source.Component, SourceProperty.GetMethod);
			if(SourceProperty.CanWrite)
				Set = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), Source.Component, SourceProperty.SetMethod);
			DelegatesCreated = true;
		}

		public override void UnregisterViewModel()
		{
			if(BindRoot == null) return;
			if (ReferenceEquals(BindRoot.dataSource, this))
				BindRoot.dataSource = null;
		}

		protected IEnumerable GetPropertyOptions()
		{
			return GetComponentsInParent<Component>().Where(c => c is NODocumentController)
				.SelectMany(c => c.GetType().GetProperties(BindingAttr)
					.Where(p => p.PropertyType == typeof(T) && p.GetCustomAttribute<NOMVVMBindAttribute>() != null)
					.Select(p => new ValueDropdownItem<PropertyReference>(
						$"{c.GetType().Name}/{p.Name}",
						new PropertyReference { Component = c, PropertyName = p.Name }
					)));
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

			public override string ToString() => Component != null ? $"{Component.GetType().Name}/{PropertyName}" : "None";
		}
	}
}
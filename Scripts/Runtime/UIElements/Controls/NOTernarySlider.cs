using System;
using NiqonNO.Core;
using NiqonNO.Core.Utility;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOTernarySlider : BaseField<Vector3>
	{
		private readonly VisualElement InputContainer;
		private readonly VisualElement DragContainer;
		private readonly VisualElement DragHandle;

		[CreateProperty] public int LowValue { get; set; }

		[CreateProperty] public int HighValue { get; set; } = 100;

		[CreateProperty] public bool RoundToInt { get; set; }

		private Simplex3 NormalizedValue = Simplex3.Identity;

		public NOTernarySlider() : this(string.Empty) { }

		public NOTernarySlider(string label) : base(label, new VisualElement())
		{
			NOUSS.TryToApplyStyle(this, NOUSS.TernaryStylePath);
			AddToClassList(NOUSS.TernaryClass);
			labelElement.AddToClassList(NOUSS.TernaryLabelClass);
			
			InputContainer = this.Q(className: inputUssClassName);
			InputContainer.AddToClassList(NOUSS.TernaryInputContainerClass);
			
			DragHandle = new VisualElement { name = "ternary-drag-handle", usageHints = UsageHints.DynamicTransform, };
			DragHandle.AddToClassList(NOUSS.TernaryHandleClass);
			DragHandle.RegisterCallback<GeometryChangedEvent>(UpdateDragElementPosition);
			
			var dragger = new NOMultiDimensionDragger(DragHandle, SetValueFromCoordinates);
			DragContainer = new VisualElement { name = "ternary-drag-area", };
			DragContainer.AddToClassList(NOUSS.TernaryDragAreaClass);
			DragContainer.RegisterCallback<GeometryChangedEvent>(UpdateDragElementPosition);
			DragContainer.AddManipulator(dragger);
			
			InputContainer.Add(DragContainer);
			DragContainer.Add(DragHandle);
		}

		private void SetValueFromCoordinates(Vector2 coordinates)
		{
			SetNormalizedValue(Simplex3.FromCoordinates(coordinates));
		}
		public void SetNormalizedValue(Simplex3 barycentric)
		{
			NormalizedValue = barycentric;
			value = Simplex3.DenormalizeValue(NormalizedValue, LowValue, HighValue);
		}

		public override void SetValueWithoutNotify(Vector3 barycentric)
		{
			var validValue = ValidateValue(barycentric);
			base.SetValueWithoutNotify(validValue);
			UpdateDragElementPosition();
		}

		private Vector3 ValidateValue(Vector3 barycentric)
		{
			var constraint = BarycentricConstraint.None;
			var normalized = NormalizedValue;
			if (value != barycentric)
			{
				var diff = new Vector3(
					Mathf.Abs(value.x - barycentric.x),
					Mathf.Abs(value.y - barycentric.y),
					Mathf.Abs(value.z - barycentric.z));

				constraint =
					Mathf.Approximately(diff.y + diff.z, Mathf.Epsilon) ? BarycentricConstraint.X :
					Mathf.Approximately(diff.x + diff.z, Mathf.Epsilon) ? BarycentricConstraint.Y :
					Mathf.Approximately(diff.x + diff.y, Mathf.Epsilon) ? BarycentricConstraint.Z :
					BarycentricConstraint.None;

				normalized = Simplex3.NormalizeValue(barycentric, LowValue, HighValue);
			}

			NormalizedValue = Simplex3.Clamp01(normalized, constraint);

			var denormalized = Simplex3.DenormalizeValue(NormalizedValue, LowValue, HighValue);

			if (!RoundToInt) return denormalized;

			var roundedBarycentric = Simplex3.Round(denormalized);
			NormalizedValue = Simplex3.NormalizeValue(roundedBarycentric, LowValue, HighValue);
			return roundedBarycentric;
		}

		private void UpdateDragElementPosition(GeometryChangedEvent evt) => UpdateDragElementPosition();

		private void UpdateDragElementPosition()
		{
			var position = Simplex3.ToPosition(NormalizedValue);
			DragHandle.style.left = Length.Percent(position.x * 100);
			DragHandle.style.top = Length.Percent(position.y * 100);
		}

		[Serializable]
		public new class UxmlSerializedData : BaseField<Vector3>.UxmlSerializedData
		{
#pragma warning disable 649
			[HideInInspector] [SerializeField] [UxmlIgnore]
			private UxmlAttributeFlags LowValue_UxmlAttributeFlags;
			[SerializeField] private int LowValue;

			[SerializeField] [HideInInspector] [UxmlIgnore]
			private UxmlAttributeFlags HighValue_UxmlAttributeFlags;
			[SerializeField] private int HighValue;

			[SerializeField] [HideInInspector] [UxmlIgnore]
			private UxmlAttributeFlags RoundToInt_UxmlAttributeFlags;
			[SerializeField] private bool RoundToInt;
#pragma warning restore 649

			[System.Diagnostics.Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				BaseField<Vector3>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData),
					new UxmlAttributeNames[]
					{
						new("LowValue", "low-value", null, Array.Empty<string>()),
						new("HighValue", "high-value", null, Array.Empty<string>()),
						new("RoundToInt", "round-to-int", null, Array.Empty<string>()),
					});
			}

			public override object CreateInstance() => new NOTernarySlider();

			public override void Deserialize(object obj)
			{
				NOTernarySlider slider = (NOTernarySlider)obj;
				if (ShouldWriteAttributeValue(LowValue_UxmlAttributeFlags))
					slider.LowValue = LowValue;
				if (ShouldWriteAttributeValue(HighValue_UxmlAttributeFlags))
					slider.HighValue = HighValue;
				if (ShouldWriteAttributeValue(RoundToInt_UxmlAttributeFlags))
					slider.RoundToInt = RoundToInt;
				base.Deserialize(obj);
			}
		}
	}
}
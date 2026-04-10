using System;
using NiqonNO.Core;
using NiqonNO.Core.Utility;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOQuaternarySlider : BaseField<Vector4>
	{
		private readonly VisualElement InputContainer;
		private readonly VisualElement DragContainer;
		private readonly VisualElement DragHandle;

		[CreateProperty] public int LowValue { get; set; }

		[CreateProperty] public int HighValue { get; set; } = 100;

		[CreateProperty] public bool RoundToInt { get; set; }

		private Barycentric4 NormalizedValue = Barycentric4.Identity;

		public NOQuaternarySlider() : this(string.Empty)
		{ }

		public NOQuaternarySlider(string label) : base(label, new VisualElement())
		{
			var styleSheet = NOUSS.GetStyleSheet(NOUSS.QuaternaryStylePath);
			if(styleSheet) styleSheets.Add(styleSheet);

			AddToClassList(NOUSS.QuaternaryClass);
			labelElement.AddToClassList(NOUSS.QuaternaryLabelClass);

			InputContainer = this.Q(className: inputUssClassName);
			InputContainer.AddToClassList(NOUSS.QuaternaryInputContainerClass);

			DragHandle = new VisualElement { name = "quaternary-drag-handle", usageHints = UsageHints.DynamicTransform };
			DragHandle.AddToClassList(NOUSS.QuaternaryHandleClass);
			DragHandle.RegisterCallback<GeometryChangedEvent>(UpdateDragElementPosition);

			var dragger = new NOMultiDimensionDragger(DragHandle, SetValueFromCoordinates);
			DragContainer = new VisualElement { name = "quaternary-drag-area", };
			DragContainer.AddToClassList(NOUSS.QuaternaryDragAreaClass);
			DragContainer.RegisterCallback<GeometryChangedEvent>(UpdateDragElementPosition);
			DragContainer.AddManipulator(dragger);

			InputContainer.Add(DragContainer);
			DragContainer.Add(DragHandle);
		}

		private void SetValueFromCoordinates(Vector2 coordinates)
		{
			SetNormalizedValue(Barycentric4.FromCoordinates(coordinates));
		}

		public void SetNormalizedValue(Barycentric4 barycentric)
		{
			NormalizedValue = barycentric;
			value = Barycentric4.DenormalizeValue(NormalizedValue, LowValue, HighValue);
		}

		public override void SetValueWithoutNotify(Vector4 barycentric)
		{
			var validValue = ValidateValue(barycentric);
			base.SetValueWithoutNotify(validValue);
			UpdateDragElementPosition();
		}

		private Vector4 ValidateValue(Vector4 barycentric)
		{
			var constraint = BarycentricConstraint.None;
			var normalized = NormalizedValue;
			if (value != barycentric)
			{
				var diff = new Vector4(
					Mathf.Abs(value.x - barycentric.x),
					Mathf.Abs(value.y - barycentric.y),
					Mathf.Abs(value.z - barycentric.z),
					Mathf.Abs(value.w - barycentric.w));

				constraint =
					Mathf.Approximately(diff.y + diff.z + diff.w, Mathf.Epsilon) ? BarycentricConstraint.X :
					Mathf.Approximately(diff.x + diff.z + diff.w, Mathf.Epsilon) ? BarycentricConstraint.Y :
					Mathf.Approximately(diff.x + diff.y + diff.w, Mathf.Epsilon) ? BarycentricConstraint.Z :
					Mathf.Approximately(diff.x + diff.y + diff.z, Mathf.Epsilon) ? BarycentricConstraint.W :
					BarycentricConstraint.None;

				normalized = Barycentric4.NormalizeValue(barycentric, LowValue, HighValue);
			}

			NormalizedValue = Barycentric4.Clamp01(normalized, constraint);

			var denormalized = Barycentric4.DenormalizeValue(NormalizedValue, LowValue, HighValue);

			if (!RoundToInt) return denormalized;

			var roundedBarycentric = Barycentric4.Round(denormalized);
			NormalizedValue = Barycentric4.NormalizeValue(roundedBarycentric, LowValue, HighValue);
			return roundedBarycentric;
		}

		private void UpdateDragElementPosition(GeometryChangedEvent evt) => UpdateDragElementPosition();

		private void UpdateDragElementPosition()
		{
			var position = Barycentric4.ToPosition(NormalizedValue);
			DragHandle.style.left = Length.Percent(position.x * 100);
			DragHandle.style.top = Length.Percent(position.y * 100);
		}

		[Serializable]
		public new class UxmlSerializedData : BaseField<Vector4>.UxmlSerializedData
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
				BaseField<Vector4>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(UxmlSerializedData),
					new UxmlAttributeNames[]
					{
						new("LowValue", "low-value", null, Array.Empty<string>()),
						new("HighValue", "high-value", null, Array.Empty<string>()),
						new("RoundToInt", "round-to-int", null, Array.Empty<string>()),
					});
			}

			public override object CreateInstance() => new NOQuaternarySlider();

			public override void Deserialize(object obj)
			{
				NOQuaternarySlider slider = (NOQuaternarySlider)obj;
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
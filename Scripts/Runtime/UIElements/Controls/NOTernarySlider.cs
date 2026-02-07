using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOTernarySlider : BaseField<Vector3>
	{
		private readonly VisualElement InputContainer;
		private readonly VisualElement BackgroundElement;
		private readonly VisualElement DragContainer;
		private readonly VisualElement DragHandle;

		[CreateProperty] public int LowValue { get; set; }

		[CreateProperty] public int HighValue { get; set; } = 100;

		[CreateProperty] public bool RoundToInt { get; set; }

		private Vector3 NormalizedValue = new (0.33f, 0.34f, 0.33f);

		public NOTernarySlider() : this(string.Empty) { }

		public NOTernarySlider(string label) : base(label, new NOAspectRatioFitterElement())
		{
			styleSheets.Add(NOUSS.GetStyleSheet(NOUSS.TernaryStylePath));
			
			AddToClassList(NOUSS.TernaryClass);
			labelElement.AddToClassList(NOUSS.TernaryLabelClass);
			InputContainer = this.Q(className: inputUssClassName);
			InputContainer.AddToClassList(NOUSS.TernaryInputContainerClass);

			BackgroundElement = new VisualElement { name = "ternary-background" };
			BackgroundElement.AddToClassList(NOUSS.TernaryBackgroundClass);
			
			DragHandle = new VisualElement { name = "ternary-drag-handle", usageHints = UsageHints.DynamicTransform, };
			DragHandle.AddToClassList(NOUSS.TernaryHandleClass);
			DragHandle.RegisterCallback<GeometryChangedEvent>(UpdateDragElementPosition);
			
			var dragger = new NOBarycentricDragger(DragHandle, SetNormalizedValue);
			DragContainer = new VisualElement { name = "ternary-drag-area", };
			DragContainer.AddToClassList(NOUSS.TernaryDragAreaClass);
			DragContainer.RegisterCallback<GeometryChangedEvent>(UpdateDragElementPosition);
			DragContainer.AddManipulator(dragger);
			
			InputContainer.Add(BackgroundElement);
			BackgroundElement.Add(DragContainer);
			DragContainer.Add(DragHandle);
		}

		public void SetNormalizedValue(Vector3 barycentric)
		{
			NormalizedValue = barycentric;
			value = DenormalizeValue(NormalizedValue);
		}

		public override void SetValueWithoutNotify(Vector3 barycentric)
		{
			Vector3 validValue = ValidateValue(barycentric);
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

				normalized = NormalizeValue(barycentric);
			}

			NormalizedValue = Clamp01(normalized, constraint);

			if (!RoundToInt) return DenormalizeValue(NormalizedValue);

			var roundedBarycentric = Round(DenormalizeValue(NormalizedValue));
			NormalizedValue = NormalizeValue(roundedBarycentric);
			return roundedBarycentric;
		}

		private Vector3 NormalizeValue(Vector3 barycentric) =>
			(barycentric - Vector3.one * LowValue) / (HighValue - LowValue);

		private Vector3 DenormalizeValue(Vector3 barycentric) =>
			Vector3.one * LowValue + barycentric * (HighValue - LowValue);

		private Vector3 Clamp01(Vector3 barycentric, BarycentricConstraint constraint)
		{
			barycentric.x = Mathf.Clamp01(barycentric.x);
			barycentric.y = Mathf.Clamp01(barycentric.y);
			barycentric.z = Mathf.Clamp01(barycentric.z);

			var currentSum = barycentric.x + barycentric.y + barycentric.z;
			return constraint switch
			{
				BarycentricConstraint.X => ClampConstrained(0),
				BarycentricConstraint.Y => ClampConstrained(1),
				BarycentricConstraint.Z => ClampConstrained(2),
				_ => currentSum == 0 ? Vector3.one / 3.0f : barycentric / currentSum
			};

			Vector3 ClampConstrained(int axis)
			{
				var axisNext = (int)Mathf.Repeat(axis + 1, 3);
				var axisPrev = (int)Mathf.Repeat(axis + 2, 3);

				if (currentSum == 0)
				{
					var halfDelta = (1 - barycentric[axis]) / 2.0f;
					barycentric[axisNext] = halfDelta;
					barycentric[axisPrev] = halfDelta;
					return barycentric;
				}

				var sumDelta = 1 - currentSum;
				var sum = barycentric[axisNext] + barycentric[axisPrev];
				var delta = sumDelta * (sum > 0 ? barycentric[axisNext] / sum : 0.5f);

				barycentric[axisNext] = Mathf.Clamp(barycentric[axisNext] + delta, 0, 1 - barycentric[axis]);
				barycentric[axisPrev] = 1 - barycentric[axis] - barycentric[axisNext];
				return barycentric;
			}
		}

		private static Vector3 Round(Vector3 barycentric)
		{
			var floored = new Vector3Int(
				Mathf.FloorToInt(barycentric.x),
				Mathf.FloorToInt(barycentric.y),
				Mathf.FloorToInt(barycentric.z)
			);

			var targetSum = Mathf.RoundToInt(barycentric.x + barycentric.y + barycentric.z);
			var currentSum = floored.x + floored.y + floored.z;
			var delta = targetSum - currentSum;

			if (delta == 0)
				return floored;

			var fracX = barycentric.x - floored.x;
			var fracY = barycentric.y - floored.y;
			var fracZ = barycentric.z - floored.z;

			var minF = Mathf.Min(fracX, Mathf.Min(fracY, fracZ));
			var maxF = Mathf.Max(fracX, Mathf.Max(fracY, fracZ));
			var midF = fracX + fracY + fracZ - minF - maxF;

			var threshold = delta == 1 ? maxF : midF;
			if (fracX >= threshold)
			{
				floored.x += 1;
				delta--;
			}

			if (fracY >= threshold && delta > 0)
			{
				floored.y += 1;
				delta--;
			}

			if (fracZ >= threshold && delta > 0) floored.z += 1;
			return floored;
		}

		private void UpdateDragElementPosition(GeometryChangedEvent evt) => UpdateDragElementPosition();

		private void UpdateDragElementPosition()
		{
			var position = NormalizedValue.x * new Vector2(0.0f, 1.0f) +
			               NormalizedValue.y * new Vector2(0.5f, 0.0f) +
			               NormalizedValue.z * new Vector2(1.0f, 1.0f);

			DragHandle.style.left = Length.Percent(position.x * 100);
			DragHandle.style.top = Length.Percent(position.y * 100);
		}

		enum BarycentricConstraint
		{
			None = 0,
			X = 1,
			Y = 2,
			Z = 3
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
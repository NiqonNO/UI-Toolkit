using NiqonNO.Core;
using NiqonNO.Core.Utility;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOColorPickerPlot : BaseField<Vector2>
	{
		private const string DrawerShaderName = "Shader Graphs/shg_NO_UI_ColorWheel";
		private static readonly string[] KeywordNames = { "_COLOR_PICKER_TYPE_LS_H", "_COLOR_PICKER_TYPE_HL_S", "_COLOR_PICKER_TYPE_HS_L" };
		private static readonly int EdgesOffsetShaderID = Shader.PropertyToID("_Wheel_Edges_Offset");
		private static readonly int ExternalShaderProperty = Shader.PropertyToID("_External");
		private static readonly Shader DrawerShader = Shader.Find(DrawerShaderName);
		
		private VisualElement DragContainer;
		private VisualElement DragHandle;

		private MaterialDefinition PickerSliderMaterial;
		private LocalKeyword[] Keywords;

		private bool UsePolarCoordinates;
		private Vector2 Offests = Vector2.up;

		public NOColorPickerPlot() : this(DrawerShader) { }
		public NOColorPickerPlot(Shader pickerDrawerShader) : base(string.Empty, new VisualElement())
		{
			name = "color-picker-container";
			AddToClassList(NOUSS.ColorPickerPlotClass);
			
			DragHandle = new VisualElement { name = "color-picker-drag-handle", usageHints = UsageHints.DynamicTransform, };
			DragHandle.AddToClassList(NOUSS.ColorPickerPlotHandleClass);
			DragHandle.RegisterCallback<GeometryChangedEvent>(UpdateDragElementPosition);
			
			var dragger = new NOMultiDimensionDragger(DragHandle, SetValueFromCoordinates);
			DragContainer = this.Q(className: inputUssClassName);
			DragContainer.AddToClassList(NOUSS.ColorPickerPlotDragAreaClass);
			DragContainer.RegisterCallback<GeometryChangedEvent>(UpdateDragElementPosition);
			DragContainer.AddManipulator(dragger);
			DragContainer.style.unityMaterial = CreateMaterial(pickerDrawerShader);
			
			Add(DragContainer);
			DragContainer.Add(DragHandle);
		}
		
		public void AddToHandle(VisualElement element)
		{
			DragHandle.Add(element);
		}
		
		public override void SetValueWithoutNotify(Vector2 newValue)
		{
			if(UsePolarCoordinates)
				newValue.x = NOMath.Remap(newValue.x, 0, 1, Offests.x, Offests.y);
			base.SetValueWithoutNotify(newValue);
			UpdateDragElementPosition();
		}
		
		private void SetValueFromCoordinates(Vector2 coordinates)
		{
			value = UsePolarCoordinates ? PolarValueFromPosition(coordinates) : CartesianValueFromPosition(coordinates);
		}
		
		private void UpdateDragElementPosition(GeometryChangedEvent evt) => UpdateDragElementPosition();

		private void UpdateDragElementPosition()
		{
			var position = UsePolarCoordinates ? PositionFromPolarValue(value) : PositionFromCartesianValue(value);
			DragHandle.style.left = Length.Percent(position.x * 100);
			DragHandle.style.top = Length.Percent(position.y * 100);
		}

		private Vector2 CartesianValueFromPosition(Vector2 coordinates)
		{
			return new Vector2(coordinates.x, 1 - coordinates.y).Clamp(0,1);
		}
		private Vector2 PositionFromCartesianValue(Vector2 cartesianValue)
		{
			cartesianValue = cartesianValue.Clamp(0, 1);
			return new Vector2(cartesianValue.x, 1 - cartesianValue.y);
		}

		private Vector2 PolarValueFromPosition(Vector2 coordinates)
		{
			coordinates.x = (1.0f - coordinates.x - 0.5f) * 2f;
			coordinates.y = (coordinates.y - 0.5f) * 2f;
			Vector2 polarValue = Polar.FromCartesian(coordinates);
			polarValue.y /= Mathf.PI * 2f;
			
			return polarValue;
		}
		private Vector2 PositionFromPolarValue(Vector2 polarValue)
		{
			polarValue.x = polarValue.x switch
			{
				< 0f => Mathf.Pow(polarValue.x + 1f, 3f) - 1f,
				> 1f => 1 - Mathf.Pow(1f - (polarValue.x - 1f), 3f) + 1f,
				_ => polarValue.x
			};
			polarValue.x = NOMath.Remap(polarValue.x, Offests.x, Offests.y, 0, 1);
			polarValue.y *= Mathf.PI * 2f;
			Vector2 coordinates = Polar.FromVector2(polarValue).ToCartesian();
			coordinates.x = 1.0f - (0.5f + 0.5f * coordinates.x);
			coordinates.y = 0.5f + 0.5f * coordinates.y;
			
			return coordinates;
		}
		
		private StyleMaterialDefinition CreateMaterial(Shader shader)
		{
			if (shader == null)
			{
				if (DrawerShader == null)
				{
					Debug.LogError(
						$"Aborting material creation. Received null shader and fallback shader '{DrawerShaderName}' could not be resolved. " +
						$"Target object: '{DragContainer?.name}' in '{name}' ({nameof(NOColorPickerPlot)}).");
					return new StyleMaterialDefinition();
				}
				Debug.LogWarning(
					$"Received null shader. Falling back to default shader '{DrawerShaderName}'. " +
					$"Target object: '{DragContainer?.name}' in '{name}' ({nameof(NOColorPickerPlot)}).");
				shader = DrawerShader;
			}

			Keywords = new LocalKeyword[KeywordNames.Length];
			for (var i = 0; i < KeywordNames.Length; i++) 
				Keywords[i] = new LocalKeyword(shader, KeywordNames[i]);

			PickerSliderMaterial = new Material(shader);
			return new StyleMaterialDefinition(PickerSliderMaterial);
		}
		
		public void SetPickerType(int pickerPlotType, bool usePolarCoordinates)
		{
			UsePolarCoordinates = usePolarCoordinates;
			ApplyStyleClasses();
			
			if (PickerSliderMaterial.IsEmpty()) return;
			for (var i = 0; i < Keywords.Length; i++)
				PickerSliderMaterial.material.SetKeyword(Keywords[i], pickerPlotType == i);
		}

		public void SetMaterialOffset(Vector2 offset)
		{
			Offests = offset;
			if (PickerSliderMaterial.IsEmpty()) return;
			PickerSliderMaterial.material.SetVector(EdgesOffsetShaderID, offset);
		}
		
		public void SetMaterialProperty(float externalMaterialValue)
		{
			if (PickerSliderMaterial.IsEmpty()) return;
			PickerSliderMaterial.material.SetFloat(ExternalShaderProperty, externalMaterialValue);
		}
		
		private void ApplyStyleClasses()
		{
			EnableInClassList(NOUSS.ColorPickerPlotRoundClass, UsePolarCoordinates);
			EnableInClassList(NOUSS.ColorPickerPlotSquareClass, !UsePolarCoordinates);

			DragContainer.EnableInClassList(NOUSS.ColorPickerPlotDragAreaRoundClass, UsePolarCoordinates);
			DragContainer.EnableInClassList(NOUSS.ColorPickerPlotDragAreaSquareClass, !UsePolarCoordinates);
		}
	}
}
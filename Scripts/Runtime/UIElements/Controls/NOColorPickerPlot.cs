using NiqonNO.Core;
using NiqonNO.Core.Utility;
using UnityEngine;
using UnityEngine.UIElements;

namespace NiqonNO.UI
{
	public class NOColorPickerPlot : BaseField<Vector2>
	{
		public VisualElement DragContainer { get; }
		public VisualElement DragHandle { get; }

		private bool _IsWheel;
		public bool IsWheel
		{
			get => _IsWheel;
			set
			{
				if (_IsWheel == value) return;
				_IsWheel = value;
				UpdateCoordinateSystem();
			}
		}
		
		public NOColorPickerPlot() : base(string.Empty, new VisualElement())
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
			
			Add(DragContainer);
			DragContainer.Add(DragHandle);
		}
		
		private void SetValueFromCoordinates(Vector2 coordinates)
		{
			value = IsWheel ? PolarValueFromPosition(coordinates) : CartesianValueFromPosition(coordinates);
			UpdateDragElementPosition();
		}
		
		private void UpdateDragElementPosition(GeometryChangedEvent evt) => UpdateDragElementPosition();

		private void UpdateDragElementPosition()
		{
			var position = IsWheel ? PositionFromPolarValue(value) : PositionFromCartesianValue(value);
			DragHandle.style.left = Length.Percent(position.x * 100);
			DragHandle.style.top = Length.Percent(position.y * 100);
		}

		private Vector2 CartesianValueFromPosition(Vector2 coordinates)
		{
			return new Vector2(coordinates.x, 1 - coordinates.y).Clamp(0,1);
		}
		private Vector2 PositionFromCartesianValue(Vector2 cartesianValue)
		{
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
			polarValue.y *= Mathf.PI * 2f;
			Vector2 coordinates = Polar.FromVector2(polarValue).ToCartesian();
			coordinates.x = 1.0f - (0.5f + 0.5f * coordinates.x);
			coordinates.y = 0.5f + 0.5f * coordinates.y;
			
			return coordinates;
		}
		

		public void SetMaterial(Material drawerMaterial)
		{
			DragContainer.style.unityMaterial = new StyleMaterialDefinition(drawerMaterial);
		}

		private void UpdateCoordinateSystem()
		{
			DragContainer.style.borderBottomLeftRadius = 
				DragContainer.style.borderBottomRightRadius =
					DragContainer.style.borderTopLeftRadius =
						DragContainer.style.borderTopRightRadius = IsWheel ? Length.Percent(100) : Length.Percent(0);
				
			UpdateDragElementPosition();
		}
	}
}
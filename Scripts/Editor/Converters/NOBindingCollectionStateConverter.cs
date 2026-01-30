using System;
using System.Collections.Generic;
using System.Text;
using NiqonNO.UI.MVVM;
using UnityEditor.UIElements;

namespace NiqonNO.UI.Editor.Converters
{

	public class NOListStateConverter : NOListStateConverter<INOBindingContext>
	{
		protected override string DataToString(INOBindingContext item) => string.Empty;
		protected override INOBindingContext DataFromString(string data) => default;
	}
	
	public abstract class NOListStateConverter<T> : UxmlAttributeConverter<List<T>>
	{
		public override List<T> FromString(string value)
		{
			var list = new List<T>();

			if (string.IsNullOrEmpty(value))
				return list;

			int index = 0;
			Parse();
			return list;

			void Parse()
			{
				if (index >= value.Length) return;

				if (index + 1 >= value.Length ||
				    value[index] != '[' ||
				    value[index + 1] != '[') return;

				index += 2;

				int typeEnd = value.IndexOf(']', index);
				if (typeEnd == -1) return;

				string typeName = value.Substring(index, typeEnd - index);
				index = typeEnd + 1;

				int dataEnd = value.IndexOf(']', index);
				if (dataEnd == -1) return;

				string data = value.Substring(index, dataEnd - index);
				index = dataEnd + 1;

				var type = Type.GetType(typeName, throwOnError: false);
				if (type != null &&
				    typeof(T).IsAssignableFrom(type) &&
				    Activator.CreateInstance(type) is T instance)
				{
					DataFromString(data);
					list.Add(instance);
				}

				Parse();
			}
		}

		public override string ToString(List<T> value)
		{
			StringBuilder builder = new StringBuilder();
			foreach (var item in value)
			{
				builder.Append("[[");
				builder.Append(item?.GetType());
				builder.Append(']');
				builder.Append(DataToString(item));
				builder.Append(']');
			}

			return builder.ToString();
		}

		protected abstract string DataToString(T item);
		protected abstract T DataFromString(string data);
	}
}
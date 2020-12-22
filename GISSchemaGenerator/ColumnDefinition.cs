using System.Configuration;
using System.Data.Spatial;
using System.Linq;

namespace GISSchemaGenerator
{
	#region Library Imports

	using System;
	using System.Collections.Generic;
	using System.ComponentModel;

	#endregion

	public class ColumnDefinition
	{
		public ColumnDefinition(string name = null)
		{
			if (!string.IsNullOrWhiteSpace(name))
				Name = name;

			RawValues = new List<string>();
		}

		public string Name { get; set; }
		public Type DataType { get; set; }
		public int? MinLength { get; set; }
		public int? MaxLength { get; set; }
		public bool IsNullable { get; set; }
		public bool IsFixedLength { get; set; }
		public ICollection<string> RawValues { get; set; }
		public IEnumerable<string> Values => RawValues?.Where(w => !string.IsNullOrWhiteSpace(w));

		public Type ResolveDataType()
		{
			if (Name == ConfigurationManager.AppSettings["GeometryColumnName"])
				return typeof(DbGeography);

			MinLength = RawValues.Min(m => m.Length);
			MaxLength = RawValues.Max(m => m.Length);

			IsNullable = Values.Count() < RawValues.Count;
			IsFixedLength = MinLength == MaxLength;

			var result = typeof(string);

			if (!Values.Any())
				return result;

			if (Values.All(a => a.Contains(".") && float.TryParse(a, out _)))
				result = typeof(float);
			else if (Values.All(a => a.Contains(".") && double.TryParse(a, out _)))
				result = typeof(double);
			else if (Values.All(a => a.Contains(".") && decimal.TryParse(a, out _)))
				result = typeof(decimal);
			else if (Values.All(a => short.TryParse(a, out _)))
				result = typeof(short);
			else if (Values.All(a => int.TryParse(a, out _)))
				result = typeof(int);
			else if (Values.All(a => long.TryParse(a, out _)))
				result = typeof(long);
			else if (MaxLength == 1 && Values.All(a => !DataHelper.BooleanArray.Contains(a.ToLower())))
				result = typeof(char);
			else if (Values.All(a => bool.TryParse(a, out _) || DataHelper.BooleanArray.Contains(a.ToLower())))
				result = typeof(bool);
			else if (Values.All(a => DateTime.TryParse(a, out _)))
				result = typeof(DateTime);
			else if (Values.All(a => TimeSpan.TryParse(a, out _)))
				result = typeof(TimeSpan);

			if (IsNullable && result != typeof(string))
				return typeof(Nullable<>).MakeGenericType(result);

			return result;
		}

		#region Overrides of Object

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return $"Column: {Name} | {DataType}";
		}

		public string ToEFString()
		{
			var output = string.Empty;
			var actualType = DataType ?? typeof(string);

			if (actualType == typeof(string) && MaxLength != null)
				output += $"\t[MaxLength({MaxLength}, ErrorMessage=\"{{0}}'s length must be {{1}} characters or less\")]\r\n";

			output += $"\tpublic {actualType.GetAliasName()} {Name} {{ get; set; }}";

			return output;
		}

		//public string ToSQLString()
		//{

		//	return $"{Name}\t{(DataType ?? typeof(string)).GetSQLName(DataType)} {Name} {{ get; set; }}";
		//}

		#endregion
	}
}
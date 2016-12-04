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
			Values = new HashSet<ValueDefinition>();

			if (!string.IsNullOrWhiteSpace(name))
				Name = name;
		}

		public string Name { get; set; }
		public Type DataType { get; set; }
		public int? MinLength { get; set; }
		public int? MaxLength { get; set; }
		public decimal? MinRange { get; set; }
		public decimal? MaxRange { get; set; }
		public bool IsNullable { get; set; }
		public ICollection<ValueDefinition> Values { get; set; }

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
			string output = string.Empty;
			Type actualType = DataType ?? typeof(string);

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
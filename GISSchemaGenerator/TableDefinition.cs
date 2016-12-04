namespace GISSchemaGenerator
{
	#region Library Imports

	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Cryptography.X509Certificates;

	#endregion

	public class TableDefinition
	{
		public TableDefinition(string name = null)
		{
			Columns = new HashSet<ColumnDefinition>();

			if (!string.IsNullOrWhiteSpace(name))
				Name = name;
		}

		public string Name { get; set; }
		public ICollection<ColumnDefinition> Columns { get; set; }

		#region Overrides of Object

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			string output = $"Name: {Name} | {Columns.Count} Columns";
			if(Columns.Count > 0)
				output = Columns.Aggregate(output, (current, column) => current + $"\r\n\t{column}");

			return output;
		}

		public string ToEFString()
		{
			string output = $"public class {Name}\r\n{{";
			if (Columns.Count > 0)
				output = Columns.Aggregate(output, (current, column) => current + $"{column.ToEFString()}\r\n");
			output += "}";
			return output;
		}

		#endregion
	}
}
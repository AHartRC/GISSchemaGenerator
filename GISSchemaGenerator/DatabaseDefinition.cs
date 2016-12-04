namespace GISSchemaGenerator
{
	#region Library Imports

	using System.Collections.Generic;
	using System.Linq;

	#endregion

	public class DatabaseDefinition
	{
		public DatabaseDefinition(string name = null)
		{
			Tables = new HashSet<TableDefinition>();

			if (!string.IsNullOrWhiteSpace(name))
				Name = name;
		}

		public string Name { get; set; }
		public ICollection<TableDefinition> Tables { get; set; }
		

		public string ToEFString()
		{
			string output = $"public class {Name}Entities : DbContext\r\n{{\r\n";
			output = Tables.Aggregate(output, (current, table) => current + $"\tpublic DbSet<{table.Name}> {DataHelper.Pluralizer.Pluralize(table.Name)} {{ get; set; }}\r\n");
			output += "}";
			return output;
		}
	}
}
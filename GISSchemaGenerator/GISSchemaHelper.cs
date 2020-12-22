using System.Collections.Generic;

namespace GISSchemaGenerator
{
	#region Library Imports

	using System;
	using System.Configuration;
	using System.Data.Spatial;
	using System.Diagnostics;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.SqlServer.Types;

	#endregion

	public class GISSchemaHelper
	{
		public static string GeometryColumnName => ConfigurationManager.AppSettings["GeometryColumnName"];
		public static string OutputDirectoryPath => ConfigurationManager.AppSettings["OutputDirectory"];

		public GISSchemaHelper()
		{
			Database = new DatabaseDefinition("RawTigerData");
		}

		public DatabaseDefinition Database { get; set; }

		public void ProcessDirectories(DirectoryInfo sourceDirectory)
		{
			var directories = sourceDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);

			foreach (var directory in directories.OrderByDescending(o => o.Name))
			{
				//if (directory.Name != "ANRC")
				//	continue;

				var table = Database.Tables.FirstOrDefault(f => f.Name == directory.Name);

				ProcessDirectory(directory);
			}
		}

		public void ProcessDirectory(DirectoryInfo sourceDirectory)
		{
			if (!sourceDirectory.Exists)
				throw new ArgumentNullException(nameof(sourceDirectory),
					"The Source Directory does not exist. Please provide a valid source directory path.");

			var tableName = sourceDirectory.Name;
			var table = Database.Tables.FirstOrDefault(f => f.Name == tableName);

			if (table == null)
			{
				table = new TableDefinition(tableName);
				Database.Tables.Add(table);
			}

			var files = sourceDirectory.EnumerateFiles("*.zip", SearchOption.AllDirectories);

			foreach (var file in files)
			{
				ProcessZipFile(file);
			}

			foreach (var column in table.Columns)
			{
				column.ResolveDataType();
				column.RawValues.Clear();
			}
		}

		public void ProcessZipFile(FileInfo file)
		{
			var sw = Stopwatch.StartNew();

			if(file == null || !file.Exists)
				throw new ArgumentNullException(nameof(file));

			var tableName = file.Directory?.Name;

			if(string.IsNullOrWhiteSpace(tableName))
				throw new ArgumentNullException(nameof(tableName));

			var table = Database.Tables.FirstOrDefault(f => f.Name == tableName);

			if (table == null)
			{
				table = new TableDefinition(tableName);
				Database.Tables.Add(table);
			}

			var zipFile = ZipFile.OpenRead(file.FullName);

			ZipArchiveEntry attributeEntry = null;
			ZipArchiveEntry metadataEntry = null;

			foreach (var entry in zipFile.Entries)
			{
				if (entry.FullName.EndsWith(".cpg")
					|| entry.FullName.EndsWith(".prj")
					|| entry.FullName.EndsWith(".iso.xml")
					|| entry.FullName.EndsWith(".shp")
					|| entry.FullName.EndsWith(".shx"))
					continue;

				if (entry.FullName.EndsWith(".dbf"))
					attributeEntry = entry;

				if (entry.FullName.EndsWith(".xml"))
					metadataEntry = entry;
			}

			if (attributeEntry == null)
				throw new ArgumentNullException(nameof(attributeEntry), "The attribute entry appears to be null. Unable to proceed!");

			//if (metadataEntry == null)
			//	throw new ArgumentNullException(nameof(metadataEntry), "The metadata entry appears to be null. Unable to proceed!");

			if (metadataEntry != null)
				Console.WriteLine(metadataEntry.FullName);

			Console.WriteLine($"Attribute File: {attributeEntry.FullName}");
			Console.WriteLine($" Metadata File: {metadataEntry?.FullName}");

			var dbfFile = new DBFFile(attributeEntry);

			foreach (var field in dbfFile.Fields)
			{
				var column = table.Columns.FirstOrDefault(f => f.Name == field.Name);

				if (column == null)
				{
					column = new ColumnDefinition(field.Name);
					table.Columns.Add(column);
				}

				foreach (var value in field.Values)
					column.RawValues.Add(value);
			}

			if (zipFile.Entries.Any(a => a.FullName.EndsWith(".shp")))
			{
				var geomColumn = table.Columns.FirstOrDefault(f => f.Name == GeometryColumnName);

				if (geomColumn == null)
				{
					geomColumn = new ColumnDefinition(GeometryColumnName);
					table.Columns.Add(geomColumn);
				}
			}

			sw.Stop();
			Console.WriteLine($"{file.Name} has finished!\r\nTOTAL DURATION: {sw.Elapsed}");
		}

		public void GenerateSchema()
		{
			WriteClassFile("RawTigerData", Database.ToEFString());

			foreach (var table in Database.Tables)
			{
				WriteClassFile(table.Name, table.ToEFString());
			}
		}

		private void WriteClassFile(string name, string contents)
		{
			var fileName = $"{name}.cs";
			var path = Path.Combine(OutputDirectoryPath, "Generated", fileName);

			if (File.Exists(path))
				File.Delete(path);

			File.WriteAllText(path, contents);
		}
	}
}
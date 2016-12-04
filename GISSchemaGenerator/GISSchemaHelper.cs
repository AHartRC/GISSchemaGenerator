namespace GISSchemaGenerator
{
	#region Library Imports

	using System;
	using System.Configuration;
	using System.Diagnostics;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.SqlServer.Types;

	#endregion

	public class GISSchemaHelper
	{
		public GISSchemaHelper()
		{
			Database = new DatabaseDefinition("RawGISData");
		}

		public static string SourceDirectoryPath => ConfigurationManager.AppSettings["SourceDirectory"];
		public DirectoryInfo SourceDirectory => new DirectoryInfo(SourceDirectoryPath);
		public string SourceDirectoryName => SourceDirectory.Name;

		public static string OutputDirectoryPath => ConfigurationManager.AppSettings["OutputDirectory"];
		public DirectoryInfo OutputDirectory => new DirectoryInfo(OutputDirectoryPath);
		public string OutputDirectoryName => OutputDirectory.Name;

		public string GeometryColumnName => ConfigurationManager.AppSettings["GeometryColumnName"];

		public string FilePrefix => ConfigurationManager.AppSettings["FilePrefix"];

		public DatabaseDefinition Database { get; set; }

		public void ProcessDirectory()
		{
			if (!SourceDirectory.Exists)
				throw new ArgumentNullException(nameof(SourceDirectoryPath),
					"The Source Directory does not exist. Please provide a valid source directory path.");

			string[] filePaths = Directory.GetFiles(SourceDirectoryPath, "*.zip", SearchOption.AllDirectories);

			foreach (string filePath in filePaths)
			{
				ProcessZipFile(filePath);
			}

			GenerateSchema();
		}

		public void ProcessZipFile(string filePath)
		{
			Stopwatch sw = Stopwatch.StartNew();
			FileInfo file = new FileInfo(filePath);

			if (file?.Directory == null || !file.Directory.Exists || !file.Exists)
				throw new ArgumentNullException(nameof(file), "The file or directory specified is null or does not exist.");

			string fileName = file.Name;
			string fileExtension = file.Extension;
			string tableName = file.Directory.Name;
			string lowerTableName = tableName.ToLower();
			string outputDirectoryPath = Path.Combine(OutputDirectoryPath, "Schemas", tableName);

			string cleanName = fileName.Replace(fileExtension, string.Empty)
				.Replace(FilePrefix, string.Empty)
				.Replace(lowerTableName, string.Empty)
				.Trim('_')
				.Trim();

			TableDefinition table = Database.Tables.FirstOrDefault(f => f.Name == tableName);

			if (table == null)
			{
				table = new TableDefinition(tableName);
				Database.Tables.Add(table);
			}

			ZipArchive zipFile = ZipFile.OpenRead(filePath);

			ZipArchiveEntry attributeEntry = null;
			ZipArchiveEntry metadataEntry = null;

			foreach (ZipArchiveEntry entry in zipFile.Entries)
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

			if (metadataEntry == null)
				throw new ArgumentNullException(nameof(metadataEntry), "The metadata entry appears to be null. Unable to proceed!");

			Console.WriteLine($"Attribute File: {attributeEntry.FullName}");
			Console.WriteLine($" Metadata File: {metadataEntry.FullName}");

			DBFFile dbfFile = new DBFFile(attributeEntry);

			foreach (DBFField field in dbfFile.Fields)
			{
				ColumnDefinition column = table.Columns.FirstOrDefault(f => f.Name == field.Name);

				if (column == null)
				{
					column = new ColumnDefinition(field.Name);
					table.Columns.Add(column);
				}

				column.DataType = column.DataType.PrioritizeDataType(field.DataType);
				column.MinLength = column.MinLength == null || field.MinLength < column.MinLength ? field.MinLength : column.MinLength;
				column.MaxLength = column.MaxLength == null || field.MaxLength > column.MaxLength ? field.MaxLength : column.MaxLength;
				column.IsNullable = field.IsNullable;
			}

			if (metadataEntry.FullName.Contains(".shp."))
			{
				ColumnDefinition geomColumn = table.Columns.FirstOrDefault(f => f.Name == GeometryColumnName);

				if (geomColumn == null)
				{
					geomColumn = new ColumnDefinition(GeometryColumnName);
					table.Columns.Add(geomColumn);
				}

				if (geomColumn.DataType != typeof (SqlGeometry))
				{
					geomColumn.DataType = typeof (SqlGeometry);
				}
			}

			sw.Stop();
			Console.WriteLine($"{filePath} has finished!\r\nTOTAL DURATION: {sw.Elapsed}");
		}

		public void GenerateSchema()
		{
			Console.WriteLine(Database.ToEFString());

			foreach (var table in Database.Tables)
			{
				Console.WriteLine(table.ToEFString());
			}
		}
	}
}
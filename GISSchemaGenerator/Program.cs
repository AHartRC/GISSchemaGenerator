using System.Configuration;
using System.IO;

namespace GISSchemaGenerator
{
	#region Library Imports

	using System;
	using System.Diagnostics;

	#endregion

	public class Program
	{
		public static string SourceDirectoryPath => ConfigurationManager.AppSettings["SourceDirectory"];
		public static string OutputDirectoryPath => ConfigurationManager.AppSettings["OutputDirectory"];

		public static DirectoryInfo SourceDirectory => new DirectoryInfo(SourceDirectoryPath);
		public static DirectoryInfo OutputDirectory => new DirectoryInfo(OutputDirectoryPath);

		public static string FilePrefix => ConfigurationManager.AppSettings["FilePrefix"];

		static Program()
		{
			Initialize();
			SchemaHelper = new GISSchemaHelper();
		}

		public static GISSchemaHelper SchemaHelper { get; set; }

		private static void Main(string[] args)
		{
			var sw = Stopwatch.StartNew();

			if (!SourceDirectory.Exists)
				throw new ArgumentNullException(SourceDirectoryPath);

			if (!OutputDirectory.Exists)
				OutputDirectory.Create();

			SchemaHelper.ProcessDirectories(SourceDirectory);

			sw.Stop();
			Console.WriteLine($"Application has finished!\r\nTOTAL DURATION: {sw.Elapsed}");
			Console.WriteLine("PRESS ENTER TO EXIT APPLICATION");
			Console.ReadLine();
		}

		private static void Initialize()
		{
			Console.BufferHeight = short.MaxValue - 1;
			Console.BufferWidth = 1024;
		}
	}
}
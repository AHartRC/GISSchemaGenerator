namespace GISSchemaGenerator
{
	#region Library Imports

	using System;
	using System.Diagnostics;

	#endregion

	public class Program
	{
		static Program()
		{
			Initialize();
			SchemaHelper = new GISSchemaHelper();
		}

		public static GISSchemaHelper SchemaHelper { get; set; }

		private static void Main(string[] args)
		{
			Stopwatch sw = Stopwatch.StartNew();
			SchemaHelper.ProcessDirectory();
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
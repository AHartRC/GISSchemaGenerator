using System.Linq;

namespace GISSchemaGenerator
{
	#region Library Imports

	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.IO.Compression;
	using System.Text;

	#endregion

	public class DBFFile
	{
		public DBFFile()
		{
			Initialize();
		}

		public DBFFile(ZipArchiveEntry fileEntry)
		{
			Initialize();
			ProcessFileEntry(fileEntry);
		}

		public DBFHeader Header { get; set; }
		//public byte[] DatabaseContainer { get; set; }
		public ICollection<DBFField> Fields { get; set; }

		private void Initialize()
		{
			Fields = new HashSet<DBFField>();
		}

		public void ProcessFileEntry(ZipArchiveEntry fileEntry)
		{
			using (var stream = fileEntry.Open())
			{
				using (var ms = new MemoryStream())
				{
					stream.CopyTo(ms);
					ms.Position = 0;

					using (var br = new BinaryReader(ms))
					{
						Header = new DBFHeader(br);

						short fieldIndex = 0;
						while (br.PeekChar() != 13)
						{
							var field = new DBFField(fieldIndex, br);
							Fields.Add(field);
							fieldIndex++;
						}

						// Read the terminator character
						br.ReadByte();

						// Visual FoxPro only
						//DatabaseContainer = br.ReadBytes(263);

						for (var i = 0; i < Header.RecordCount; i++)
						{
							// Check if the record starts with an asterisk (indicating that it was deleted)
							if (br.ReadByte() == 42)
								// Ignore deleted records
								continue;

							// Record isn't deleted and should be valid
							foreach (var field in Fields.OrderBy(o => o.Index))
							{
								field.Values.Add(Encoding.ASCII.GetString(br.ReadBytes(field.Length)).Trim());
							}
						}
						br.Close();
					}
					ms.Close();
				}
				stream.Close();
			}
		}
	}
}
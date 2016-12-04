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
		public ICollection<string[]> Values { get; set; }

		private void Initialize()
		{
			Fields = new HashSet<DBFField>();
			Values = new HashSet<string[]>();
		}

		public void ProcessFileEntry(ZipArchiveEntry fileEntry)
		{
			using (Stream stream = fileEntry.Open())
			{
				using (MemoryStream ms = new MemoryStream())
				{
					stream.CopyTo(ms);
					ms.Position = 0;

					using (BinaryReader br = new BinaryReader(ms))
					{
						Header = new DBFHeader(br);

						short fieldIndex = 0;
						while (br.PeekChar() != 13)
						{
							DBFField field = new DBFField(fieldIndex, br);
							Fields.Add(field);
							fieldIndex++;
						}

						// Read the terminator character
						br.ReadByte();

						// Visual FoxPro only
						//DatabaseContainer = br.ReadBytes(263);

						for (int i = 0; i < Header.RecordCount; i++)
						{
							// Check if the record starts with an asterisk (indicating that it was deleted)
							if (br.ReadByte() == 42)
								// Ignore deleted records
								continue;

							string[] record = new string[Fields.Count];

							// Record isn't deleted and should be valid
							foreach (DBFField field in Fields)
							{
								//byte[] value = br.ReadBytes(field.Item1.Length);
								//string valueString = Encoding.ASCII.GetString(value).Trim();
								//record[field.Item2] = valueString;

								record[field.Index] = Encoding.ASCII.GetString(br.ReadBytes(field.Length)).Trim();
							}
							Values.Add(record);
						}
						br.Close();
					}
					ms.Close();
				}
				stream.Close();
			}

			DataHelper.FixFields(this);
		}
	}
}
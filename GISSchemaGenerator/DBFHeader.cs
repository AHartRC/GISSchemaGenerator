namespace GISSchemaGenerator
{
	#region Library Imports

	using System;
	using System.IO;

	#endregion

	public class DBFHeader
	{
		public DBFHeader(BinaryReader br)
		{
			Version = br.ReadByte(); // Byte 0
			LastUpdatedYear = br.ReadByte(); // Byte 1
			LastUpdatedMonth = br.ReadByte(); // Byte 2
			LastUpdatedDay = br.ReadByte(); // Byte 3
			RecordCount = br.ReadInt32(); // Bytes 4, 5, 6, and 7
			HeaderByteCount = br.ReadInt16(); // Bytes 8 & 9
			RecordByteCount = br.ReadInt16(); // Byte 10 & 11
			ReservedA = br.ReadInt16(); // Bytes 12 & 13
			IncompleteTransaction = br.ReadByte(); // Byte 14
			EncryptionFlag = br.ReadByte(); // Byte 15
			FreeRecordThread = br.ReadInt32(); // Bytes 16, 17, 18, and 19
			MultiUserProcessingReserved = br.ReadInt64(); // Bytes 20->27
			MDXFlag = br.ReadByte(); // Byte 28
			LanguageDriver = br.ReadByte(); // Byte 29
			ReservedB = br.ReadInt16(); // Bytes 30 & 31
		}

		public short Version { get; set; }
		public short LastUpdatedYear { get; set; }
		public short LastUpdatedMonth { get; set; }
		public short LastUpdatedDay { get; set; }
		public DateTime LastUpdated => new DateTime(1900 + LastUpdatedYear, LastUpdatedMonth, LastUpdatedDay);
		public int RecordCount { get; set; }
		public short HeaderByteCount { get; set; }
		public short RecordByteCount { get; set; }
		public short ReservedA { get; set; }
		public byte IncompleteTransaction { get; set; }
		public byte EncryptionFlag { get; set; }
		public int FreeRecordThread { get; set; }
		public long MultiUserProcessingReserved { get; set; }
		public byte MDXFlag { get; set; }
		public short LanguageDriver { get; set; }
		public short ReservedB { get; set; }

		public override string ToString()
		{
			return $"Version: {Version}\r\n" +
			       $"Last Updated: {LastUpdated}\r\n" +
			       $"Record Count: {RecordCount}\r\n" +
			       $"Header Byte Count: {HeaderByteCount}\r\n" +
			       $"Record Byte Count: {RecordByteCount}\r\n" +
			       $"Reserved Bytes (A): {ReservedA}\r\n" +
			       $"Incomplete Transaction: {IncompleteTransaction}\r\n" +
			       $"Encryption Flag: {EncryptionFlag}\r\n" +
			       $"Free Record Thread: {FreeRecordThread}\r\n" +
			       $"Multi-User Processing: {MultiUserProcessingReserved}\r\n" +
			       $"MDX Flag: {MDXFlag}\r\n" +
			       $"Language Driver: {LanguageDriver}\r\n" +
			       $"Reserved Bytes (B): {ReservedB}";
		}
	}
}
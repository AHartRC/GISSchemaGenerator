namespace GISSchemaGenerator
{
	#region Library Imports

	using System;
	using System.IO;
	using System.Linq;
	using System.Text;

	#endregion

	public class DBFField
	{
		public DBFField(short index, BinaryReader br)
		{
			Index = index;
			RawName = br.ReadBytes(11).TakeWhile(w => w != 0).ToArray();
			RawFieldType = br.ReadByte();
			DataAddress = br.ReadInt32();
			Length = br.ReadByte();
			DecimalCount = br.ReadByte();
			ReservedA = br.ReadInt16();
			WorkAreaID = br.ReadByte();
			ReservedB = br.ReadInt16();
			SetFieldsFlag = br.ReadByte();
			ReservedC = br.ReadBytes(7);
			MDXFlag = br.ReadByte();
		}

		public short Index { get; private set; }
		public byte[] RawName { get; set; }
		public string Name => Encoding.ASCII.GetString(RawName).Trim();
		public byte RawFieldType { get; set; }
		public char FieldType => Convert.ToChar(RawFieldType);
		public Type DataType { get; set; }
		public int DataAddress { get; set; }
		public short Length { get; set; }
		public short DecimalCount { get; set; }
		public short ReservedA { get; set; }
		public short WorkAreaID { get; set; }
		public short ReservedB { get; set; }
		public short SetFieldsFlag { get; set; }
		public byte[] ReservedC { get; set; }
		public short MDXFlag { get; set; }
		public int? MaxLength { get; set; }
		public int? MinLength { get; set; }
		public long MaxNumericValue { get; set; }
		public long MinNumericValue { get; set; }
		public decimal MaxDecimalValue { get; set; }
		public decimal MinDecimalValue { get; set; }
		public short Precision { get; set; }
		public short Scale { get; set; }
		public bool IsNullable { get; set; }

		public override string ToString()
		{
			return
				$"Name: {Name} | Field Type: {FieldType} | Data Type: {DataType} | Length: {Length} | Decimals: {DecimalCount}";
		}
	}
}
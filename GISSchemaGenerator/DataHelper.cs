namespace GISSchemaGenerator
{
	#region Library Imports

	using System;
	using System.Collections.Generic;
	using System.Data.Entity.Design.PluralizationServices;
	using System.Diagnostics;
	using System.Globalization;
	using System.Linq;
	using Microsoft.SqlServer.Types;

	#endregion

	public static class DataHelper
	{
		public static PluralizationService Pluralizer => PluralizationService.CreateService(CultureInfo.CurrentCulture);

		public static string[] BooleanArray = { "0", "1", "t", "f", "y", "n", "true", "false", "yes", "no" };
		public static string[] BooleanTrueArray = { "1", "t", "y", "true", "yes" };
		public static string[] BooleanFalseArray = { "0", "f", "n", "false", "no" };

		public static Type[] NumericTypes =
		{
			typeof (sbyte), typeof (short), typeof (ushort), typeof (int), typeof (uint),
			typeof (long), typeof (ulong)
		};

		public static Type[] PrecisionNumericTypes = { typeof(float), typeof(double), typeof(decimal) };

		public static void FixFields(DBFFile dbfFile)
		{
			foreach (DBFField field in dbfFile.Fields)
			{
				FixFieldInfo(field, dbfFile.Values.Select(s => s[field.Index]).ToArray());
			}
		}

		public static void FixFieldInfo(DBFField field, IReadOnlyCollection<string> values)
		{
			string[] checkValues = values.Where(w => !string.IsNullOrWhiteSpace(w)).ToArray();

			if (!checkValues.Any() || checkValues.All(string.IsNullOrWhiteSpace))
				return;

			field.MaxLength = checkValues.Max(m => m.Length);
			field.MinLength = checkValues.Min(m => m.Length);
			field.IsNullable = checkValues.Length < values.Count;

			//byte byteVal;
			//char charVal;
			//ushort uShortVal;
			//uint uIntVal;
			//ulong uLongVal;
			bool boolVal;
			sbyte sByteVal;
			decimal decimalVal;
			double doubleVal;
			float floatVal;
			int intVal;
			long longVal;
			short shortVal;

			if (checkValues.All(a => a.Contains(".") && float.TryParse(a, out floatVal)))
				field.DataType = typeof(float);
			else if (checkValues.All(a => a.Contains(".") && double.TryParse(a, out doubleVal)))
				field.DataType = typeof(double);
			else if (checkValues.All(a => a.Contains(".") && decimal.TryParse(a, out decimalVal)))
				field.DataType = typeof(decimal);
			//else if (checkValues.All(a => ushort.TryParse(a, out uShortVal)))
			//	field.DataType = typeof(ushort);
			//else if (checkValues.All(a => uint.TryParse(a, out uIntVal)))
			//	field.DataType = typeof(uint);
			//else if (checkValues.All(a => ulong.TryParse(a, out uLongVal)))
			//	field.DataType = typeof(ulong);
			else if (checkValues.All(a => sbyte.TryParse(a, out sByteVal)))
				field.DataType = typeof(sbyte);
			else if (checkValues.All(a => short.TryParse(a, out shortVal)))
				field.DataType = typeof(short);
			else if (checkValues.All(a => int.TryParse(a, out intVal)))
				field.DataType = typeof(int);
			else if (checkValues.All(a => long.TryParse(a, out longVal)))
				field.DataType = typeof(long);
			else if (field.MaxLength == 1 && checkValues.Any(a => !BooleanArray.Contains(a.ToLower())))
				field.DataType = typeof(char);
			else if (checkValues.All(a => bool.TryParse(a, out boolVal) || BooleanArray.Contains(a.ToLower())))
				field.DataType = typeof(bool);
			else if (field.MaxLength > 0 && checkValues.Any(a => a.Any(char.IsLetter)))
				field.DataType = typeof(string);
		}

		public static Type PrioritizeDataType(this Type currentType, Type newType)
		{
			//if (currentType == null && newType != null)
			//	return newType;

			if (newType == typeof(bool))
			{
				if (currentType == typeof(string) || currentType == typeof(char))
					return currentType;
			}

			if (newType == typeof(float))
			{
				if (currentType == typeof(double) || currentType == typeof(decimal) || currentType == typeof(string))
					return currentType;
			}

			if (newType == typeof(double))
			{
				if (currentType == typeof(decimal) || currentType == typeof(string))
					return currentType;
			}

			if (newType == typeof(decimal))
			{
				// Should be able to just return newtype
			}

			//if (newType == typeof(ushort))
			//{
			//	if (currentType == typeof(uint) || currentType == typeof(ulong) || currentType == typeof(string))
			//		return currentType;
			//}

			//if (newType == typeof(uint))
			//{
			//	if (currentType == typeof(ulong) || currentType == typeof(string))
			//		return currentType;
			//}

			//if (newType == typeof(ulong))
			//{
			//	// Should be able to just return newtype
			//}

			if (newType == typeof(sbyte))
			{
				if (currentType == typeof(short) || currentType == typeof(int) || currentType == typeof(long) || currentType == typeof(string))
					return currentType;
			}

			if (newType == typeof(short))
			{
				if (currentType == typeof(int) || currentType == typeof(long) || currentType == typeof(string))
					return currentType;
			}

			if (newType == typeof(int))
			{
				if (currentType == typeof(long) || currentType == typeof(string))
					return currentType;
			}

			if (newType == typeof(long))
			{
				// Should be able to just return newtype
			}

			if (newType == typeof(char))
			{
				if (currentType == typeof(string) || currentType == typeof(bool))
					return currentType;
			}

			//if (newType == null || newType != currentType)
			//	Debugger.Break();

			return newType ?? currentType;

			//if (currentType == typeof(sbyte))
			//{
			//	if (newType == typeof(short) ||
			//		newType == typeof(int) ||
			//		newType == typeof(long))
			//		return newType;
			//}

			//if (currentType == typeof(short))
			//{
			//	if (newType == typeof(int) ||
			//		newType == typeof(long))
			//		return newType;

			//	if (newType == typeof(sbyte))
			//		return typeof(short);
			//}

			//if (currentType == typeof(int))
			//{
			//	if (newType == typeof(long))
			//		return currentType;

			//	if (newType == typeof (sbyte) ||
			//	    newType == typeof (short))
			//		return currentType;
			//}

			//if (currentType == typeof(ushort))
			//{
			//	if (newType == typeof(uint) ||
			//		newType == typeof(ulong))
			//		return newType;
			//}

			//if (currentType == typeof(uint))
			//{
			//	if (newType == typeof(ulong))
			//		return newType;

			//	if (newType == typeof (ushort))
			//		return currentType;
			//}

			//if (currentType == typeof(float))
			//{
			//	if (newType == typeof(double) ||
			//		newType == typeof(decimal))
			//		return newType;
			//}

			//if (currentType == typeof(double))
			//{
			//	if (newType == typeof(decimal))
			//		return newType;
			//}

			//if (currentType == null)
			//	return newType ?? typeof(string);

			//if (newType != currentType)
			//	Debugger.Break();

			//return currentType;
		}

		public static string GetAliasName(this Type dataType)
		{
			string output = string.Empty;

			bool isNullable = dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(Nullable<>);
			Type baseType = Nullable.GetUnderlyingType(dataType) ?? dataType;

			if (baseType == typeof(char))
				output = "char";
			if (baseType == typeof(string))
				output = "string";
			if (baseType == typeof(byte))
				output = "byte";
			if (baseType == typeof(sbyte))
				output = "sbyte";
			if (baseType == typeof(short))
				output = "short";
			if (baseType == typeof(int))
				output = "int";
			if (baseType == typeof(long))
				output = "long";
			//if (baseType == typeof(ushort))
			//	output = "ushort";
			//if (baseType == typeof(uint))
			//	output = "uint";
			//if (baseType == typeof(ulong))
			//	output = "ulong";
			if (baseType == typeof(float))
				output = "float";
			if (baseType == typeof(double))
				output = "double";
			if (baseType == typeof(decimal))
				output = "decimal";
			if (baseType == typeof(bool))
				output = "bool";
			if (baseType == typeof(bool))
				output = "bool";
			if (baseType == typeof(DateTime))
				output = "DateTime";
			if (baseType == typeof(TimeSpan))
				output = "TimeSpan";
			if (baseType == typeof(SqlGeometry))
				output = "SqlGeometry";
			if (baseType == typeof(SqlGeography))
				output = "SqlGeography";

			if (isNullable)
				output += "?";

			return output;
		}

		//public static string GetSQLName(this Type dataType, int? maxLength = null)
		//{
		//	string output = string.Empty;

		//	bool isNullable = dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(Nullable<>);
		//	Type baseType = Nullable.GetUnderlyingType(dataType);

		//	if (baseType == typeof (string))
		//	{
		//		string length = maxLength != null ? maxLength.ToString() : "MAX";
		//		output = $"NVARCHAR({length})";
		//	}
		//	if (baseType == typeof(sbyte))
		//		output = "sbyte";
		//	if (baseType == typeof(byte))
		//		output = "byte";
		//	if (baseType == typeof(short))
		//		output = "short";
		//	if (baseType == typeof(ushort))
		//		output = "ushort";
		//	if (baseType == typeof(int))
		//		output = "int";
		//	if (baseType == typeof(uint))
		//		output = "uint";
		//	if (baseType == typeof(long))
		//		output = "long";
		//	if (baseType == typeof(ulong))
		//		output = "ulong";
		//	if (baseType == typeof(float))
		//		output = "float";
		//	if (baseType == typeof(double))
		//		output = "double";
		//	if (baseType == typeof(decimal))
		//		output = "decimal";
		//	if (baseType == typeof(bool))
		//		output = "bool";
		//	if (baseType == typeof(char))
		//		output = "char";
		//	if (baseType == typeof(bool))
		//		output = "bool";
		//	if (baseType == typeof(DateTime))
		//		output = "DateTime";

		//	if (isNullable)
		//		output += "?";

		//	return output;
		//}
	}
}
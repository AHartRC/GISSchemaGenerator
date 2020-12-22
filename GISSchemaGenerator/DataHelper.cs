using System.Data.Spatial;

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

		public static string GetAliasName(this Type dataType)
		{
			var output = string.Empty;

			var isNullable = dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(Nullable<>);
			var baseType = Nullable.GetUnderlyingType(dataType) ?? dataType;

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
			if (baseType == typeof(DbGeometry))
				output = "DbGeometry";
			if (baseType == typeof(DbGeography))
				output = "DbGeography";

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
// Copyright © 2015-2025 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.Linq;

namespace A2v10.Data;

public static class SqlExtensions
{
	public static SqlCommand CreateCommandSP(this SqlConnection cnn, String command)
	{
		var cmd = cnn.CreateCommand();
		cmd.CommandType = CommandType.StoredProcedure;
		cmd.CommandText = command;
		return cmd;
	}

	public static Type ToType(this SqlDbType sqlType)
	{
		return sqlType switch
		{
			SqlDbType.BigInt => typeof(Int64),
			SqlDbType.Int => typeof(Int32),
			SqlDbType.SmallInt => typeof(Int16),
			SqlDbType.TinyInt => typeof(Byte),
			SqlDbType.Bit => typeof(Boolean),
			SqlDbType.Float => typeof(Double),
			SqlDbType.Money or SqlDbType.Decimal => typeof(Decimal),
			SqlDbType.Real => typeof(Double),
			SqlDbType.DateTime or SqlDbType.Date or SqlDbType.DateTime2 => typeof(DateTime),
			SqlDbType.DateTimeOffset => typeof(DateTimeOffset),
			SqlDbType.NVarChar or SqlDbType.NText or SqlDbType.NChar or SqlDbType.VarChar or SqlDbType.Text or SqlDbType.Char => typeof(String),
			SqlDbType.VarBinary => typeof(Byte[]),
			SqlDbType.UniqueIdentifier => typeof(Guid),
			_ => throw new ArgumentOutOfRangeException("SqlExtensions.SqlType.ToType"),
		};
	}

	static readonly String[] dateFormats = new String[]
	{
		"yyyyMMdd",
		"yyyy-MM-dd",
		"yyyy-MM-ddTHH:mm"
	};

	public static Object FromString(String strVal, Type to)
	{
		if (String.IsNullOrEmpty(strVal)) // allowEmptyStrings: false
			return DBNull.Value;

		if (to == typeof(Guid))
		{
			if (Guid.TryParse(strVal, out Guid guidResult))
				return guidResult;
			throw new InvalidCastException($"Can't convert '{strVal}' to Guid");
		}
		else if (to == typeof(Decimal)) 
		{ 
			if (Decimal.TryParse(strVal, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal decResult))
				return decResult;
			throw new InvalidCastException($"Can't convert '{strVal}' to Decimal");
		}
		else if (to == typeof(Double))
		{
			if (Double.TryParse(strVal, NumberStyles.Any, CultureInfo.InvariantCulture, out Double dblResult))
				return dblResult;
			throw new InvalidCastException($"Can't convert '{strVal}' to Double");
		}
		else if (to == typeof(DateTime))
		{
			if (DateTime.TryParseExact(strVal, dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateResult))
				return dateResult;
			if (DateTime.TryParse(strVal, out var dateResult2))
				return dateResult2;
			return Convert.ToDateTime(strVal);
		}
		return Convert.ChangeType(strVal, to, CultureInfo.InvariantCulture);
	}


    public static byte[] FromHexString(string hex) =>
    Enumerable.Range(0, hex.Length / 2)
              .Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))
              .ToArray();


    public static Object ConvertTo(Object value, Type to, String columnName)
	{
		if (value == null)
			return DBNull.Value;
		if (value is ExpandoObject eo)
		{
			var id = eo.GetObject("Id");
			if (DataHelpers.IsIdIsNull(id))
				return DBNull.Value;
			if (to == typeof(Guid))
				return Guid.Parse(id.ToString());
			return Convert.ChangeType(id, to, CultureInfo.InvariantCulture);
		}
        else if (to == typeof(Byte[]) && (columnName == "rv" || columnName.EndsWith("!!RowVersion")))
        {
            // BEFORE STRING
            if (value is Byte[])
                return value;
            else if (value is String s)
                return FromHexString(s);
            throw new InvalidCastException($"Cannot convert {value.GetType().Name} to Byte[]");
        }
        else if (value is String str)
			return FromString(str, to);
		// AFTER string
        if (value.GetType() == to)
            return value;
        return Convert.ChangeType(value, to, CultureInfo.InvariantCulture);
	}

	public static Object Value2SqlValue(Object value)
	{
		if (value == null)
			return DBNull.Value;
		return value;
	}

	public static IDictionary<String, Object> GetParametersDictionary(Object prms)
	{
		if (prms == null)
			return null;
		if (prms is ExpandoObject)
			return prms as IDictionary<String, Object>;
		var retDict = new Dictionary<String, Object>();
		var props = prms.GetType().GetProperties();
		foreach (var p in props)
		{
			retDict.Add(p.Name, p.GetValue(prms, null));
		}
		return retDict;
	}

	public static void RemoveDbName(this SqlParameter prm)
	{
		Int32 dotPos = prm.TypeName.IndexOf('.');
		if (dotPos != -1)
		{
			prm.TypeName = prm.TypeName.Substring(dotPos + 1);

			dotPos = prm.TypeName.IndexOf('.');
			// wrap TypeName into []
			var newName = $"[{prm.TypeName.Substring(0, dotPos)}].[{prm.TypeName.Substring(dotPos + 1)}]";
			prm.TypeName = newName;
		}
	}

	public static void SetFromDynamic(SqlParameterCollection prms, Object vals)
	{
		if (vals == null)
			return;
		IDictionary<String, Object> valsD;
		// may be EpandoObject
		valsD = vals as IDictionary<String, Object>;
		valsD ??= vals.GetType()
				.GetProperties()
				.ToDictionary(key => key.Name, val => val.GetValue(vals));
		foreach (var prop in valsD)
			prms.AddWithValue("@" + prop.Key, prop.Value);
	}


	public static String Update2Metadata(this String source)
	{
		if (source.EndsWith(".Update"))
			return source.Substring(0, source.Length - 7) + ".Metadata";
		else if (source.EndsWith(".Update]"))
			return source.Substring(0, source.Length - 8) + ".Metadata]";
		return source;
	}
}

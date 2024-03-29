﻿// Copyright © 2012-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using A2v10.Data.Interfaces;

namespace A2v10.Data;

internal class TableDescription : ITableDescription
{

	readonly DataTable _table;
	readonly List<Object> _list;
	public IFormatProvider FormatProvider { get; set; }

	public TableDescription(DataTable table)
	{
		_table = table;
		_list = new List<Object>();
	}

	public ExpandoObject NewRow()
	{
		var eo =  new ExpandoObject();
		_list.Add(eo);
		return eo;
	}

	public void SetValue(ExpandoObject obj, String propName, Object value)
	{
		var col = _table.Columns[propName];
		if (col == null)
			return;
		var val = ConvertTo(col.DataType, value);
		if (val == null)
			return;
		obj.Set(propName, val);
	}

	Object ConvertTo(Type type, Object value)
	{
		if (value == null)
			return null;
		if (type == value.GetType())
			return value;
		if (type == typeof(DateTime) && value is Double dblVal)
			return DateTime.FromOADate(dblVal);
		var fp = FormatProvider ?? CultureInfo.InvariantCulture;
		return Convert.ChangeType(value, type, fp);
	}

	public ExpandoObject ToObject()
	{
		var eo = new ExpandoObject();
		eo.Set("Rows", _list);
		return eo;
	}
}

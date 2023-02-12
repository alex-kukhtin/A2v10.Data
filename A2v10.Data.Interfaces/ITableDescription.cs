// Copyright © 2015-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Dynamic;

namespace A2v10.Data.Interfaces;

public interface ITableDescription
{
	IFormatProvider FormatProvider { get; set; }
	ExpandoObject NewRow();
	void SetValue(ExpandoObject obj, String propName, Object value);
	ExpandoObject ToObject();
}

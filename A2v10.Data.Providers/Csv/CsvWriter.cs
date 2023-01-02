
// Copyright © 2015-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using A2v10.Data.Interfaces;

namespace A2v10.Data.Providers.Csv;

public class CsvWriter : IExternalDataWriter
{
	private readonly DataFile _file;

	public CsvWriter(DataFile file)
	{
		_file = file;
	}

	public void Write(Stream stream)
	{
		using (var sw = new StreamWriter(stream, _file.Encoding))
		{
			Write(sw);
		}
	}

	public void Write(StreamWriter wr)
	{

		wr.Write(GetHeader());
		for (var r = 0; r < _file.NumRecords; r++)
		{
			wr.WriteLine();
			wr.Write(GetRecord(_file.GetRecord(r)));
		}
	}

	String GetHeader()
	{
		var sb = new StringBuilder();
		foreach (var f in _file.Fields)
		{
			if (sb.Length > 0)
				sb.Append(_file.Delimiter);
			sb.Append(EscapeString(f.Name));
		}
		return sb.ToString();
	}

	String GetRecord(Record record)
	{
		var sa = new List<String>();
		foreach (var df in record.DataFields)
		{
			sa.Add(EscapeString(df.StringValue));
		}
		return String.Join(_file.Delimiter.ToString(), sa);
	}

	String EscapeString(String str)
	{
		if (str == null)
			return String.Empty;
		if (str.IndexOfAny(new Char[] { _file.Delimiter, '"', '\n', '\r' }) != -1)
		{
			return $"\"{str.Replace("\"", "\"\"")}\"";
		}
		return str;
	}
}

// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Data.Providers
{
	public class DataFile
	{
		IList<Field> _fields;
		IList<Record> _records;

		const Int32 DefaultCodePage = 866;

		public DateTime LastModifedDate { get; set; }

		public DataFile()
		{
			_fields = new List<Field>();
			_records = new List<Record>();
			LastModifedDate = DateTime.Today;
			Encoding = Encoding.GetEncoding(DefaultCodePage);
	}

		public Encoding Encoding { get; set; }

		public Int32 FieldCount => _fields.Count;
		public Int32 NumRecords => _records.Count;

		public Field CreateField()
		{
			var f = new Field();
			_fields.Add(f);
			return f;
		}

		public Field GetField(Int32 index)
		{
			if (index < 0 || index >= _fields.Count)
				throw new InvalidOperationException();
			return _fields[index];
		}
		public IEnumerable<Field> Fields => _fields;

		public Record CreateRecord()
		{
			var r = new Record();
			_records.Add(r);
			return r;
		}

		public Record GetRecord(Int32 index)
		{
			if (index < 0 || index >= _records.Count)
				throw new InvalidOperationException();
			return _records[index];
		}
		public IEnumerable<Record> Records => _records;
	}
}

// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using A2v10.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace A2v10.Data.Providers
{
	public class DataFile : IExternalDataFile
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

		private IDictionary<String, Int32> _fieldMap;

		internal void MapFields()
		{
			_fieldMap = new Dictionary<String, Int32>();
			for (Int32 f = 0; f < _fields.Count; f++)
				_fieldMap.Add(_fields[f].Name, f);
		}

		public Record CreateRecord()
		{
			var r = new Record(_fieldMap);
			_records.Add(r);
			return r;
		}

		public Record GetRecord(Int32 index)
		{
			if (index < 0 || index >= _records.Count)
				throw new InvalidOperationException();
			return _records[index];
		}
		public IEnumerable<IExternalDataRecord> Records => _records;

	}
}

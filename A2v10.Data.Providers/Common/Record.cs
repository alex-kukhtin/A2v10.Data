// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using A2v10.Data.Interfaces;

namespace A2v10.Data.Providers
{
	public class Record : IExternalDataRecord
	{
		public List<FieldData> DataFields;
		private IDictionary<String, Int32> _fieldMap;

		public Record(IDictionary<String, Int32> fields)
		{
			DataFields = new List<FieldData>();
			_fieldMap = fields ?? throw new ArgumentNullException(nameof(fields));
		}

		public Record(List<FieldData> dat, IDictionary<String, Int32> fields)
		{
			DataFields = dat;
			_fieldMap = fields ?? throw new ArgumentNullException(nameof(fields));
		}

		public Object FieldValue(String name)
		{
			if (_fieldMap.TryGetValue(name, out Int32 fieldNo))
			{
				if (fieldNo >= 0 && fieldNo < DataFields.Count)
					return DataFields[fieldNo].Value;
			}
			throw new ExternalDataException($"Invalid field name: {name}");
		}
	}
}

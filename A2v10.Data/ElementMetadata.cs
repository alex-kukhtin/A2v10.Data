// Copyright © 2012-2017 Alex Kukhtin. All rights reserved.

using A2v10.Data.Interfaces;
using System;
using System.Collections.Generic;

namespace A2v10.Data
{
	public class ElementMetadata : IDataMetadata
	{
		IDictionary<String, IDataFieldMetadata> _fields = new Dictionary<String, IDataFieldMetadata>();

		public String Id { get; private set; }
		public String Name { get; private set; }
		public String RowNumber { get; private set; }
		public String HasChildren { get; private set; }
		public String Permissions { get; set; }
		public String Items { get; set; }

		public Boolean IsArrayType { get; set; }
		public Boolean IsRowCount { get; set; }
		public Boolean IsGroup { get; set; }

		public SortedList<String, Tuple<Int32, String>> Groups { get; private set; }

		public IDictionary<String, IDataFieldMetadata> Fields { get { return _fields; } }

		public FieldMetadata AddField(FieldInfo field, DataType type)
		{
			if (!field.IsVisible)
				return null;
			FieldMetadata fm;
			if (IsFieldExists(field.PropertyName, type, out fm))
				return fm;
			fm = new FieldMetadata(field, type);
			_fields.Add(field.PropertyName, fm);
			switch (field.SpecType)
			{
				case SpecType.Id:
					Id = field.PropertyName;
					break;
				case SpecType.Name:
					Name = field.PropertyName;
					break;
				case SpecType.RowNumber:
					RowNumber = field.PropertyName;
					break;
				case SpecType.RowCount:
					IsRowCount = true;
					break;
				case SpecType.HasChildren:
					HasChildren = field.PropertyName;
					break;
				case SpecType.Permissions:
					Permissions = field.PropertyName;
					break;
				case SpecType.Items:
					Items = field.PropertyName;
					break;
			}
			return fm;
		}

		public Int32 FieldCount { get { return _fields.Count; } }

		public Boolean ContainsField(String field)
		{
			return _fields.ContainsKey(field);
		}

		bool IsFieldExists(String name, DataType dataType, out FieldMetadata fm)
		{
			fm = null;
			IDataFieldMetadata ifm;
			if (_fields.TryGetValue(name, out ifm))
			{
				fm = ifm as FieldMetadata;
				if (fm.DataType != dataType)
					throw new DataLoaderException($"Invalid property '{name}'. Type mismatch.");
				return true;
			}
			return false;
		}

		public FieldMetadata GetField(String name)
		{
			IDataFieldMetadata fm;
			if (_fields.TryGetValue(name, out fm))
			{
				return fm as FieldMetadata;
			}
			throw new DataLoaderException($"Field '{name}' not found.");
		}
	}
}

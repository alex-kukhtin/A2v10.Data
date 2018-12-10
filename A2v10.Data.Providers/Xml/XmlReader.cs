// Copyright © 2018 Alex Kukhtin. All rights reserved.

using System;
using System.IO;
using System.Xml;

using A2v10.Data.Interfaces;

namespace A2v10.Data.Providers.Xml
{
	public class XmlReader : IExternalDataReader
	{
		private readonly DataFile _file;

		public XmlReader(DataFile file)
		{
			_file = file;
		}

		public IExternalDataFile Read(Stream stream)
		{
			Boolean level2 = false;
			using (var rdr = System.Xml.XmlReader.Create(stream))
			{
				while (rdr.Read()) {
					if (rdr.NodeType == XmlNodeType.Element)
					{
						if (!level2)
							level2 = true;
						else 
							ReadRow(rdr);
					}
				}
			}
			return _file;
		}

		void ReadRow(System.Xml.XmlReader rdr)
		{
			var record = _file.CreateRecord();
			for (Int32 i= 0; i < rdr.AttributeCount; i++)
			{
				rdr.MoveToAttribute(i);
				ReadValue(record, rdr.Name, rdr.Value);
			}
		}

		void ReadValue(Record record, String name, String value)
		{
			Int32 ix = _file.GetOrCreateField(name);
			record.SetFieldValueString(ix, value);
		}
	}
}

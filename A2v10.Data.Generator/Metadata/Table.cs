// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace A2v10.Data.Generator
{
	public class Table
	{
		public String Schema { get; set; }
		public String Name { get; set; }
		public IList<Field> Fields => _fields;

		private readonly IList<Field> _fields;

		public Field Key => Fields.FirstOrDefault(f => f.Id);

		public Table(String schema, String name)
		{
			Name = name;
			Schema = schema;
			_fields = new List<Field>();
		}

		public void WriteCreate(ModelWriter mw)
		{
			if (_fields.Count == 0)
				throw new DataCreatorException($"table {Name} is empty");
			var writer = mw.Writer;
			writer.WriteLine($"if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'{Schema}' and TABLE_NAME=N'{Name}')");
			writer.WriteLine($"\tcreate table [{Schema}].[{Name}]");
			writer.WriteLine("\t)");
			foreach (var f in Fields)
				f.WriteCreate(writer);
			writer.RemoveTailCommma();
			writer.WriteLine("\t);");
		}		
	}
}

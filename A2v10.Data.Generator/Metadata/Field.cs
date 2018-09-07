// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Data.Generator
{
	public class Field
	{
		public String Name { get; set; }
		public FieldType Type { get; set; }
		public Int32 Size { get; set; }
		public Boolean Nullbable { get; set; }
		public Boolean Id { get; set; }

		public Field(String name, FieldType type, Int32 size = 0)
		{
			Name = name;
			Type = type;
			Size = size;
		}

		public void WriteCreate(StringWriter writer)
		{
			writer.WriteLine($"{Name}\t{TypeAsString()},");
		}

		public String TypeAsString()
		{
			var sb = new StringBuilder();
			switch (Type)
			{
				case FieldType.String:
					sb.Append($"nvarchar({Size})");
					break;
				default:
					throw new NotSupportedException();
			}
			sb.Append(Nullbable ? "null" : "not null");
			return sb.ToString();
		}
	}
}

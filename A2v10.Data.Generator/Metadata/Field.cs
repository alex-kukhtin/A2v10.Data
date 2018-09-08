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
		public Boolean Nullable { get; set; }
		public Boolean Id { get; set; }
		public Boolean Parent { get; set; }
		public Table Reference { get; set; }
		public Table ParentTable {get;}

		public Field(Table parent, String name, FieldType type, Int32 size = 0)
		{
			ParentTable = parent;
			Name = name;
			Type = type;
			Size = size;
			Nullable = true;
		}

		public void BuildCreate(StringBuilder sb)
		{
			if (Type == FieldType.Array)
				return;
			sb.AppendLine($"\t[{Name}] {TypeAsString} {NullAsString},");
		}

		public String TypeAsString
		{
			get
			{
				var sb = new StringBuilder();
				switch (Type)
				{
					case FieldType.VarChar:
						sb.Append($"nvarchar({Size})");
						break;
					case FieldType.Char:
						sb.Append($"nchar({Size})");
						break;
					case FieldType.DateTime:
						sb.Append("datetime");
						break;
					case FieldType.Money:
						sb.Append("money");
						break;
					case FieldType.Sequence:
						sb.Append("bigint");
						break;
					case FieldType.Integer:
						sb.Append("int");
						break;
					case FieldType.Float:
						sb.Append("float");
						break;
					default:
						throw new NotSupportedException();
				}
				return sb.ToString();
			}
		}

		public String FieldForSelect(String prefix)
		{
			if (Id)
				return $"[{Name}!!Id] = {prefix}[{Name}]";
			else if (Type == FieldType.Reference)
				return $"[{Name}!T{Reference.EntityName}!RefId] = {prefix}[{Name}]";
			else if (Type == FieldType.Parent)
				return $"[!T{Reference.EntityName}!ParentId] = {prefix}[{Name}]";
			return $"{prefix}[{Name}]";
		}

		public String NullAsString => Nullable? "null" : "not null";
	}
}

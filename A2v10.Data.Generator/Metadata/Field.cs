// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Text;

namespace A2v10.Data.Generator
{
	public enum FieldModifier
	{
		None,
		Id,
		Name,
		UtcDate
	}

	public class Field
	{
		public String Name { get; set; }
		public FieldType Type { get; set; }
		public Int32 Size { get; set; }
		public Boolean Nullable { get; set; }
		public Boolean Parent { get; set; }
		public Table Reference { get; set; }
		public FieldModifier Modifier {get; set; }
		public Boolean PrimaryKey { get; set; }
		public Object Default { get; set; }

		public Table ParentTable { get; }

		public Field(Table parent, String name, FieldType type, Int32 size = 0)
		{
			ParentTable = parent;
			Name = name;
			Type = type;
			Size = size;
			Nullable = true;
		}

		public Boolean IsId => Modifier == FieldModifier.Id;
		public Boolean IsName => Modifier == FieldModifier.Name;
		public Boolean IsReference => Reference != null && !Parent;

		public void BuildCreate(StringBuilder sb)
		{
			if (Type == FieldType.Array)
				return;
			Field f = this;
			if (Parent)
				f = Reference.PrimaryKey;
			sb.AppendLine($"\t[{Name}] {f.TypeAsString} {f.NullAsString},");
		}

		public String TypeAsString
		{
			get
			{
				return Type switch
				{
					FieldType.VarChar => $"nvarchar({Size})",
					FieldType.Char => $"nchar({Size})",
					FieldType.DateTime => "datetime",
					FieldType.Money => "money",
					FieldType.Sequence => "bigint",
					FieldType.Integer => "int",
					FieldType.Boolean => "bit",
					FieldType.Float => "float",
					_ => throw new NotSupportedException($"Invalid field type {Type}"),
				};
			}
		}

		public String FieldForSelect(String prefix)
		{
			if (IsId)
				return $"[{Name}!!Id] = {prefix}[{Name}]";
			else if (IsName)
				return $"[{Name}!!Name] = {prefix}[{Name}]";
			else if (IsReference)
				return $"[{Name}!T{Reference.EntityName}!RefId] = {prefix}[{Name}]";
			else if (Type == FieldType.Parent)
				return $"[!T{Reference.EntityName}!ParentId] = {prefix}[{Name}]";
			return $"{prefix}[{Name}]";
		}

		public String NullAsString => Nullable? "null" : "not null";
	}
}

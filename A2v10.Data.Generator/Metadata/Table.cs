// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace A2v10.Data.Generator
{
	public class Table
	{
		public String Schema { get; set; }
		public String TableName { get; set; }
		public String EntityName { get; set; }
		public IList<Field> Fields => _fields;


		const Int32 DEFAULT_VARCHAR_LENGTH = 255;

		private readonly IList<Field> _fields;

		public Field Key => Fields.FirstOrDefault(f => f.Id);
		public Field Parent => Fields.FirstOrDefault(f => f.Parent);

		public Table(String schema, String name)
		{
			EntityName = name;
			TableName = name.Plural();
			Schema = schema;
			_fields = new List<Field>();
		}

		public Field AddKeyField(String name, FieldType type = FieldType.Sequence, Int32 size = 0)
		{
			if (type == FieldType.VarChar && size == 0)
				size = DEFAULT_VARCHAR_LENGTH;
			if (Key != null)
				throw new DataCreatorException($"Only one key is allowed for table {TableName}");
			var f = new Field(this, name, type, size)
			{
				Id = true,
				Nullable = false
			};
			_fields.Add(f);
			return f;
		}

		public Field AddField(String name, FieldType type = FieldType.VarChar, Int32 size = 0)
		{
			if (type == FieldType.VarChar && size == 0)
				size = DEFAULT_VARCHAR_LENGTH;
			var f = new Field(this, name, type, size);
			_fields.Add(f);
			return f;
		}

		public Field AddReferenceField(String name, Table refTable)
		{
			var f = new Field(this, name, FieldType.Reference)
			{
				Reference = refTable
			};
			_fields.Add(f);
			return f;
		}

		public Field AddArrayField(String name, Table refTable)
		{
			var f = new Field(this, name, FieldType.Array)
			{
				Reference = refTable
			};
			_fields.Add(f);
			return f;
		}

		public Field AddParentField(String name, Table refTable)
		{
			var f = new Field(this, name, FieldType.Parent)
			{
				Parent = true,
				Reference = refTable
			};
			_fields.Add(f);
			return f;
		}

		public void BuildCreate(ModelBuilder modelBuilder)
		{
			if (_fields.Count == 0)
				throw new DataCreatorException($"table {TableName} is empty");
			var sb = modelBuilder.StringBuilder;
			sb.AppendLine($"if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'{Schema}' and TABLE_NAME=N'{TableName}')");
			sb.AppendLine($"create table [{Schema}].[{TableName}] (");
			foreach (var f in Fields)
				f.BuildCreate(sb);
			BuildStdFields(sb);
			sb.AppendLine(");");
			sb.AppendLine("go");
		}

		void BuildStdFields(StringBuilder sb)
		{
			sb.Append("\tUserCreated bigint not null");
			sb.AppendLine($" constraint FK_{TableName}_UserCreated_Users foreign key references a2security.Users(Id),");
			sb.Append("\tUserModified bigint not null");
			sb.AppendLine($" constraint FK_{TableName}_UserModified_Users foreign key references a2security.Users(Id),");
			sb.Append("\tDateCreated datetime not null");
			sb.AppendLine($" constraint DF_{TableName}_DateCreated default(getutcdate()),");
			sb.Append("\tDateModified datetime not null");
			sb.AppendLine($" constraint DF_{TableName}_DateModified default(getutcdate())");
		}

		public void BuildFields(StringBuilder sb, String prefix)
		{
			foreach (var f in Fields)
			{
				sb.Append($"{f.FieldForSelect(prefix)}").Append(", ");
			}
		}
	}
}

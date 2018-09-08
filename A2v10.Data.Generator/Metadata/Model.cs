// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A2v10.Data.Generator
{
	public class Model
	{
		public String Schema { get; set; }
		public String Name { get; set; }
		public Boolean WithIndex { get; set; }

		public Table BasedOn { get; set; }
		public IDictionary<String, Table> Children { get; set; }

		public Boolean Paged { get; set; }

		public Model(String schema, String name, Boolean withIndex = true)
		{
			Name = name;
			Schema = schema;
			WithIndex = withIndex;
			Children = new Dictionary<String, Table>();
		}

		public void BuildCreateProcedures(ModelBuilder modelBuilder)
		{
			if (WithIndex)
				BuildCreateIndex(modelBuilder);
			BuildCreateLoad(modelBuilder);
		}

		public void BuildCreateTypes(ModelBuilder modelBuilder)
		{

		}

		public void BuildCreateIndex(ModelBuilder modelBuilder)
		{
			var sb = modelBuilder.StringBuilder;
			sb.AppendLine($"create procedure [{Schema}].[{Name}.Index]");
			modelBuilder.BuildTenantParam();
			sb.Append("@UserId bigint,");
			// create index parameters
			sb.RemoveTailCommma();
			sb.AppendLine();
			sb.AppendLine("as begin");
			sb.AppendLine("\tset nocount on;");
			sb.AppendLine("\tset transaction isolation level read uncommitted;");
			sb.AppendLine();
			sb.Append($"\tselect [{Name.Plural()}!T{Name}!Array] = null, ");
			BuildFields(sb, BasedOn, String.Empty);
			sb.AppendLine($"\tfrom [{BasedOn.Schema}].[{BasedOn.TableName}];");
			sb.AppendLine();
			BuildReferences(sb, false);
			sb.AppendLine("end");
			sb.AppendLine("go");
		}

		IEnumerable<Field> FieldsForReferences(Table t)
		{
			foreach (var f1 in t.Fields)
			{
				if (f1.Type != FieldType.Reference)
					continue;
				yield return f1;
			}
		}

		IEnumerable<Field> AllFieldsForReferences()
		{
			foreach (var f1 in FieldsForReferences(BasedOn))
			{
				if (f1.Type != FieldType.Reference)
					continue;
				yield return f1;
			}
			foreach (var ch in Children)
			{
				foreach (var f2 in FieldsForReferences(ch.Value))
					yield return f2;
			}
		}

		void BuildReferences(StringBuilder sb, Boolean where)
		{
			var refFeilds = AllFieldsForReferences()
				.Where(fld => fld.Type == FieldType.Reference)
				.GroupBy(fld => fld.Reference.EntityName)
				.Select(grp => new { Group = grp.First(), Fields = String.Join(", ", grp.Select(g => $"m.{g.Name}"))});
			foreach (var f in refFeilds)
			{
				var refTable = f.Group.Reference;
				var refTableKey = refTable.Key;
				sb.Append($"\tselect [!T{refTable.EntityName}!Map] = null, ");
				// fields
				refTable.BuildFields(sb, "s.");
				sb.RemoveTailCommma();
				sb.RemoveTailSpace();
				sb.AppendLine();
				// from
				var parentTable = f.Group.ParentTable;
				sb.AppendLine($"\tfrom [{refTable.Schema}].[{refTable.TableName}] s");
				sb.Append($"\tinner join [{parentTable.Schema}].[{parentTable.TableName}] m on s.[{refTableKey.Name}] in ({f.Fields})");
				String whereKey = BasedOn.Key.Name;
				if (parentTable != BasedOn)
				{
					whereKey = parentTable.Parent.Name;
				}
				if (where)
				{
					sb.AppendLine();
					sb.AppendLine($"\twhere m.[{whereKey}] = @Id");
				}
				else
				{
					sb.AppendLine(";");
				}
				sb.AppendLine();
			}
		}

		void BuildFields(StringBuilder sb, Table table, String prefix)
		{
			table.BuildFields(sb, prefix);
			foreach (var c in Children)
			{
				sb.Append($"[{c.Key}!T{c.Value.EntityName}!Array] = null, ");
			}
			sb.Append($"{prefix}DateCreated, {prefix}DateModified");
			sb.AppendLine();
		}

		public void BuildCreateLoad(ModelBuilder modelBuilder)
		{
			StringBuilder sb = modelBuilder.StringBuilder;
			sb.AppendLine($"create procedure [{Schema}].[{Name}.Load]");
			modelBuilder.BuildTenantParam();
			sb.AppendLine("@UserId bigint,");
			var idKey = BasedOn.Key;
			if (idKey == null)
				throw new DataCreatorException($"There is no key in the '{BasedOn.TableName}' table");
			sb.AppendLine($"@Id {idKey.TypeAsString},");
			sb.RemoveTailCommma();
			sb.AppendLine("as begin");
			sb.AppendLine("\tset nocount on");
			sb.AppendLine("\tset transaction isolation level read uncommitted;");
			sb.AppendLine();
			// select
			sb.Append($"\tselect [{Name}!T{Name}!Object] = null, ");
			BuildFields(sb, BasedOn, "t.");
			sb.AppendLine($"\tfrom [{BasedOn.Schema}].[{BasedOn.TableName}] t ");
			sb.AppendLine($"\twhere t.[{idKey.Name}] = @Id;");
			BuildChildrenLoad(sb);
			sb.AppendLine();
			BuildReferences(sb, true);
			sb.AppendLine("end");
			sb.AppendLine("go");
		}

		void BuildChildrenLoad(StringBuilder sb)
		{
			if (Children == null)
				return;
			foreach (var ch in Children)
			{
				sb.AppendLine();
				var tbl = ch.Value;
				sb.Append($"\tselect [!T{tbl.EntityName}!Array] = null,");
				tbl.BuildFields(sb, String.Empty);
				sb.RemoveTailCommma();
				sb.RemoveTailSpace();
				sb.AppendLine();
				sb.AppendLine($"\tfrom [{tbl.Schema}].[{tbl.TableName}]");
				sb.AppendLine($"\twhere [{tbl.Parent.Name}] = @Id;");
			}
		}

		public void BuildCreateMetadata(ModelBuilder modelBuilder)
		{
			var sb = modelBuilder.StringBuilder;
			sb.AppendLine($"create procedure [{Schema}].[{Name}.Metadata]");
			sb.AppendLine("as begin");
			sb.AppendLine("\tset nocount on;");
			sb.AppendLine("\tset transaction isolation level read uncommitted;");
			sb.AppendLine();
			sb.AppendLine($"\tdeclare @{BasedOn.EntityName} [{BasedOn.Schema}].[{BasedOn.EntityName}.TableType];");
			foreach (var ch in Children)
			{
				var chTable = ch.Value;
				sb.AppendLine($"\tdeclare @{ch.Key} [{chTable.Schema}].[{chTable.EntityName}.TableType];");
			}
			sb.AppendLine();
			sb.AppendLine($"\tselect [{BasedOn.EntityName}!{BasedOn.EntityName}!Metadata] = null, * from @{BasedOn.EntityName};");
			foreach (var ch in Children)
			{
				var chTable = ch.Value;
				sb.AppendLine($"\tselect [{ch.Key}!{BasedOn.EntityName}.{ch.Key}!Metadata] = null, * from @{ch.Key};");
			}
			sb.AppendLine("end");
			sb.AppendLine("go");
		}

		public void BuildCreateUpdate(ModelBuilder modelBuilder)
		{
			var idKey = BasedOn.Key;
			if (idKey == null)
				throw new DataCreatorException($"There is no key in the '{BasedOn.TableName}' table");
			var sb = modelBuilder.StringBuilder;
			sb.AppendLine($"create procedure [{Schema}].[{Name}.Update]");
			modelBuilder.BuildTenantParam();
			sb.AppendLine("@UserId bigint,");
			// table types
			sb.AppendLine($"@{Name} [{BasedOn.Schema}].[{BasedOn.EntityName}.TableType] readonly,");
			sb.AppendLine($"@RetId {idKey.TypeAsString} = null output");
			sb.AppendLine("as begin");
			sb.AppendLine("\tset nocount on");
			sb.AppendLine("\tset transaction isolation level serializable;");
			sb.AppendLine("\tset xact_abort on;");
			sb.AppendLine();
			sb.AppendLine($"\tdeclare @output table(op sysname, id {idKey.TypeAsString});");
			sb.AppendLine();
			// merge operator (main table)
			sb.AppendLine($"\tmerge [{BasedOn.Schema}.{BasedOn.TableName}] as target");
			sb.AppendLine($"\tusing @{Name} as source");
			sb.AppendLine($"\ton target.[{idKey.Name}] = source.[{idKey.Name}]");
			sb.AppendLine("\twhen matched then update set");
			// update part
			foreach (var f in BasedOn.Fields)
			{
				if (f.Id) continue;
				sb.AppendLine($"\t\ttarget.[{f.Name}] = source.[{f.Name}],");
			}
			sb.AppendLine("\t\ttarget.DateModified = getutcdate(),");
			sb.AppendLine("\t\ttarget.UserModified = @UserId");
			sb.AppendLine("\twhen not matched by target then ");
			// insert part
			sb.Append("\t\tinsert (");
			foreach (var f in BasedOn.Fields)
			{
				if (f.Id && f.Type == FieldType.Sequence) continue;
				sb.Append($"[{f.Name}], ");
			}
			sb.AppendLine("UserCreated, UserModified) ");
			sb.Append("\t\tvalues (");
			foreach (var f in BasedOn.Fields)
			{
				if (f.Id && f.Type == FieldType.Sequence) continue;
				sb.Append($"[{f.Name}], ");
			}
			sb.AppendLine("@UserId, @UserId)");
			sb.AppendLine($"\toutput $action op, inserted.[{idKey.Name}] id into @output(op, id)");
			sb.AppendLine();
			sb.AppendLine("\tselect top(1) @RetId = id from @output;");
			sb.AppendLine();
			// merge children tables
			sb.AppendLine($"\texec [{Schema}].[{Name}.Load] {modelBuilder.TenantParamEQ} @UserId=@UserId, @Id=@RetId;");
			sb.AppendLine();
			sb.AppendLine("end");
			sb.AppendLine("go");
		}
	}
}

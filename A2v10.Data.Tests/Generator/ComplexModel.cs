// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using A2v10.Data.Tests;

namespace A2v10.Data.Generator
{
	[TestClass]
	[TestCategory("Generator")]
	public class ComplexModel
	{
		Table MakeAgentTable()
		{
			var t = new Table("a2demo", "Agent", "Agents");
			t.AddKeyField("Id");
			t.AddField("Name").Modifier = FieldModifier.Name;
			return t;
		}

		Table MakeEntityTable(Table unit)
		{
			var t = new Table("a2demo", "Entity", "Entities");
			t.AddKeyField("Id");
			t.AddField("Name").Modifier = FieldModifier.Name;
			t.AddField("Price", FieldType.Float);
			t.AddReferenceField("Unit", unit);
			return t;
		}

		Table MakeUnitTable()
		{
			var t = new Table("a2demo", "Unit", "Units");
			t.AddKeyField("Id");
			t.AddField("Name").Modifier = FieldModifier.Name;
			t.AddField("Short", FieldType.VarChar, 20);
			return t;
		}


		Table MakeRowsTable(Table doc, Table ent)
		{
			var t = new Table("a2demo", "Row", "DocDetails");
			t.AddKeyField("Id");
			t.AddParentField("Document", doc);
			t.AddReferenceField("Entity", ent);
			t.AddField("Qty", FieldType.Float);
			t.AddField("Price", FieldType.Float);
			t.AddField("Sum", FieldType.Money);
			return t;
		}

		Table MakeDocumentTable(Table agent, Table entity)
		{
			var t = new Table("a2demo", "Document", "Documents");
			t.AddKeyField("Id");
			var dateField = t.AddField("Date", FieldType.DateTime);
			dateField.Nullable = false;
			t.AddField("Sum", FieldType.Money);
			t.AddReferenceField("Agent", agent);
			t.AddReferenceField("Company", agent);
			t.AddReferenceField("Entity", entity);
			return t;
		}

		Model MakeTestModel()
		{
			var mb = new ModelBuilder();
			var a = MakeAgentTable();
			var u = MakeUnitTable();
			var e = MakeEntityTable(u);
			var d = MakeDocumentTable(a, e);
			var r = MakeRowsTable(d, e);

			var m = new Model("a2demo", "Document", true)
			{
				BasedOn = d
			};
			m.Children.Add("Rows", r);
			return m;

		}

		[TestMethod]
		public void ComplexIndexProcedure()
		{
			var mb = new ModelBuilder();
			var a = MakeAgentTable();
			var u = MakeUnitTable();
			var e = MakeEntityTable(u);
			var d = MakeDocumentTable(a, e);
			var r = MakeRowsTable(d, e);

			var m = new Model("a2demo", "Document", true)
			{
				BasedOn = d
			};

			m.BuildCreateIndex(mb);

			String expected =
@"create procedure [a2demo].[Document.Index]
@UserId bigint
as begin
	set nocount on;
	set transaction isolation level read uncommitted;

	select [Documents!TDocument!Array] = null, [Id!!Id] = [Id], [Date], [Sum], [Agent!TAgent!RefId] = [Agent], [Company!TAgent!RefId] = [Company], [Entity!TEntity!RefId] = [Entity], DateCreated, DateModified
	from [a2demo].[Documents];

	select [!TAgent!Map] = null, [Id!!Id] = s.[Id], [Name!!Name] = s.[Name]
	from [a2demo].[Agents] s
	inner join [a2demo].[Documents] m on s.[Id] in (m.Agent, m.Company);

	select [!TEntity!Map] = null, [Id!!Id] = s.[Id], [Name!!Name] = s.[Name], s.[Price]
	from [a2demo].[Entities] s
	inner join [a2demo].[Documents] m on s.[Id] in (m.Entity);

end
go
";
			String actual = mb.ToString();

			if (expected != actual)
				Assert.Fail(StringTools.StringDiff(expected, actual));
		}

		[TestMethod]
		public void ComplexLoadProcedure()
		{
			var mb = new ModelBuilder();

			var m = MakeTestModel();

			m.BuildCreateLoad(mb);

			String expected =
@"create procedure [a2demo].[Document.Load]
@UserId bigint,
@Id bigint = null
as begin
	set nocount on
	set transaction isolation level read uncommitted;

	declare @all_Agents table (id bigint);
	declare @all_Entities table (id bigint);
	declare @all_Units table (id bigint);

	select [Document!TDocument!Object] = null, [Id!!Id] = t.[Id], t.[Date], t.[Sum], [Agent!TAgent!RefId] = t.[Agent], [Company!TAgent!RefId] = t.[Company], [Entity!TEntity!RefId] = t.[Entity], [Rows!TRow!Array] = null, t.DateCreated, t.DateModified
	from [a2demo].[Documents] t 
	where t.[Id] = @Id;

	insert into @all_Agents(id)
	select [value] from (
		select [Agent], [Company] from [a2demo].[Documents] where [Id] = @Id) d
		unpivot (value for [name] in ([Agent], [Company])) u;

	insert into @all_Entities(id)
		select [Entity] from [a2demo].[Documents] where [Id] = @Id;

	select [!TRow!Array] = null,[Id!!Id] = [Id], [!TDocument!ParentId] = [Document], [Entity!TEntity!RefId] = [Entity], [Qty], [Price], [Sum]
	from [a2demo].[DocDetails]
	where [Document] = @Id;

	insert into @all_Entities(id)
		select [Entity] from [a2demo].[DocDetails] where [Document] = @Id;

	insert into @all_Units(id)
		select s.[Unit] from @all_Entities b inner join [a2demo].[Entities] s on b.id = s.[Id]

	select [!TAgent!Map] = null, [Id!!Id] = s.[Id], [Name!!Name] = s.[Name]
	from [a2demo].[Agents] s where s.[Id] in (select id from @all_Agents);

	select [!TEntity!Map] = null, [Id!!Id] = s.[Id], [Name!!Name] = s.[Name], s.[Price], [Unit!TUnit!RefId] = s.[Unit]
	from [a2demo].[Entities] s where s.[Id] in (select id from @all_Entities);

	select [!TUnit!Map] = null, [Id!!Id] = s.[Id], [Name!!Name] = s.[Name], s.[Short]
	from [a2demo].[Units] s where s.[Id] in (select id from @all_Units);

end
go
";
			String actual = mb.ToString();

			if (expected != actual)
				Assert.Fail(StringTools.StringDiff(expected, actual));
		}

		[TestMethod]
		public void ComplexMetadataProcedure()
		{
			var mb = new ModelBuilder();

			var m = MakeTestModel();

			m.BuildCreateMetadata(mb);

			String actual = mb.ToString();
			String expected =
@"create procedure [a2demo].[Document.Metadata]
as begin
	set nocount on;
	set transaction isolation level read uncommitted;

	declare @Document [a2demo].[Document.TableType];
	declare @Rows [a2demo].[Row.TableType];

	select [Document!Document!Metadata] = null, * from @Document;
	select [Rows!Document.Rows!Metadata] = null, * from @Rows;
end
go
";
			Assert.AreEqual(expected, actual);
		}
	}
}

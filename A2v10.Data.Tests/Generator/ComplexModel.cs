// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A2v10.Data.Generator
{
	[TestClass]
	[TestCategory("Generator")]
	public class ComplexModel
	{
		Table MakeAgentTable()
		{
			var t = new Table("a2demo", "Agent");
			t.AddKeyField("Id");
			t.AddField("Name");
			return t;
		}

		Table MakeEntityTable()
		{
			var t = new Table("a2demo", "Entity");
			t.AddKeyField("Id");
			t.AddField("Name");
			t.AddField("Price", FieldType.Float);
			return t;
		}


		Table MakeRowsTable(Table doc, Table ent)
		{
			var t = new Table("a2demo", "Row");
			t.AddKeyField("Id");
			t.AddParentField("Document", doc);
			t.AddReferenceField("Entity", ent);
			t.AddField("Qty", FieldType.Float);
			t.AddField("Price", FieldType.Float);
			t.AddField("Sum", FieldType.Money);
			return t;
		}

		Table MakeDocumentTable(Table agent)
		{
			var t = new Table("a2demo", "Document");
			t.AddKeyField("Id");
			var dateField = t.AddField("Date", FieldType.DateTime);
			dateField.Nullable = false;
			t.AddField("Sum", FieldType.Money);
			t.AddReferenceField("Agent", agent);
			t.AddReferenceField("Company", agent);
			return t;
		}

		Model MakeTestModel()
		{
			var mb = new ModelBuilder();
			var a = MakeAgentTable();
			var d = MakeDocumentTable(a);
			var e = MakeEntityTable();
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
			var d = MakeDocumentTable(a);
			var e = MakeEntityTable();
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

	select [Documents!TDocument!Array] = null, [Id!!Id] = [Id], [Date], [Sum], [Agent!TAgent!RefId] = [Agent], [Company!TAgent!RefId] = [Company], DateCreated, DateModified
	from [a2demo].[Documents];

	select [!TAgent!Map] = null, [Id!!Id] = s.[Id], s.[Name]
	from [a2demo].[Agents] s
	inner join [a2demo].[Documents] m on s.[Id] in (m.Agent, m.Company);

end
go
";
			String actual = mb.ToString();

			Assert.AreEqual(expected, actual);
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
@Id bigint
as begin
	set nocount on
	set transaction isolation level read uncommitted;

	select [Document!TDocument!Object] = null, [Id!!Id] = t.[Id], t.[Date], t.[Sum], [Agent!TAgent!RefId] = t.[Agent], [Company!TAgent!RefId] = t.[Company], [Rows!TRow!Array] = null, t.DateCreated, t.DateModified
	from [a2demo].[Documents] t 
	where t.[Id] = @Id;

	select [!TRow!Array] = null,[Id!!Id] = [Id], [!TDocument!ParentId] = [Document], [Entity!TEntity!RefId] = [Entity], [Qty], [Price], [Sum]
	from [a2demo].[Rows]
	where [Document] = @Id;

	select [!TAgent!Map] = null, [Id!!Id] = s.[Id], s.[Name]
	from [a2demo].[Agents] s
	inner join [a2demo].[Documents] m on s.[Id] in (m.Agent, m.Company)
	where m.[Id] = @Id

	select [!TEntity!Map] = null, [Id!!Id] = s.[Id], s.[Name], s.[Price]
	from [a2demo].[Entities] s
	inner join [a2demo].[Rows] m on s.[Id] in (m.Entity)
	where m.[Document] = @Id

end
go
";
			String actual = mb.ToString();

			Assert.AreEqual(expected, actual);
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

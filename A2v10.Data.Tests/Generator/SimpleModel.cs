// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A2v10.Data.Generator
{
	[TestClass]
	[TestCategory("Generator")]
	public class SimpleModel
	{

		Table MakeSimpleTable()
		{
			var t = new Table("a2demo", "Table");
			t.AddKeyField("Key", FieldType.VarChar, 20);
			return t;
		}

		[TestMethod]
		public void CreateSimpleTable()
		{
			var mb = new ModelBuilder();
			var t = MakeSimpleTable();
			t.BuildCreate(mb);

			String expected =
@"if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA=N'a2demo' and TABLE_NAME=N'Tables')
create table [a2demo].[Tables] (
	[Key] nvarchar(20) not null,
	UserCreated bigint not null constraint FK_Tables_UserCreated_Users foreign key references a2security.Users(Id),
	UserModified bigint not null constraint FK_Tables_UserModified_Users foreign key references a2security.Users(Id),
	DateCreated datetime not null constraint DF_Tables_DateCreated default(getutcdate()),
	DateModified datetime not null constraint DF_Tables_DateModified default(getutcdate())
);
go
";
			String actual = mb.ToString();
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SimpleIndexProcedure()
		{
			var mb = new ModelBuilder();
			var t = MakeSimpleTable();

			var m = new Model("a2demo", "Table", true)
			{
				BasedOn = t
			};

			m.BuildCreateIndex(mb);

			String expected =
@"create procedure [a2demo].[Table.Index]
@UserId bigint
as begin
	set nocount on;
	set transaction isolation level read uncommitted;

	select [Tables!TTable!Array] = null, [Key!!Id] = [Key], DateCreated, DateModified
	from [a2demo].[Tables];

end
go
";

			String actual = mb.ToString();
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SimpleLoadProcedure()
		{
			var mb = new ModelBuilder();
			var t = MakeSimpleTable();

			var m = new Model("a2demo", "Table", true)
			{
				BasedOn = t
			};

			m.BuildCreateLoad(mb);

			String actual = mb.ToString();
			String expected =
@"create procedure [a2demo].[Table.Load]
@UserId bigint,
@Id nvarchar(20)
as begin
	set nocount on
	set transaction isolation level read uncommitted;

	select [Table!TTable!Object] = null, [Key!!Id] = t.[Key], t.DateCreated, t.DateModified
	from [a2demo].[Tables] t 
	where t.[Key] = @Id;

end
go
";

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SimpleUpdateProcedure()
		{
			var mb = new ModelBuilder();
			var t = MakeSimpleTable();

			var m = new Model("a2demo", "Table", true)
			{
				BasedOn = t

			};

			m.BuildCreateUpdate(mb);

			String actual = mb.ToString();
			String expected =
@"create procedure [a2demo].[Table.Update]
@UserId bigint,
@Table [a2demo].[Table.TableType] readonly,
@RetId nvarchar(20) = null output
as begin
	set nocount on
	set transaction isolation level serializable;
	set xact_abort on;

	declare @output table(op sysname, id nvarchar(20));

	merge [a2demo.Tables] as target
	using @Table as source
	on target.[Key] = source.[Key]
	when matched then update set
		target.DateModified = getutcdate(),
		target.UserModified = @UserId
	when not matched by target then 
		insert ([Key], UserCreated, UserModified) 
		values ([Key], @UserId, @UserId)
	output $action op, inserted.[Key] id into @output(op, id)

	select top(1) @RetId = id from @output;

	exec [a2demo].[Table.Load]  @UserId=@UserId, @Id=@RetId;

end
go
";
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void SimpleMetadataProcedure()
		{
			var mb = new ModelBuilder();
			var t = MakeSimpleTable();

			var m = new Model("a2demo", "Table", true)
			{
				BasedOn = t
			};

			m.BuildCreateMetadata(mb);

			String actual = mb.ToString();
			String expected =
@"create procedure [a2demo].[Table.Metadata]
as begin
	set nocount on;
	set transaction isolation level read uncommitted;

	declare @Table [a2demo].[Table.TableType];

	select [Table!Table!Metadata] = null, * from @Table;
end
go
";

			Assert.AreEqual(expected, actual);
		}
	}
}

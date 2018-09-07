using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Data.Generator
{
	public class Model
	{
		public String Schema { get; set; }
		public String Name { get; set; }
		public Boolean WithIndex { get; set; }

		public Table BasedOn { get; set; }
		public IList<Table> Children { get; set; }

		public Boolean Paged { get; set; }

		public Model(String schema, String name, Boolean withIndex = true)
		{
			Name = name;
			Schema = schema;
			WithIndex = withIndex;
		}

		public void WriteCreateProcedures(ModelWriter modelWriter)
		{
			if (WithIndex)
				WriteCreateIndex(modelWriter);
			WriteCreateLoad(modelWriter);
		}

		public void WriteCreateTypes(ModelWriter modelWriter)
		{

		}

		public void WriteCreateIndex(ModelWriter modelWriter)
		{
			var writer = modelWriter.Writer;
			writer.WriteLine($"create procedure [{Schema}].[{Name}.Index]");
			modelWriter.WriteTenantParam();
			writer.WriteLine("@UserId bigint,");
			// create index parameters
			writer.RemoveTailCommma();
			writer.WriteLine("as begin");
			writer.WriteLine("set nocount on");
			writer.WriteLine("set transaction isolation level read uncommitted;");
			// create body // TODO: plural entity NAME ????
			writer.WriteLine($"select [{Name}s!T{Name}!Array] = null, ");
			writer.WriteLine("end");
			writer.WriteLine("go");
		}

		public void WriteCreateLoad(ModelWriter modelWriter)
		{
			var writer = modelWriter.Writer;
			writer.WriteLine($"create procedure [{Schema}].[{Name}.Load]");
			modelWriter.WriteTenantParam();
			writer.WriteLine("@UserId bigint,");
			var idKey = BasedOn.Key;
			if (idKey == null)
				throw new DataCreatorException($"There is no key in the '{BasedOn.Name}' table");
			writer.WriteLine($"@Id {idKey.TypeAsString()},");
			writer.RemoveTailCommma();
			writer.WriteLine("as begin");
			writer.WriteLine("set nocount on");
			writer.WriteLine("set transaction isolation level read uncommitted;");
			// select
			writer.WriteLine($"select [{Name}!T{Name}!Object] = null, ");
			writer.WriteLine($"[{idKey.Name}!!Id] = [{idKey.Name}],");
			// fields
			writer.RemoveTailCommma();
			writer.WriteLine($"from [{BasedOn.Schema}].[{BasedOn.Name}] t ");
			writer.WriteLine("where t.[{idKey.Name}] = @Id");
			writer.WriteLine("end");
			writer.WriteLine("go");
		}

		public void WriteCreateMetadata(ModelWriter modelWriter)
		{
			var writer = modelWriter.Writer;
			writer.WriteLine($"create procedure [{Schema}].[{Name}.Metadata]");
			writer.WriteLine("as begin");
			writer.WriteLine("set nocount on");
			writer.WriteLine("set transaction isolation level read uncommitted;");
			writer.WriteLine("end");
			writer.WriteLine("go");
		}

		public void WriteCreateUpdate(ModelWriter modelWriter)
		{
			var idKey = BasedOn.Key;
			if (idKey == null)
				throw new DataCreatorException($"There is no key in the '{BasedOn.Name}' table");
			var writer = modelWriter.Writer;
			writer.WriteLine($"create procedure [{Schema}].[{Name}.Update]");
			modelWriter.WriteTenantParam();
			writer.WriteLine("@UserId bigint,");
			// table types
			writer.WriteLine($"@RetId {idKey.TypeAsString()} = null output");
			writer.WriteLine("as begin");
			writer.WriteLine("set nocount on");
			writer.WriteLine("set transaction isolation level serializable;");
			writer.WriteLine("set xact_abort on;");
			writer.WriteLine();
			writer.WriteLine($"declare @output table(op sysname, id {idKey.TypeAsString()});");
			// merge operator (main table)
			writer.WriteLine($"merge [{BasedOn.Schema}.{BasedOn.Name}] as target");
			writer.WriteLine($"using @{Name} as source");
			writer.WriteLine($"on target.[{idKey.Name}] = source.[{idKey.Name}]");
			writer.WriteLine("when matched then update set");
			// update part
			writer.WriteLine("when not matched by target then ");
			// insert part
			writer.WriteLine($"output $action op, inserted.[{idKey.Name}] id");
			writer.WriteLine("into @output(op, id)");
			writer.WriteLine();
			writer.WriteLine("select top(1) @RetId = id from @output;");
			writer.WriteLine();
			// merge children tables
			writer.WriteLine($"exec [{Schema}].[{Name}.Load] {modelWriter.TenantParamEQ} @UserId=@UserId, @Id=@RetId");
			writer.WriteLine();
			writer.WriteLine("end");
			writer.WriteLine("go");
		}
	}
}

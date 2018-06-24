using System;

using A2v10.Data.Interfaces;
using A2v10.Data.ScriptBuilder;
using A2v10.Data.Tests.Configuration;

namespace ScriptBuilder
{
	class Program
	{
		static void Main(String[] args)
		{
			var iDbContext = Starter.Create();

			const String divider = "==========================";

			IDataModel dm = iDbContext.LoadModel(null, "a2test.[SimpleModel.Load]");

			var scripter = new VueScriptBuilder();
			String script = dm.CreateScript(scripter);
			Console.WriteLine(script);
			Console.WriteLine(divider);

			dm = iDbContext.LoadModel(null, "a2test.[MapObjects.Load]");
			script = dm.CreateScript(scripter);
			Console.WriteLine(script);
			Console.WriteLine(divider);

			dm  = iDbContext.LoadModel(null, "a2test.[Document.RowsMethods.Load]");
			script = dm.CreateScript(scripter);
			Console.WriteLine(script);
			Console.WriteLine(divider);
		}
	}
}

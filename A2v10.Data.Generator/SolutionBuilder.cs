// Copyright © 2018 Alex Kukhtin. All rights reserved.

using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace A2v10.Data.Generator
{
	public class SolutionBuilder
	{
		public void BuildSolution(String fileName, ModelBuilder builder)
		{
			if (!File.Exists(fileName))
				throw new FileNotFoundException(fileName);
			String text = File.ReadAllText(fileName, Encoding.UTF8);

			var jsonModule = JsonConvert.DeserializeObject<JsonModule>(text);
			jsonModule.EndInit();

			var solution = new Solution(jsonModule);

			MakeTables(solution, jsonModule, builder);
		}

		void MakeTables(Solution solution, JsonModule module, ModelBuilder builder)
		{
			foreach (var table in module.Tables)
				solution.AddTable(table.Key, table.Value);
			solution.CreateFields();
			solution.MakeTables(builder);
		}
	}
}

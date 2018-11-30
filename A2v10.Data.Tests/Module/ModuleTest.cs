// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.IO;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using A2v10.Data.Generator;

namespace A2v10.Data.Solution
{
	[TestClass]
	[TestCategory("Solution")]
	public class ModuleTest
	{
		const String SOLUTION_FILE = "../../testfiles/solution.json";

		[TestMethod]
		public void SimpleModuleLoad()
		{
			var json = File.ReadAllText(SOLUTION_FILE);
			JsonModule module = JsonConvert.DeserializeObject<JsonModule>(json);
			module.EndInit();
			String result = JsonConvert.SerializeObject(module);
			//int z = 55;
		}
	}
}

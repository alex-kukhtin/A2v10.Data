using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using A2v10.Data.Interfaces;
using A2v10.Data.Tests.Configuration;
using A2v10.Data.ScriptBuilder;
using A2v10.Data.Tests;

namespace A2v10.Data.Models
{
	[TestClass]
	public class MultiplyTrees
	{
		IDbContext _dbContext;
		public MultiplyTrees()
		{
			_dbContext = Starter.Create();
		}

		[TestMethod]
		public async Task LoadMultiplyTreeModel()
		{
			var dm = await _dbContext.LoadModelAsync(null, "a2test.[MultiplyTrees.Load]");


			var json = JsonConvert.SerializeObject(dm.Root);

			var md = new MetadataTester(dm);
			md.IsAllKeys("TRoot,TModel,TElem");
			md.HasAllProperties("TRoot", "Model");
			md.HasAllProperties("TModel", "Id,Elements");
			md.HasAllProperties("TElem", "Id,Name,TreeItems");




			var dt = new DataTester(dm, "Model");
			dt.IsArray(2);
			dt.AreArrayValueEqual(50, 0, "Id");
			dt.AreArrayValueEqual(70, 1, "Id");

			dt = new DataTester(dm, "Model[0].Elements");
			dt.AreValueEqual(50, "Id");
			dt.AreValueEqual("50-50", "Name");

			dt = new DataTester(dm, "Model[0].Elements.TreeItems");
			dt.IsArray(2);
			dt.AreArrayValueEqual(500, 0, "Id");
			dt.AreArrayValueEqual("50-50-500", 0, "Name");
			dt.AreArrayValueEqual(510, 1, "Id");
			dt.AreArrayValueEqual("50-50-510", 1, "Name");

			dt = new DataTester(dm, "Model[1].Elements");
			dt.AreValueEqual(70, "Id");
			dt.AreValueEqual("70-70", "Name");

			dt = new DataTester(dm, "Model[1].Elements.TreeItems");
			dt.IsArray(1);
			dt.AreArrayValueEqual(700, 0, "Id");
			dt.AreArrayValueEqual("70-70-700", 0, "Name");

			dt = new DataTester(dm, "Model[1].Elements.TreeItems[0].TreeItems");
			dt.IsArray(1);
			dt.AreArrayValueEqual(710, 0, "Id");
			dt.AreArrayValueEqual("70-70-700-710", 0, "Name");
		}
	}
}

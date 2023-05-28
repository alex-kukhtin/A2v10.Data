using A2v10.Data.Interfaces;
using A2v10.Data.Tests.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Data.Tests;

[TestClass]
[TestCategory("Models.Dynamic Grouping")]
public class DynamicGrouping
{
	private readonly IDbContext _dbContext;
	public DynamicGrouping()
	{
		_dbContext = Starter.Create();
	}

	[TestMethod]
	public async Task LoadMultParent()
	{
		var dm = await _dbContext.LoadModelAsync(null, "a2test.[MultParent.Load]");

		var md = new MetadataTester(dm);
		md.IsAllKeys("TRoot,TRepInfo,TItem");
		md.HasAllProperties("TRoot", "RepInfo");
		md.HasAllProperties("TRepInfo", "Id,Views,Grouping,Filters");
		md.IsId("TRepInfo", "Id");

		var dt = new DataTester(dm, "RepInfo");
		dt.AreValueEqual(1, "Id");

		dt = new DataTester(dm, "RepInfo.Views");
		dt.IsArray(1);
		dt.AreArrayValueEqual("View", 0, "Name");

		dt = new DataTester(dm, "RepInfo.Grouping");
		dt.IsArray(1);
		dt.AreArrayValueEqual("Grouping", 0, "Name");

		dt = new DataTester(dm, "RepInfo.Filters");
		dt.IsArray(1);
		dt.AreArrayValueEqual("Filter", 0, "Name");
	}

}


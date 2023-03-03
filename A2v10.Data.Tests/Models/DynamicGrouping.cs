using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Newtonsoft.Json;

using A2v10.Data.Interfaces;
using A2v10.Data.Tests.Configuration;
using A2v10.Data.ScriptBuilder;
using A2v10.Data.Tests;

namespace A2v10.Data.Models;

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
	public async Task LoadDynamicGroups()
	{
		var dm = await _dbContext.LoadModelAsync(null, "a2test.[DynamicGrouping]");
	}

}
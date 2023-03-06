// Copyright © 2022-2023 Oleksandr Kukhtin. All rights reserved.

using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;


using A2v10.Data.Interfaces;
using A2v10.Data.Tests.Configuration;

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
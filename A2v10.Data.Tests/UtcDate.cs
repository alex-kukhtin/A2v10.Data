﻿// Copyright © 2015-2023 Oleksandr Kukhtin. All rights reserved.

using System;
using System.Threading.Tasks;
using A2v10.Data.Interfaces;
using A2v10.Data.Tests.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A2v10.Data.Tests;

[TestClass]
[TestCategory("Date operations")]
public class UtcDate
{
	readonly IDbContext _dbContext;
	public UtcDate()
	{
		_dbContext = Starter.Create();
	}

	[TestMethod]
	public async Task CheckUtcDate()
	{
		var dm = await _dbContext.LoadModelAsync(null, "a2test.[UtcDate.Load]");
		var md = new MetadataTester(dm);
		md.IsAllKeys("TRoot,TModel");
		md.HasAllProperties("TRoot", "Model");
		md.HasAllProperties("TModel", "Date,UtcDate");

		var now = DateTime.Now;
		var utcNow = DateTime.UtcNow;

		var dt = new DataTester(dm, "Model");
		var mdate = dt.GetValue<DateTime>("Date");
		var mutc = dt.GetValue<DateTime>("UtcDate");

		Assert.IsTrue(Math.Abs((mdate - now).TotalSeconds) < 2);
		Assert.IsTrue(Math.Abs((mutc- now).TotalSeconds) < 2);
	}
}

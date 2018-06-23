// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Dynamic;
using System.Threading.Tasks;
using A2v10.Data.Interfaces;
using A2v10.Data.Tests.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A2v10.Data.Tests
{
	[TestClass]
	public class DatabaseModels
	{
		IDbContext _dbContext;
		public DatabaseModels()
		{
			_dbContext = Starter.Create();
		}

		[TestMethod]
		public async Task DocumentWithRowsAndMethods()
		{
			var dm = await _dbContext.LoadModelAsync(null, "a2test.[Document.RowsMethods.Load]");

			var md = new MetadataTester(dm);
			md.IsAllKeys("TRoot,TDocument,TRow,TMethod");
			md.HasAllProperties("TRoot", "Document");
			md.HasAllProperties("TDocument", "Name,Id,Rows");
			md.IsId("TDocument", "Id");

			md.HasAllProperties("TRow", "Id,Methods");
			md.HasAllProperties("TMethod", "Key,Name");

			var dt = new DataTester(dm, "Document");
			dt.AreValueEqual(123, "Id");

			dt = new DataTester(dm, "Document.Rows");
			dt.IsArray(1);

			dt = new DataTester(dm, "Document.Rows[0].Methods.Mtd1");
			dt.AreValueEqual("Mtd1", "Key");
			dt.AreValueEqual("Method 1", "Name");

			dt = new DataTester(dm, "Document.Rows[0].Methods.Mtd2");
			dt.AreValueEqual("Mtd2", "Key");
			dt.AreValueEqual("Method 2", "Name");
		}
	}
}

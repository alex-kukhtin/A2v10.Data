// Copyright © 2015-2023 Oleksandr Kukhtin. All rights reserved.

using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using A2v10.Data.Providers.Csv;
using A2v10.Data.Tests.Providers;
using A2v10.Data.Interfaces;
using A2v10.Data.Tests.Configuration;
using System.Threading.Tasks;

namespace A2v10.Data.Providers;

[TestClass]
[TestCategory("Providers")]
public class CsvReaderTest
{

	private readonly IDbContext _dbContext;

	public CsvReaderTest()
	{
		_dbContext = Starter.Create();
	}

	[TestMethod]
	public void CsvReadSimpleFile()
	{
		var f = new DataFile();
		var rdr = new CsvReader(f);

		using (var file = File.Open("../../../testfiles/simple.csv", FileMode.Open))
		{
			rdr.Read(file);
		}

		var wrt = new CsvWriter(f);
		using (var file = File.Create("../../../testfiles/output.csv"))
		{
			wrt.Write(file);
		}

		ProviderTools.CompareFiles("../../../testfiles/simple.csv", "../../../testfiles/output.csv");
	}

	[TestMethod]
	public void CsvReadSomeRecordsFile()
	{
		var f = new DataFile();
		var rdr = new CsvReader(f);

		using (var file = File.Open("../../../testfiles/records.csv", FileMode.Open))
		{
			rdr.Read(file);
		}

		var wrt = new CsvWriter(f);
		using (var file = File.Create("../../../testfiles/recordsout.csv"))
		{
			wrt.Write(file);
		}

		var nf = new DataFile();
		var nrdr = new CsvReader(nf);
		using (var file = File.Open("../../../testfiles/recordsout.csv", FileMode.Open))
		{
			nrdr.Read(file);
		}
		Assert.AreEqual(f.FieldCount, nf.FieldCount);
		Assert.AreEqual(f.NumRecords, nf.NumRecords);
		for (var c = 0; c < f.FieldCount; c++)
		{
			var f1 = f.GetField(c);
			var f2 = nf.GetField(c);
			Assert.AreEqual(f1.Name, f2.Name);
		}

		for (var r = 0; r < f.NumRecords; r++)
		{
			var r1 = f.GetRecord(r);
			var r2 = nf.GetRecord(r);
			for (var c = 0; c < f.FieldCount; c++)
			{
				var v1 = r1.DataFields[c];
				var v2 = r2.DataFields[c];
				Assert.AreEqual(v1.StringValue, v2.StringValue);
			}
		}
	}


	[TestMethod]
	public void CsvReadExternalFile()
	{
		/*
		var f = new DataFile();
		var rdr = new CsvReader(f);

		using (var file = File.Open("../../../testfiles/export.csv", FileMode.Open))
		{
			rdr.Read(file);
		}
		var wrt = new CsvWriter(f);
		using (var file = File.Create("../../../testfiles/extenral_output.csv"))
		{
			wrt.Write(file);
		}
		*/
	}

	[TestMethod]
	public void CsvReadLargeFile()
	{
		/*
		var f = new DataFile();
		var rdr = new CsvReader(f);

		using (var file = File.Open("", FileMode.Open))
		{
			var dm = rdr.CreateDataModel(file);
			int z = 55;
		}

		var x = f.FieldCount;
		*/
	}

	[TestMethod]
	public void CsvReadUquoted()
	{
		var f = new DataFile();
		var rdr = new CsvReader(f);

		using var file = File.Open("../../../testfiles/uquoted.csv", FileMode.Open);
		var df = rdr.Read(file);
	}

	[TestMethod]
	public void CsvReadTabbedFile()
	{
		var f = new DataFile();
		var rdr = new CsvReader(f);

		using (var file = File.Open("../../../testfiles/tabbed.csv", FileMode.Open))
		{
			rdr.Read(file);
		}

		var wrt = new CsvWriter(f);
		using (var file = File.Create("../../../testfiles/output.csv"))
		{
			wrt.Write(file);
		}

		ProviderTools.CompareFiles("../../../testfiles/tabbed.csv", "../../../testfiles/output.csv");
	}

	[TestMethod]
	public Task CsvReadTabbedModel()
	{
		/*
		var f = new DataFile();
		var rdr = new CsvReader(f);

		using (var file = File.Open("../../../testfiles/tabbed.csv", FileMode.Open))
		{
			var dm = rdr.CreateDataModel(file);

			var result = await _dbContext.SaveModelAsync(null, "Tabbed.Csv.Update", dm);
		}
		*/
		return Task.CompletedTask;
    }

    [TestMethod]
    public void CsvReadZeroSpaceModel()
    {
		/*
        var f = new DataFile();
        var rdr = new CsvReader(f);

        using (var file = File.Open("../../../testfiles/novapay.csv", FileMode.Open))
        {
            var dm = rdr.CreateDataModel(file);

			int z = 55;
        }
		*/
    }

    [TestMethod]
    public void CsvReadStrangeCoding()
    {
        var f = new DataFile();
        var rdr = new CsvReader(f);

        using (var file = File.Open("../../../testfiles/strange_encoding.csv", FileMode.Open))
        {
            var dm = rdr.CreateDataModel(file);
        }
    }
}

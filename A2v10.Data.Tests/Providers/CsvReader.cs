// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using A2v10.Data.Providers.Csv;
using A2v10.Data.Tests.Providers;

namespace A2v10.Data.Providers
{
	[TestClass]
	[TestCategory("Providers")]
	public class CsvReaderTest
	{
		[TestMethod]
		public void CsvReadSimpleFile()
		{
			var f = new DataFile();
			var rdr = new CsvReader(f);

			using (var file = File.Open("../../testfiles/simple.csv", FileMode.Open))
			{
				rdr.Read(file);
			}

			var wrt = new CsvWriter(f);
			using (var file = File.Create("../../testfiles/output.csv"))
			{
				wrt.Write(file);
			}

			ProviderTools.CompareFiles("../../testfiles/simple.csv", "../../testfiles/output.csv");
		}
	}
}

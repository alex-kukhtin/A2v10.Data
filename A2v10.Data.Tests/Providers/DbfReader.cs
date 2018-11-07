using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using A2v10.Data.Providers;
using A2v10.Data.Providers.Dbf;
using System.IO;
using System.Text;

namespace A2v10.Data.Providers
{
	[TestClass]
	[TestCategory("Providers")]
	public class DbfReaderTest
	{
		[TestMethod]
		public void DbfReadSimpleFile()
		{
			var f = new DataFile()
			{
				Encoding = Encoding.GetEncoding(866)
			};
			var rdr = new DbfReader(f);

			using (var file = File.Open("../../testfiles/simple.dbf", FileMode.Open))
			{
				rdr.Read(file);
			}

			var wrt = new DbfWriter(f);
			using (var file = File.Open("../../testfiles/output.dbf", FileMode.OpenOrCreate|FileMode.Truncate))
			{
				wrt.Write(file);
			}

			CompareFiles("../../testfiles/simple.dbf", "../../testfiles/output.dbf");
		}

		void CompareFiles(String file1, String file2)
		{
			var b1 = File.ReadAllBytes(file1);
			var b2 = File.ReadAllBytes(file2);
			Assert.AreEqual(b1.Length, b2.Length);
			for (Int32 i=0; i<b1.Length; i++)
			{
				if (b1[i] != b2[i])
					Assert.IsTrue(b1[i] == b2[i]);
			}
		}

		[TestMethod]
		public void DbfReadAutoEncoding()
		{
			var f = new DataFile()
			{
				Encoding = null // AUTO
			};

			var rdr = new DbfReader(f);

			using (var file = File.Open("../../testfiles/ENCODING.dbf", FileMode.Open))
			{
				rdr.Read(file);
			}

			var wrt = new DbfWriter(f);
			using (var file = File.Open("../../testfiles/output.dbf", FileMode.OpenOrCreate | FileMode.Truncate))
			{
				wrt.Write(file);
			}

			CompareFiles("../../testfiles/ENCODING.dbf", "../../testfiles/output.dbf");
		}

	}
}

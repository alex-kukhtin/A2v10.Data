// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Text;

using A2v10.Data.Interfaces;
using A2v10.Data.Providers.Csv;
using A2v10.Data.Providers.Dbf;

namespace A2v10.Data.Providers
{
	public class ExternalDataContext : IExternalDataProvider
	{
		#region IExternalDataProvider

		public IExternalDataReader GetReader(String format, Encoding enc)
		{
			switch (format?.ToLowerInvariant())
			{
				case "dbf":
					var dataFileDbf = new DataFile()
					{
						Encoding = enc
					};
					return new DbfReader(dataFileDbf);
				case "csv":
					var dataFileCsv = new DataFile()
					{
						Encoding = enc
					};
					return new CsvReader(dataFileCsv);
			}
			return null;
		}
		#endregion
	}
}

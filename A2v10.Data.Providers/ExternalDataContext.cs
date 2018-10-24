// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using A2v10.Data.Interfaces;
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
					var dataFile = new DataFile()
					{
						Encoding = enc
					};
					return new DbfReader(dataFile);
			}
			return null;
		}
		#endregion
	}
}

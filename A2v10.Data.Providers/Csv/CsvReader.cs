// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.IO;

using A2v10.Data.Interfaces;

namespace A2v10.Data.Providers.Csv
{
	public class CsvReader :  IExternalDataReader
	{
		private readonly DataFile _file;

		public CsvReader(DataFile file)
		{
			_file = file;
		}

		public IExternalDataFile Read(Stream stream)
		{
			using (TextReader rdr = new StreamReader(stream))
			{
				Read(rdr);
			}
			return null; 
		}

		void Read(TextReader rdr)
		{
			rdr.ReadLine();
		}
	}
}

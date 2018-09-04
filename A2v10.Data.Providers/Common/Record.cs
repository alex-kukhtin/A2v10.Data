using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Data.Providers
{
	public class Record
	{
		public List<FieldData> DataFields;

		public Record()
		{
			DataFields = new List<FieldData>();
		}

		public Record(List<FieldData> dat)
		{
			DataFields = dat;
		}
	}
}

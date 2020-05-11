using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A2v10.Data.Interfaces;

namespace A2v10.Data.Providers
{
	internal class FlatTable : ITableDescription
	{
		private readonly List<Object> _list;

		public FlatTable()
		{
			_list = new List<Object>();
		}

		public ExpandoObject NewRow()
		{
			var nr = new ExpandoObject();
			_list.Add(nr);
			return nr;
		}

		public void SetValue(ExpandoObject obj, String propName, Object value)
		{
			var d = obj as IDictionary<String, Object>;
			d.Add(propName, value);
		}

		public ExpandoObject ToObject()
		{
			var eo = new ExpandoObject();
			var d = eo as IDictionary<String, Object>;
			d.Add("Rows", _list);
			return eo;
		}
	}
}

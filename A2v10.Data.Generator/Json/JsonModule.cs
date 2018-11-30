// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace A2v10.Data.Generator
{
	public class JsonModule
	{
		[JsonProperty("schema")]
		public String Schema { get; set; }

		[JsonProperty("tables")]
		public Dictionary<String, JsonTable> Tables { get; set; }

		[JsonProperty("models")]
		public Dictionary<String, JsonModel> Models { get; set; }

		public void EndInit()
		{
			if (Tables != null)
				foreach (var t in Tables.Values)
					t.EndInit(this);
			if (Models != null)
				foreach (var m in Models.Values)
					m.EndInit(this);
		}
	}
}

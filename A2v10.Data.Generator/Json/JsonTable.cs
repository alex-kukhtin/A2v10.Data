// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace A2v10.Data.Generator
{
	public class JsonTable
	{
		[JsonIgnore]
		JsonModule _parent;

		[JsonIgnore]
		internal JsonModule Parent => _parent;

		[JsonProperty("schema")]
		public String Schema { get; set; }
		

		[JsonProperty("columns")]
		public Dictionary<String, JsonColumn> Columns { get; set; }

		internal void EndInit(JsonModule parent)
		{
			_parent = parent;
		}

		public String CurrentSchema => String.IsNullOrEmpty(Schema) ? _parent.Schema : Schema;
	}
}

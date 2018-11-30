// Copyright © 2018 Alex Kukhtin. All rights reserved.

using System;
using Newtonsoft.Json;

namespace A2v10.Data.Generator
{
	public class JsonModel
	{
		[JsonIgnore]
		JsonModule _parent;

		[JsonIgnore]
		internal JsonModule Parent => _parent;

		[JsonProperty("base")]
		public String Base { get; set; }

		[JsonIgnore]
		public JsonTable BaseTable => _parent.Tables[this.Base];

		internal void EndInit(JsonModule parent)
		{
			_parent = parent;
			if (!_parent.Tables.ContainsKey(Base))
				throw new DataCreatorException($"Table '{Base}' not found in solution");
		}
	}
}

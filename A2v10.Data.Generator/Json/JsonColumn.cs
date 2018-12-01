// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;

using Newtonsoft.Json;

namespace A2v10.Data.Generator
{
	public class JsonColumn
	{
		[JsonProperty("name")]
		public String Name { get; set; }

		[JsonProperty("type")]
		public FieldType Type { get; set; }

		[JsonProperty("size")]
		public Int32 Size { get; set; }

		[JsonProperty("parent")]
		public String Parent { get; set; }

		[JsonProperty("reference")]
		public String Reference { get; set; }

		[JsonProperty("primaryKey")]
		public Boolean PrimaryKey { get; set; }

		[JsonProperty("default")]
		public Object Default { get; set; }

		[JsonIgnore]
		public Boolean IsParent => !String.IsNullOrEmpty(Parent);

		[JsonIgnore]
		public Boolean IsReference => !String.IsNullOrEmpty(Reference);
	}
}
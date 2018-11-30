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
	}
}

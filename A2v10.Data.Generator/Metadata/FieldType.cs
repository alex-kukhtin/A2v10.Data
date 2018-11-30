// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.


using Newtonsoft.Json;

namespace A2v10.Data.Generator
{
	public enum FieldType
	{
		[JsonProperty("varchar")]
		VarChar,

		[JsonProperty("char")]
		Char,

		[JsonProperty("datetime")]
		DateTime,

		[JsonProperty("int")]
		Integer,

		[JsonProperty("sequence")]
		Sequence,

		[JsonProperty("float")]
		Float,

		[JsonProperty("boolean")]
		Boolean,

		[JsonProperty("money")]
		Money,

		[JsonProperty("reference")]
		Reference,

		[JsonProperty("array")]
		Array,

		[JsonProperty("parent")]
		Parent
	}
}

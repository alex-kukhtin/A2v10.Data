// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Data.Generator.Solution
{
	public class Table
	{
		[JsonIgnore]
		Module _parent;
		[JsonIgnore]
		internal Module Parent => _parent;

		[JsonProperty("columns")]
		public Dictionary<String, Column> Columns { get; set; }

		internal void EndInit(Module parent)
		{
			_parent = parent;
		}
	}
}

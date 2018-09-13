// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace A2v10.Data.Generator.Solution
{
	public class Model
	{
		[JsonIgnore]
		Module _parent;
		[JsonIgnore]
		internal Module Parent => _parent;

		[JsonProperty("base")]
		public String Base { get; set; }

		[JsonIgnore]
		public Table BaseTable => _parent.Tables[this.Base];

		internal void EndInit(Module parent)
		{
			_parent = parent;
			if (!_parent.Tables.ContainsKey(Base))
				throw new DataCreatorException($"Table '{Base}' not found in solution");
		}
	}
}

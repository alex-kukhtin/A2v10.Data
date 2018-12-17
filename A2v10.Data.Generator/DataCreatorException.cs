// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Data.Generator
{
	[Serializable]
	public sealed class DataCreatorException : Exception
	{
		public DataCreatorException(String message)
			: base(message)
		{

		}
	}
}

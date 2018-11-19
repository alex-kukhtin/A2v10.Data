// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;

namespace A2v10.Data.Interfaces
{
	public interface IDataScripter
	{
		String CreateScript(IDictionary<String, Object> sys, IDictionary<String, IDataMetadata> meta);
		String CreateDataModelScript(IDataModel model);
	}
}

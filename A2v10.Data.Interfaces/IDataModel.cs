// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Dynamic;

namespace A2v10.Data.Interfaces
{
	public interface IDataModel
	{
		ExpandoObject Root { get; }
		ExpandoObject System { get; }
		IDictionary<String, IDataMetadata> Metadata { get; }

		Object FirstElementId { get; }
		Boolean IsReadOnly { get; }
		void SetReadOnly();

		T Eval<T>(String expression);
		T Eval<T>(ExpandoObject root, String expression);

		T CalcExpression<T>(String expression);
		T CalcExpression<T>(ExpandoObject root, String expression);

		void Merge(IDataModel src);

		String CreateScript(IDataScripter scripter);
		IDictionary<String, dynamic> GetDynamic();
	}
}

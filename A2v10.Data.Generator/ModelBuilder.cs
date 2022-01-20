// Copyright © 2015-2022 Alex Kukhtin. All rights reserved.

using System;
using System.Text;

namespace A2v10.Data.Generator
{
	public class ModelBuilder
	{
		public StringBuilder StringBuilder => _stringBuilder;
		public Boolean MultiTenant { get; set; }

		StringBuilder _stringBuilder;
		String _divider { get; set; }

		public ModelBuilder()
		{
			_stringBuilder = new StringBuilder();
			_divider = new String('-', 32);
		}


		public void BuildTenantParam()
		{
			if (!MultiTenant) return;
			_stringBuilder.AppendLine("@TenantId int,");
		}

		public void WriteDivider()
		{
			_stringBuilder.AppendLine(_divider);
		}

		public String TenantParamEQ => MultiTenant ? "@TenantId = @TenantId," : String.Empty;

		public void Clear()
		{
			_stringBuilder = new StringBuilder();
		}

		public override String ToString()
		{
			return _stringBuilder.ToString();
		}
	}
}

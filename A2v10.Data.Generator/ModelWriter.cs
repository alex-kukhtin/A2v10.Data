// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Data.Generator
{
	public class ModelWriter
	{
		public StringWriter Writer { get; set; }

		public Boolean MultiTenant { get; set; }

		public void WriteTenantParam()
		{
			if (!MultiTenant) return;
			Writer.WriteLine("@TenantId int,");
		}

		public String TenantParamEQ => MultiTenant ? "@TenantId = @TenantId," : String.Empty;
	}
}

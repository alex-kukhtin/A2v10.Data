// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;

namespace A2v10.Data.Providers
{
	public sealed class FieldData 
	{
		public DateTime DateValue { get; set; }
		public Decimal DecimalValue { get; set; }
		public String StringValue { get; set; }
		public Boolean BooleanValue { get; set; }

		public FieldType FieldType { get; set; }
	}
}

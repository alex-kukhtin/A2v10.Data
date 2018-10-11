// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;

namespace A2v10.Data.DynamicExpression
{
	public struct NaN
	{
		public static NaN Value => new NaN();
		public static Boolean IsNaN(Object test)
		{
			return test is NaN;
		}
	}

	public struct Undefined
	{
		public static Undefined Value => new Undefined();
		public static Boolean IsUndefined(Object test)
		{
			return test is Undefined;
		}
	}

	public struct Infinity
	{
		public static Infinity Value => new Infinity();
		public static Boolean IsInfinity(Object test)
		{
			return test is Infinity;
		}
	}
}

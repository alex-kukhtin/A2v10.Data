// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.IO;

namespace A2v10.Data.Generator
{
	public static class Helpers
	{
		public static void RemoveTailCommma(this StringWriter writer)
		{
			var sb = writer.GetStringBuilder();
			var len = sb.Length;
			Char ch = sb[len - 1];
			while (ch == '\r' || ch == '\n' || ch == ' ')
			{
				sb.Remove(len - 1, 1);
				ch = sb[len - 1];
				len--;
			}
			if (ch == ',')
				sb.Remove(len - 1, 1);
		}
	}
}

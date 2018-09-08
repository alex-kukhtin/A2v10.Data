// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.IO;
using System.Text;

namespace A2v10.Data.Generator
{
	public static class Helpers
	{
		public static StringBuilder RemoveTailCommma(this StringBuilder sb)
		{
			var last = sb.Length - 1;
			if (last < 0) return sb;
			Char ch = sb[last];
			while (last > 0 && (ch == '\r' || ch == '\n' || ch == ' ' || ch == ','))
			{
				if (ch == ',')
				{
					sb.Remove(last, 1);
					return sb;
				}
				last -= 1;
				ch = sb[last];
			}
			return sb;
		}

		public static StringBuilder RemoveTailSpace(this StringBuilder sb)
		{
			var last = sb.Length - 1;
			if (last < 0) return sb;
			Char ch = sb[last];
			if (ch == ' ')
				sb.Remove(last, 1);
			return sb;
		}

		public static String Plural(this String s)
		{
			if (s == null || s.Length < 1) return s;
			var ch = s[s.Length - 1];

			if (ch == 's')
				return s + "es";
			else if (ch == 'y')
				return  s.Substring(0, s.Length - 1) + "ies";
			return s + "s";
		}
	}
}

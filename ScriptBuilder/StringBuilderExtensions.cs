using System;
using System.Text;

namespace ScriptBuilder
{
	public static class StringBuilderExtensions
	{
		public static StringBuilder RemoveTailComma(this StringBuilder sb)
		{
			if (sb.Length < 1)
				return sb;
			Int32 len = sb.Length;
			if (sb[len - 1] == ',')
				sb.Remove(len - 1, 1);
			return sb;
		}
	}
}

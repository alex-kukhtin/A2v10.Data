using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2v10.Data.DynamicExpression
{
	public static class TypeHelpers
	{
		public static bool IsNullableType(this Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public static Type GetNonNullableType(this Type type)
		{
			return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
		}
	}
}

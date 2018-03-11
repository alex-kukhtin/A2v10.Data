// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Text.RegularExpressions;

namespace A2v10.Data
{
	public static class DynamicExtensions
	{
		public static T Get<T>(this ExpandoObject obj, String name)
		{
			var d = obj as IDictionary<String, Object>;
			if (d == null)
				return default(T);
			if (d.TryGetValue(name, out Object result))
			{
				if (result is T)
					return (T)result;
			}
			return default(T);
		}

		public static T GetOrCreate<T>(this ExpandoObject obj, String name) where T: new()
		{
			var d = obj as IDictionary<String, Object>;
			if (d == null)
				return default(T);
			if (d.TryGetValue(name, out Object result))
			{
				if (result is T)
					return (T)result;
				else
					throw new InvalidCastException();
			}
			var no = new T();
			d.Add(name, no);
			return no;
		}

		public static Object GetObject(this ExpandoObject obj, String name)
		{
			var d = obj as IDictionary<String, Object>;
			if (d == null)
				return null;
			if (d.TryGetValue(name, out Object result))
			{
				return result;
			}
			return null;
		}

		public static void Set(this ExpandoObject obj, String name, Object value)
		{
			var d = obj as IDictionary<String, Object>;
			if (d == null)
				return;
			if (d.ContainsKey(name))
				d[name] = value;
			else
				d.Add(name, value);
		}

		public static T Eval<T>(this ExpandoObject root, String expression, T fallback = default(T), Boolean throwIfError = false)
		{
			if (expression == null)
				return fallback;
			Object result = root.EvalExpression(expression, throwIfError);
			if (result == null)
				return fallback;
			if (result is T)
				return (T)result;
			return fallback;
		}

		private static Object EvalExpression(this ExpandoObject root, String expression, Boolean throwIfError = false)
		{
			Object currentContext = root;
			var arrRegEx = new Regex(@"(\w+)\[(\d+)\]{1}");
			foreach (var exp in expression.Split('.'))
			{
				if (currentContext == null)
					return null;
				String prop = exp.Trim();
				var d = currentContext as IDictionary<String, Object>;
				if (prop.Contains("["))
				{
					var match = arrRegEx.Match(prop);
					prop = match.Groups[1].Value;
					if ((d != null) && d.ContainsKey(prop))
					{
						var x = d[prop] as IList<ExpandoObject>;
						currentContext = x[Int32.Parse(match.Groups[2].Value)];
					}
					else
					{
						if (throwIfError)
							throw new ArgumentException($"Error in expression '{expression}'. Property '{prop}' not found");
						return null;
					}
				}
				else
				{
					if ((d != null) && d.ContainsKey(prop))
						currentContext = d[prop];
					else
					{
						if (throwIfError)
							throw new ArgumentException($"Error in expression '{expression}'. Property '{prop}' not found");
						return null;
					}
				}
			}
			return currentContext;
		}

	}
}

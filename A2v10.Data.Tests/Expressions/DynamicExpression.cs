// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Dynamic;
using System.Linq.Expressions;
using A2v10.Data.DynamicExpression;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A2v10.Data.Tests.Expressions
{
	[TestClass]
	[TestCategory("Expressions")]
	public class Expressions
	{
		Object CalcSimpleExpression(String expression)
		{
			var prms = new ParameterExpression[] {
			};
			var lexpr = DynamicParser.ParseLambda(prms, null, expression, null);
			var lambda = lexpr.Compile();
			return lambda.DynamicInvoke();
		}

		Object CalcExpression(String expression, String prm, Object value)
		{
			var prms = new ParameterExpression[] {
				Expression.Parameter(typeof(Object), prm)
			};
			var lexpr = DynamicParser.ParseLambda(prms, null, expression, null);
			var lambda = lexpr.Compile();
			return lambda.DynamicInvoke(value);
		}

		[TestMethod]
		public void UnaryOperator()
		{
			var result = CalcSimpleExpression("+ 2");
			Assert.AreEqual(2M, result); // as decimal
			result = CalcSimpleExpression("- 2");
			Assert.AreEqual(-2M, result); // as decimal
			result = CalcSimpleExpression("-'2'");
			Assert.AreEqual(-2M, result); // as decimal
			result = CalcSimpleExpression("+'2'");
			Assert.AreEqual(2M, result); // as decimal
			result = CalcSimpleExpression("-'a'");
			Assert.IsTrue(NaN.IsNaN(result));
			result = CalcSimpleExpression("-true");
			Assert.AreEqual(-1M, result); // as decimal
			result = CalcSimpleExpression("+false");
			Assert.AreEqual(0M, result); // as decimal
		}

		[TestMethod]
		public void MultiplicativeOperations()
		{
			var result = CalcSimpleExpression("2 * 2");
			Assert.AreEqual(result, 4M); // as decimal

			result = CalcSimpleExpression("2 * '2'");
			Assert.AreEqual(result, 4M); // as decimal

			result = CalcSimpleExpression("'3'* 2");
			Assert.AreEqual(result, 6M); // as decimal

			result = CalcSimpleExpression("2 * true");
			Assert.AreEqual(result, 2M); // as decimal

			result = CalcSimpleExpression("4 / 2");
			Assert.AreEqual(result, 2M); // as decimal

			result = CalcSimpleExpression("4 / 0");
			Assert.IsTrue(Infinity.IsInfinity(result));

			result = CalcSimpleExpression("4 / false");
			Assert.IsTrue(Infinity.IsInfinity(result));

			result = CalcSimpleExpression("2 / true");
			Assert.AreEqual(result, 2M); // as decimal
		}

		[TestMethod]
		public void AdditiveOperations()
		{
			var result = CalcSimpleExpression("2 + 2");
			Assert.AreEqual(result, 4M); // as decimal

			result = CalcSimpleExpression("'2' + 4");
			Assert.AreEqual(result, "24");

			result = CalcSimpleExpression("2 + '4'");
			Assert.AreEqual(result, "24");

			result = CalcSimpleExpression("'aaa' + 'bbb'");
			Assert.AreEqual(result, "aaabbb");

			result = CalcSimpleExpression("5 - '3'");
			Assert.AreEqual(result, 2M);

			result = CalcSimpleExpression("'5' - - 8");
			Assert.AreEqual(result, 13M);

			result = CalcSimpleExpression("'s' - 23");
			Assert.IsTrue(NaN.IsNaN(result));
		}

		[TestMethod]
		public void TernaryOperation()
		{
			var result = CalcSimpleExpression("2 === 2 ? 'yes' : 'no'");
			Assert.AreEqual(result, "yes");

			result = CalcSimpleExpression("2 == 2 ? true : false");
			Assert.AreEqual(result, true);

			result = CalcSimpleExpression("2 !== 2 ? 'yes' : 'no'");
			Assert.AreEqual(result, "no");

			result = CalcSimpleExpression("2 !== 2 ? 'yes' : null");
			Assert.AreEqual(result, null);
		}

		[TestMethod]
		public void MemberAccess()
		{

			var agent = new ExpandoObject();
			agent.Set("Name", "agent name");
			var addr = new ExpandoObject();
			addr.Set("Text", "text");
			agent.Set("Address", addr);

			var result = CalcExpression("Agent.Name", "Agent", agent);
			Assert.AreEqual(result, "agent name");

			result = CalcExpression("Agent.Address.Text", "Agent", agent);
			Assert.AreEqual(result, "text");
		}

		[TestMethod]
		public void ComparisonOperation()
		{
			var result = CalcSimpleExpression("3 > 2");
			Assert.AreEqual(result, true);

			result = CalcSimpleExpression("2 >= 2");
			Assert.AreEqual(result, true);

			result = CalcSimpleExpression("2 > 3");
			Assert.AreEqual(result, false);

			result = CalcSimpleExpression("2 < 3");
			Assert.AreEqual(result, true);

			result = CalcSimpleExpression("2 <= 3");
			Assert.AreEqual(result, true);

			result = CalcSimpleExpression("'aaa' < 'bbb'");
			Assert.AreEqual(result, true);
		}

	}
}

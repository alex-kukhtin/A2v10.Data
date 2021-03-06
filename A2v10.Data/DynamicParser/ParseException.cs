﻿// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;
using System.Runtime.Serialization;

namespace A2v10.Data.DynamicExpression
{
	[Serializable]
	public sealed class ParseException : Exception
	{
		readonly Int32 position;

		public ParseException(String message, Int32 position)
			: base(message)
		{
			this.position = position;
		}

		public Int32 Position
		{
			get { return position; }
		}

		public override String ToString()
		{
			return String.Format(Res.ParseExceptionFormat, Message, position);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}

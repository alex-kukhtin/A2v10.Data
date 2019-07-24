// Copyright © 2012-2019 Alex Kukhtin. All rights reserved.

using System;

namespace A2v10.Data
{
	[Serializable]
	public class DataLoaderException : Exception
	{
		public DataLoaderException(String message)
			: base(message)
		{
		}
	}

	[Serializable]
	public class DataWriterException : Exception
	{
		public DataWriterException(String message)
			: base(message)
		{
		}
	}

	[Serializable]
	public class DataDynamicException : Exception
	{
		public DataDynamicException(String message)
			: base(message)
		{
		}
	}

	[Serializable]
	public class DataValidationException: Exception
	{
		public DataValidationException(String message)
			: base(message)
		{
		}
	}
}

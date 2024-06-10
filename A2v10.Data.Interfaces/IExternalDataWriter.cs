// Copyright © 2019-2024 Oleksandr Kukhtin. All rights reserved.

using System;
using System.IO;

namespace A2v10.Data.Interfaces;

public interface IExternalDataWriter
{
	void SetDelimiter(Char delimiter);
	void Write(Stream stream);
}

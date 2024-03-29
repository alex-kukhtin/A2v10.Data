﻿// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using A2v10.Data.Interfaces;
using System;
using System.Configuration;

namespace A2v10.Data.Tests.Configuration;

public class TestConfig : IDataConfiguration
{
    public String ConnectionString(String source)
    {
        if (String.IsNullOrEmpty(source))
            source = "Default";
        var cnnStr = ConfigurationManager.ConnectionStrings[source] 
            ?? throw new ConfigurationErrorsException($"Connection string '{source}' not found");
		return cnnStr.ConnectionString;
    }
}

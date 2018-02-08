// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using A2v10.Data.Interfaces;
using System;

namespace A2v10.Data.Tests.Configuration
{
    public class TestLocalizer : IDataLocalizer
    {
        public String Localize(String content)
        {
            return content;
        }
    }
}
